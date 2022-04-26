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
        if (CosmosEntry.DataTableManager.HasDataTable(dataTableName))
            return;
        dataTable = CosmosEntry.DataTableManager.CreateDataTable<SampleCSVDataRow>(dataTableName);
        var dataTableBase = (DataTableBase)dataTable;
        dataTableBase.OnReadSuccess += OnDataTableReadSuccess;
        dataTableBase.OnReadFailure += OnDataTableReadFailure;
        CosmosEntry.DataTableManager.ReadDataTableAssetAsync(new DataTableAssetInfo(dataTableName), dataTableBase);
    }
    void OnReadRowClick()
    {
        if (!canReadRow)
            return;
        //这里+1是因为SampleCSV这个表本身是从1开始的
        txtContent.text = this.dataTable[currentPopupIndex + 1].SampleCSV.ToString();
    }
    void OnPopup(int index)
    {
        currentPopupIndex = index;
    }
    void OnDataTableReadSuccess(DataTableBase dataTable)
    {
        Utility.Debug.LogInfo($"{dataTable.Name} ReadSuccess");
        var rowDatas = this.dataTable.GetAllRowDatas();
        drpdPopup.AddOptions(rowDatas.Select(r => (r.Id.ToString() + " Row")).ToList());
        canReadRow = true;
        drpdPopup.interactable = true;
        btnReadRow.interactable = true;
    }
    void OnDataTableReadFailure(DataTableBase dataTable)
    {
        Utility.Debug.LogInfo($"{dataTable.Name} ReadFailure", DebugColor.red); ;
    }
}
