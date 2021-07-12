using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
using Cosmos.Mvvm;
namespace Cosmos.Test
{
    public class NavigatePanel : UIForm
    {
        WelcomePanel welcome;
        InventoryPanel inventory;
        SettingPanel setting;
        StorePanel store;
        StatusPanel status;
        bool state;
        protected override void OnInitialization()
        {
            GetUIPanel<Button>("BtnWelcome").onClick.AddListener(WelcomeClick);
            GetUIPanel<Button>("BtnInventory").onClick.AddListener(InventoryClick);
            GetUIPanel<Button>("BtnStore").onClick.AddListener(StoreClick);
            GetUIPanel<Button>("BtnStatus").onClick.AddListener(StatusClick);
            GetUIPanel<Button>("BtnSetting").onClick.AddListener(SettingClick);
            GetUIPanel<Button>("BtnOpGroup").onClick.AddListener(OperateGroup);
        }
        /// <summary>
        /// welcome panel 是临时类型，因此当panel存在时，点击移除，不存在时则载入
        /// </summary>
        void WelcomeClick()
        {
            if (welcome == null)
            {
                UIManager.OpenUIFormAsync<WelcomePanel>(panel =>
                { panel.gameObject.SetActive(true); welcome = panel; });
                return;
            }
            if (welcome.gameObject.activeSelf)
                UIManager.DeactiveUIForm(welcome.UIFormName);
            else
                UIManager.ActiveUIForm(welcome.UIFormName);
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
                UIManager.DeactiveUIForm(inventory.UIFormName);
            else
                UIManager.ActiveUIForm(inventory.UIFormName);
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
                UIManager.DeactiveUIForm(store.UIFormName);
            else
                UIManager.ActiveUIForm(store.UIFormName);
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
                UIManager.DeactiveUIForm(setting.UIFormName);
            else
                UIManager.ActiveUIForm(setting.UIFormName);
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
                UIManager.DeactiveUIForm(status.UIFormName);
            else
                UIManager.ActiveUIForm(status.UIFormName);
        }
        void OperateGroup()
        {
            if (state)
                UIManager.ActiveUIGroup("Example");
            else
                UIManager.DeactiveUIGroup("Example");
            state = !state;
        }
    }
}