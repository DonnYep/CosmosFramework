using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
using Cosmos.Event;
using Cosmos.Data;
namespace Cosmos.Mvvm
{
    /// <summary>
    /// 仅测试
    /// </summary>
    [UIAsset("UI/InventoryPanel")]
    public class InventoryPanel : UIResidentForm
    {
        LogicEventArgs<InventoryDataSet> uip;
        Text txtDescription;
        public void UpdataItem(string desc)
        {
            txtDescription.text = desc;
        }
        protected override void OnInitialization()
        {
            GetUIForm<Button>("BtnLoad").onClick.AddListener(LoadClick);
            GetUIForm<Button>("BtnQuit").onClick.AddListener(QuitClick);
            GetUIForm<Button>("BtnSave").onClick.AddListener(SaveClick);
            GetUIForm<Button>("BtnUpdate").onClick.AddListener(UpdateClick);
            txtDescription = GetUIForm<Text>("TxtDescription");
        }
        private void Start()
        {
            MVVM.Fire(UIEventDefine.UI_UPD_SLOT);
            //GameManager.GetModule<IEventManager>().DispatchEvent(UIEventDefine.UI_UPD_SLOT, null, Uip);
        }
        void LoadClick()
        {
            UpdateClick();
        }
        void SaveClick()
        {
            //Facade.SaveJsonDataToLocal("Inventory", "InventoryCache.json", inventoryDataSet);
            Utility.Debug.LogInfo("SaveJsonDataToLocal");
        }
        void QuitClick()
        {
            HideUIForm();
        }
        void UpdateClick()
        {
            //eventManager.DispatchEvent(UIEventDefine.UI_UPD_SLOT, this, Uip);
        }
    }
}