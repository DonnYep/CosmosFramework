using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
namespace Cosmos
{
    /// <summary>
    /// 仅测试
    /// </summary>
    public class InventoryPanel : UILogicResident
    {
        [SerializeField] InventoryDataSet inventoryDataSet;
        public InventoryDataSet InventoryDataSet { get { return inventoryDataSet; } set { inventoryDataSet = value; } }
        LogicEventArgs<InventoryDataSet> uip;
        public LogicEventArgs<InventoryDataSet> Uip
        {
            get
            {
                if (uip == null)
                    uip = new LogicEventArgs<InventoryDataSet>(InventoryDataSet);
                return uip;
            }
        }
        Text txtDescription;
        protected override void OnInitialization()
        {
            GetUIComp<Button>("BtnLoad").onClick.AddListener(LoadClick);
            GetUIComp<Button>("BtnQuit").onClick.AddListener(QuitClick);
            GetUIComp<Button>("BtnSave").onClick.AddListener(SaveClick);
            GetUIComp<Button>("BtnUpdate").onClick.AddListener(UpdateClick);
            txtDescription = GetUIComp<Text>("TxtDescription");
            AddUIEventListener(UIImplementParam.UIIMPLEMENT_ITEMDESCRIPTION, UpdateItemHandler);
        }
        protected override void OnTermination()
        {
            GetUIComp<Button>("BtnLoad").onClick.RemoveAllListeners();
            GetUIComp<Button>("BtnQuit").onClick.RemoveAllListeners();
            GetUIComp<Button>("BtnSave").onClick.RemoveAllListeners();
            GetUIComp<Button>("BtnUpdate").onClick.RemoveAllListeners();
            RemoveUIEventListener(UIImplementParam.UIIMPLEMENT_ITEMDESCRIPTION, UpdateItemHandler);
        }
        private void Start()
        {
            DispatchUIEvent(UIImplementParam.UIIMPLEMENT_UPDATESLOT, null, Uip);
        }
        void LoadClick()
        {

            Facade.Instance.LoadJsonDataFromLocal("Inventory", "InventoryCache.json", ref inventoryDataSet);
            Utility.DebugLog("LoadJsonDataFromLocal");
            UpdateClick();
        }
        void SaveClick()
        {
            Facade.Instance.SaveJsonDataToLocal("Inventory", "InventoryCache.json", inventoryDataSet);
            Utility.DebugLog("SaveJsonDataToLocal");
        }
        void QuitClick()
        {
            HidePanel();
        }
        void UpdateClick()
        {
            DispatchUIEvent(UIImplementParam.UIIMPLEMENT_UPDATESLOT, null, Uip);
        }
        void UpdateItemHandler(object sender,GameEventArgs args)
        {
            var stringUip=args as LogicEventArgs<string>;
            txtDescription.text = stringUip.Data;
        }
    }
}