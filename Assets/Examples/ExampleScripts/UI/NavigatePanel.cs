using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
public class NavigatePanel : UILogicBase
{
    WelcomePanel welcome;
    InventoryPanel inventory;
    SettingPanel setting;
    StorePanel store;
    StatusPanel status;
    protected override void OnInitialization()
    {
        GetUIComp<Button>("BtnWelcome").onClick.AddListener(WelcomeClick);
        GetUIComp<Button>("BtnInventory").onClick.AddListener(InventoryClick);
        GetUIComp<Button>("BtnStore").onClick.AddListener(StoreClick);
        GetUIComp<Button>("BtnStatus").onClick.AddListener(StatusClick);
        GetUIComp<Button>("BtnSetting").onClick.AddListener(SettingClick);
    }
    protected override void OnTermination()
    {
        GetUIComp<Button>("BtnWelcome").onClick.RemoveAllListeners();
        GetUIComp<Button>("BtnInventory").onClick.RemoveAllListeners();
        GetUIComp<Button>("BtnStore").onClick.RemoveAllListeners();
        GetUIComp<Button>("BtnStatus").onClick.RemoveAllListeners();
        GetUIComp<Button>("BtnSetting").onClick.RemoveAllListeners();
    }
    void WelcomeClick()
    {
        if (welcome == null)
            Facade.Instance.ShowPanel<WelcomePanel>(Utility.UI.GetUIFullRelativePath( "WelcomePanel")
                , panel => { panel.gameObject.name = "WelcomePanel"; panel.gameObject.SetActive(true); welcome = panel; });
        else
            Facade.Instance.RemovePanel(Utility.UI.GetUIFullRelativePath("WelcomePanel"));
    }
    void InventoryClick()
    {
        if (inventory == null)
        {
            Facade.Instance.ShowPanel<InventoryPanel>(Utility.UI.GetUIFullRelativePath("InventoryPanel"), panel =>
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
            Facade.Instance.ShowPanel<StorePanel>(Utility.UI.GetUIFullRelativePath("StorePanel"), panel =>
            { panel.gameObject.name = "StorePanel"; panel.gameObject.SetActive(true); store = panel; });
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
            Facade.Instance.ShowPanel<SettingPanel>(Utility.UI.GetUIFullRelativePath("SettingPanel"), panel => 
            { panel.gameObject.name = "SettingPanel";panel.gameObject.SetActive(true);setting = panel; });
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
            Facade.Instance.ShowPanel<StatusPanel>(Utility.UI.GetUIFullRelativePath("StatusPanel"), panel => 
            { panel.gameObject.name = "StatusPanel";panel.gameObject.SetActive(true);status = panel; });
            return;
        }
        if (status.gameObject.activeSelf)
            status.HidePanel();
        else
            status.ShowPanel();
    }

}
