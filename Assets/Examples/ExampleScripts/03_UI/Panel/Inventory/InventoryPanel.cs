using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PureMVC;
using Cosmos.UI;
/// <summary>
/// 仅测试
/// </summary>
[UIAsset(nameof(InventoryPanel), "Example", "UI/InventoryPanel")]
public class InventoryPanel : UIForm
{
    MED_Inventory med_Inventory;
    public Button BtnLoad { get; private set; }
    public Button BtnQuit { get; private set; }
    public Button BtnSave { get; private set; }
    public Button BtnUpdate { get; private set; }
    public Text TxtDescription { get; private set; }
    public SlotContext SlotContext { get; private set; }
    public void UpdateSlotItem(InventoryDataset invDataSet)
    {
        SlotContext.UpdateSlot(invDataSet);
        SlotContext.UpdateItem(invDataSet);
    }
    protected override void Awake()
    {
        BtnLoad = GetUILable<Button>("BtnLoad");
        BtnQuit = GetUILable<Button>("BtnQuit");
        BtnSave = GetUILable<Button>("BtnSave");
        BtnUpdate = GetUILable<Button>("BtnUpdate");
        TxtDescription = GetUILable<Text>("TxtDescription");
        SlotContext = gameObject.GetComponentInChildren<SlotContext>();
    }
    private void Start()
    {
        med_Inventory = MVC.PeekMediator(MED_Inventory.NAME) as MED_Inventory;
        med_Inventory.Init(this);
        med_Inventory.UpdateSlotItem();
    }
}
