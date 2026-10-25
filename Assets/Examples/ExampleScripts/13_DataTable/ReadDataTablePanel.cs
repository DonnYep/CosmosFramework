using Cosmos;
using Cosmos.DataTable;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ReadDataTablePanel : MonoBehaviour
{
    readonly string dataTableName = "SampleCSV";
    Button btnReadDataTable;
    Button btnReadRow;
    Text txtContent;
    Dropdown drpdPopup;
    IDataTable<SampleCSVDataRow> dataTable;
    int currentPopupIndex;
    bool canReadRow;
    void Start()
    {
        btnReadDataTable = gameObject.GetComponentInChildren<Button>("BtnReadDataTable");
        btnReadRow = gameObject.GetComponentInChildren<Button>("BtnReadRow");
        txtContent = gameObject.GetComponentInChildren<Text>("TxtContent");
        drpdPopup = gameObject.GetComponentInChildren<Dropdown>("DrpdPopup");
        btnReadDataTable.onClick.AddListener(OnReadDataTableClick);
        btnReadRow.onClick.AddListener(OnReadRowClick);
        drpdPopup.onValueChanged.AddListener(OnPopup);
    }
    void OnReadDataTableClick()
    {
        var manager = CosmosEntry.DataTableManager;
        //表只创建一次，重复点击复用同一份共享数据
        dataTable = manager.HasDataTable(dataTableName)
            ? manager.GetDataTable<SampleCSVDataRow>(dataTableName)
            : manager.CreateDataTable<SampleCSVDataRow>(dataTableName);
        //幂等加载：已在加载中会忽略；已加载会立即回调
        manager.ReadDataTableAssetAsync(new DataTableAssetInfo(dataTableName), dataTable, OnDataTableReadCompleted);
    }
    void OnReadRowClick()
    {
        if (!canReadRow)
            return;
        //这里+1是因为SampleCSV这个表本身是从1开始的；行不存在时索引器返回 null
        var row = dataTable[currentPopupIndex + 1];
        txtContent.text = row == null ? "row not found" : row.SampleCSV.ToString();
    }
    void OnPopup(int index)
    {
        currentPopupIndex = index;
    }
    void OnDataTableReadCompleted(DataTableBase table)
    {
        if (table.State == DataTableState.Failed)
        {
            //失败原因不再丢失：表上带 Error
            txtContent.text = table.Error;
            Utility.Debug.LogError($"{table.Name} ReadFailure : {table.Error}");
            canReadRow = false;
            return;
        }
        Utility.Debug.LogInfo($"{table.Name} ReadSuccess , rows : {table.RowCount}");
        drpdPopup.ClearOptions();
        drpdPopup.AddOptions(dataTable.Rows.Select(r => (r.Id.ToString() + " Row")).ToList());
        canReadRow = true;
        drpdPopup.interactable = true;
        btnReadRow.interactable = true;
    }
}
