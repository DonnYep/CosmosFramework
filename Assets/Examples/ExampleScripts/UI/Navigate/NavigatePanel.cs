using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
using Cosmos.Mvvm;
namespace Cosmos.Test
{
    public class NavigatePanel : UIResidentForm
    {
        WelcomePanel welcome;
        InventoryPanel inventory;
        SettingPanel setting;
        StorePanel store;
        StatusPanel status;
        protected override void OnInitialization()
        {
            GetUIForm<Button>("BtnWelcome").onClick.AddListener(WelcomeClick);
            GetUIForm<Button>("BtnInventory").onClick.AddListener(InventoryClick);
            GetUIForm<Button>("BtnStore").onClick.AddListener(StoreClick);
            GetUIForm<Button>("BtnStatus").onClick.AddListener(StatusClick);
            GetUIForm<Button>("BtnSetting").onClick.AddListener(SettingClick);
        }
        /// <summary>
        /// welcome panel 是临时类型，因此当panel存在时，点击移除，不存在时则载入
        /// </summary>
        void WelcomeClick()
        {
            if (welcome == null)
                UIManager.OpenUIFormAsync<WelcomePanel>(panel =>
                { panel.gameObject.SetActive(true); welcome = panel; });
            else
                welcome.HideUIForm();
        }
        /// <summary>
        /// Invenmtory panel是常驻类型，若不存在，则载入；开启与关闭只进行显示与隐藏操作
        /// </summary>
        void InventoryClick()
        {
            if (inventory == null)
            {
                UIManager.OpenUIFormAsync<InventoryPanel>(panel =>
                { panel.gameObject.name = "InventoryPanel"; panel.gameObject.SetActive(true); inventory = panel; });
                return;
            }
            if (inventory.gameObject.activeSelf)
                inventory.HideUIForm();
            else
                inventory.ShowUIForm();
        }
        void StoreClick()
        {
            if (store == null)
            {
                UIManager.OpenUIFormAsync<StorePanel>(panel =>
                { panel.gameObject.SetActive(true); store = panel; });
                return;
            }
            if (store.gameObject.activeSelf)
                store.HideUIForm();
            else
                store.ShowUIForm();
        }
        void SettingClick()
        {
            if (setting == null)
            {
                UIManager.OpenUIFormAsync<SettingPanel>(panel =>
                { panel.gameObject.SetActive(true); setting = panel; });
                return;
            }
            if (setting.gameObject.activeSelf)
                setting.HideUIForm();
            else
                setting.ShowUIForm();
        }
        void StatusClick()
        {
            if (status == null)
            {
                UIManager.OpenUIFormAsync<StatusPanel>(panel =>
                { panel.gameObject.SetActive(true); status = panel; });
                return;
            }
            if (status.gameObject.activeSelf)
                status.HideUIForm();
            else
                status.ShowUIForm();
        }
    }
}