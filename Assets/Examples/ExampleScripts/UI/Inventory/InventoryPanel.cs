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
            GetUIPanel<Button>("BtnLoad").onClick.AddListener(LoadClick);
            GetUIPanel<Button>("BtnQuit").onClick.AddListener(QuitClick);
            GetUIPanel<Button>("BtnSave").onClick.AddListener(SaveClick);
            GetUIPanel<Button>("BtnUpdate").onClick.AddListener(UpdateClick);
            txtDescription = GetUIPanel<Text>("TxtDescription");
            Facade.AddEventListener(UIIEventDefine.UI_IMPL_ITEM_DESC, UpdateItemHandler);
        }
        protected override void OnTermination()
        {
            GetUIPanel<Button>("BtnLoad").onClick.RemoveAllListeners();
            GetUIPanel<Button>("BtnQuit").onClick.RemoveAllListeners();
            GetUIPanel<Button>("BtnSave").onClick.RemoveAllListeners();
            GetUIPanel<Button>("BtnUpdate").onClick.RemoveAllListeners();
            Facade.RemoveEventListener(UIIEventDefine.UI_IMPL_ITEM_DESC, UpdateItemHandler);
        }
        private void Start()
        {
           Facade.DispatchEvent(UIIEventDefine.UI_IMPL_UPD_SLOT, null, Uip);
        }
        void LoadClick()
        {
            //这里需要注意，Unity提供的JsonUtility.FromJsonOverwrite方法，官方对这一方法的文档为：
            //it must be a MonoBehaviour, ScriptableObject, or plain class/struct with the Serializable attribute applied

            string json = Facade.LoadJsonDataFromLocal("Inventory", "InventoryCache.json");
            JsonUtility.FromJsonOverwrite(json, inventoryDataSet);
            Utility.Debug.LogInfo("LoadJsonDataFromLocal");
            UpdateClick();
        }
        void SaveClick()
        {
            Facade.SaveJsonDataToLocal("Inventory", "InventoryCache.json", inventoryDataSet);
            Utility.Debug.LogInfo("SaveJsonDataToLocal");
        }
        void QuitClick()
        {
            HidePanel();
        }
        void UpdateClick()
        {
           Facade. DispatchEvent(UIIEventDefine.UI_IMPL_UPD_SLOT, this, Uip);
        }
        void UpdateItemHandler(object sender,GameEventArgs args)
        {
            var stringUip=args as LogicEventArgs<string>;
            txtDescription.text = stringUip.Data;
        }
    }
}