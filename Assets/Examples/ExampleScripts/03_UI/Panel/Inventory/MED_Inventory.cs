using System;
using System.Collections.Generic;
using PureMVC;
using Cosmos;
/// <summary>
/// 一个Mediator持有一个panel 的ViewEntity；
/// </summary>
public class MED_Inventory : Mediator
{
    public new const string NAME = "MED_Inventory";

    InventoryPanel InventoryPanel;

    public MED_Inventory() : base(NAME)
    {
        EventKeys = new List<string>();
        EventKeys.Add(MVCEventDefine.CMD_Inventory);
    }
    PRX_Inventory PRX_Inventory { get { return MVC.PeekProxy<PRX_Inventory>(PRX_Inventory.NAME); } }
    public override void HandleEvent(INotifyArgs notifyArgs)
    {
        var data = notifyArgs.NotifyData.CastTo<InventoryEventArgs>();
        var cmd = data.InvCmd;
        switch (cmd)
        {
            case InvCmd.Flush:
                {
                    //刷新inventory的数据
                    InventoryPanel.SlotContext.UpdateDataSet(PRX_Inventory.InventoryDataSet);
                }
                break;
            case InvCmd.ShowDescription:
                {
                    InventoryPanel.TxtDescription.text = Convert.ToString(data.Msg);
                }
                break;
        }
    }
    public void Init(object viewEntity)
    {
        InventoryPanel = viewEntity.CastTo<InventoryPanel>();
        InventoryPanel.BtnLoad.onClick.AddListener(LoadClick);
        InventoryPanel.BtnQuit.onClick.AddListener(QuitClick);
        InventoryPanel.BtnSave.onClick.AddListener(SaveClick);
        InventoryPanel.BtnUpdate.onClick.AddListener(UpdateClick);
        InventoryPanel.SlotContext.SlotAsset = PRX_Inventory.SlotAsset;
    }
    /// <summary>
    /// 更新inventory的slot与item中的信息；
    /// </summary>
    public void UpdateSlotItem()
    {
        InventoryPanel.UpdateSlotItem(PRX_Inventory.InventoryDataSet);
    }
    public void UpdateSlots(InventoryDataset dataset)
    {
        InventoryPanel.UpdateSlotItem(dataset);
    }
    void QuitClick()
    {
        InventoryPanel.Active = false;
    }
    void LoadClick()
    {
        PRX_Inventory.LoadJson();
        InventoryPanel.UpdateSlotItem(PRX_Inventory.InventoryDataSet);
    }
    void UpdateClick()
    {
        InventoryPanel.UpdateSlotItem(PRX_Inventory.InventoryDataSet);
    }
    void SaveClick()
    {
        PRX_Inventory.SaveJson();
    }
}
