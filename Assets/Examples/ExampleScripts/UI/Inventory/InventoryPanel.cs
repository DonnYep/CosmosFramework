using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos.Mvvm;
using Cosmos.UI;
namespace Cosmos.Test
{
    /// <summary>
    /// 仅测试
    /// </summary>
    [UIAsset(nameof(InventoryPanel), "Example","UI/InventoryPanel")]
    public class InventoryPanel : UIForm
    {
        Text txtDescription;
        MED_Inventory med_Inventory;
        SlotContext slotContext;
        public SlotContext SlotContext { get { return slotContext; } }
        public void UpdataItemDescription(string desc)
        {
            txtDescription.text = desc;
        }
        public void UpdateSlotItem(InventoryDataSet invDataSet)
        {
            SlotContext.UpdateSlot(invDataSet);
            SlotContext.UpdateItem(invDataSet);
        }
        protected override void OnInitialization()
        {
            GetUIPanel<Button>("BtnLoad").onClick.AddListener(LoadClick);
            GetUIPanel<Button>("BtnQuit").onClick.AddListener(QuitClick);
            GetUIPanel<Button>("BtnSave").onClick.AddListener(SaveClick);
            GetUIPanel<Button>("BtnUpdate").onClick.AddListener(UpdateClick);

            MVVM.Dispatch(MVVMDefine.CMD_Inventory);
            slotContext = gameObject.GetComponentInChildren<SlotContext>();
            txtDescription = GetUIPanel<Text>("TxtDescription");
        }
        protected override void OnDeactive()
        {
            Utility.Debug.LogInfo($"{UIFormName} OnHide");
        }
        protected override void OnActive()
        {
            Utility.Debug.LogInfo($"{UIFormName} OnShow");
        }
        private void Start()
        {
            med_Inventory= MVVM.PeekMediator<MED_Inventory>(MVVMDefine.MED_Inventory);
            med_Inventory.UpdateSlotItem();
        }
        void LoadClick()
        {
            med_Inventory.LoadInventoryJson();
        }
        void SaveClick()
        {
            med_Inventory.SaveInventoryJson();
        }
        void QuitClick()
        {
            UIManager.DeactiveUIForm(UIFormName);
        }
        void UpdateClick()
        {
            med_Inventory.UpdateSlotItem();
        }
    }
}