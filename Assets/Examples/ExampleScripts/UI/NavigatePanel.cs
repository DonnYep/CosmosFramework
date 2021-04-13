﻿using System.Collections;
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
    protected override void OnInitialization()
    {
        GetUIPanel<Button>("BtnWelcome").onClick.AddListener(WelcomeClick);
        GetUIPanel<Button>("BtnInventory").onClick.AddListener(InventoryClick);
        GetUIPanel<Button>("BtnStore").onClick.AddListener(StoreClick);
        GetUIPanel<Button>("BtnStatus").onClick.AddListener(StatusClick);
        GetUIPanel<Button>("BtnSetting").onClick.AddListener(SettingClick);
    }
    protected override void OnTermination()
    {
        GetUIPanel<Button>("BtnWelcome").onClick.RemoveAllListeners();
        GetUIPanel<Button>("BtnInventory").onClick.RemoveAllListeners();
        GetUIPanel<Button>("BtnStore").onClick.RemoveAllListeners();
        GetUIPanel<Button>("BtnStatus").onClick.RemoveAllListeners();
        GetUIPanel<Button>("BtnSetting").onClick.RemoveAllListeners();
    }
    /// <summary>
    /// welcome panel 是临时类型，因此当panel存在时，点击移除，不存在时则载入
    /// </summary>
    void WelcomeClick()
    {
        if (welcome == null)
            Facade.ShowPanel<WelcomePanel>(panel =>
            { panel.gameObject.SetActive(true); welcome = panel; });
        else
            Facade.RemovePanel<WelcomePanel>();
    }
    /// <summary>
    /// Invenmtory panel是常驻类型，若不存在，则载入；开启与关闭只进行显示与隐藏操作
    /// </summary>
    void InventoryClick()
    {
        if (inventory == null)
        {
            Facade.ShowPanel<InventoryPanel>(Utility.UI.GetUIFullRelativePath("InventoryPanel"), panel =>
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
            Facade.ShowPanel<StorePanel>(panel =>
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
            Facade.ShowPanel<SettingPanel>(panel => 
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
            Facade.ShowPanel<StatusPanel>(panel => 
            { panel.gameObject.SetActive(true);status = panel; });
            return;
        }
        if (status.gameObject.activeSelf)
            status.HidePanel();
        else
            status.ShowPanel();
    }

}