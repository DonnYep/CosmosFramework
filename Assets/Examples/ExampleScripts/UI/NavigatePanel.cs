using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
public class NavigatePanel : UILogicResident
{
    WelcomePanel welcome;
    InventoryPanel inventory;
    SettingPanel setting;
    StorePanel store;
    StatusPanel status;
    IUIManager uiManager;
    protected override void OnInitialization()
    {
        uiManager = GameManager.GetModule<IUIManager>();
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
            uiManager.ShowPanel<WelcomePanel>(panel =>
            { panel.gameObject.SetActive(true); welcome = panel; });
        else
            uiManager.RemovePanel<WelcomePanel>();
    }
    /// <summary>
    /// Invenmtory panel是常驻类型，若不存在，则载入；开启与关闭只进行显示与隐藏操作
    /// </summary>
    void InventoryClick()
    {
        if (inventory == null)
        {
            uiManager.ShowPanel<InventoryPanel>(panel =>
            { panel.gameObject.name = "InventoryPanel"; panel.gameObject.SetActive(true); inventory = panel; });
            return;
        }
        if (inventory.gameObject.activeSelf)
            inventory.HidePanel();
        else
            inventory.ShowPanel();
    }
    void StoreClick()
    {
        if (store == null)
        {
            uiManager.ShowPanel<StorePanel>(panel =>
            { panel.gameObject.SetActive(true); store = panel; });
            return;
        }
        if (store.gameObject.activeSelf)
            store.HidePanel();
        else
           store.ShowPanel();
    }
    void SettingClick()
    {
        if (setting == null)
        {
            uiManager.ShowPanel<SettingPanel>(panel => 
            { panel.gameObject.SetActive(true);setting = panel; });
            return;
        }
        if (setting.gameObject.activeSelf)
            setting.HidePanel();
        else
            setting.ShowPanel();
    }
    void StatusClick()
    {
        if (status == null)
        {
            uiManager.ShowPanel<StatusPanel>(panel => 
            { panel.gameObject.SetActive(true);status = panel; });
            return;
        }
        if (status.gameObject.activeSelf)
            status.HidePanel();
        else
            status.ShowPanel();
    }

}
