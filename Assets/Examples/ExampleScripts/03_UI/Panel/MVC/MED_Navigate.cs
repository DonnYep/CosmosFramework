using PureMVC;
using Cosmos.UI;
using Cosmos;
public class MED_Navigate : Mediator
{
    public new const string NAME = "MED_Navigate";

    WelcomePanel welcome;
    InventoryPanel inventory;
    SettingPanel setting;
    StorePanel store;
    StatusPanel status;
    bool state;

    public MED_Navigate(object viewEntity) : base(NAME, viewEntity) { }
    NavigatePanel NavigatePanel { get { return ViewEntity as NavigatePanel; } }
    public override void OnRegister()
    {
        NavigatePanel.BtnWelcome.onClick.AddListener(WelcomeClick);
        NavigatePanel.BtnInventory.onClick.AddListener(InventoryClick);
        NavigatePanel.BtnStore.onClick.AddListener(StoreClick);
        NavigatePanel.BtnStatus.onClick.AddListener(StatusClick);
        NavigatePanel.BtnSetting.onClick.AddListener(SettingClick);
        NavigatePanel.BtnOpGroup.onClick.AddListener(OperateGroup);
    }
    void WelcomeClick()
    {
        if (welcome == null)
        {
            CosmosEntry.UIManager.OpenUIFormAsync<WelcomePanel>(new UIAssetInfo("UI/WelcomePanel", "WelcomePanel", "Example"),panel =>
            { panel.gameObject.SetActive(true); welcome = panel; });
            return;
        }
        if (welcome.gameObject.activeSelf)
            CosmosEntry.UIManager.DeactiveUIForm(welcome.UIFormName);
        else
            CosmosEntry.UIManager.ActiveUIForm(welcome.UIFormName);
    }
    void InventoryClick()
    {
        if (inventory == null)
        {
            CosmosEntry.UIManager.OpenUIFormAsync<InventoryPanel>(new UIAssetInfo("UI/InventoryPanel", "InventoryPanel", "Example"),panel =>
           { panel.gameObject.name = "InventoryPanel"; panel.gameObject.SetActive(true); inventory = panel; });
            return;
        }
        if (inventory.gameObject.activeSelf)
            CosmosEntry.UIManager.DeactiveUIForm(inventory.UIFormName);
        else
            CosmosEntry.UIManager.ActiveUIForm(inventory.UIFormName);
    }
    void StoreClick()
    {
        if (store == null)
        {
            CosmosEntry.UIManager.OpenUIFormAsync<StorePanel>(new UIAssetInfo("UI/StorePanel", "StorePanel", "Example"),panel =>
            { panel.gameObject.SetActive(true); store = panel; });
            return;
        }
        if (store.gameObject.activeSelf)
            CosmosEntry.UIManager.DeactiveUIForm(store.UIFormName);
        else
            CosmosEntry.UIManager.ActiveUIForm(store.UIFormName);
    }
    void SettingClick()
    {
        if (setting == null)
        {
            CosmosEntry.UIManager.OpenUIFormAsync<SettingPanel>(new UIAssetInfo("UI/SettingPanel", "SettingPanel", "Example"),panel =>
            { panel.gameObject.SetActive(true); setting = panel; });
            return;
        }
        if (setting.gameObject.activeSelf)
            CosmosEntry.UIManager.DeactiveUIForm(setting.UIFormName);
        else
            CosmosEntry.UIManager.ActiveUIForm(setting.UIFormName);
    }
    void StatusClick()
    {
        if (status == null)
        {
            CosmosEntry.UIManager.OpenUIFormAsync<StatusPanel>(new UIAssetInfo("UI/StatusPanel", "StatusPanel", "Example"),panel =>
            { panel.gameObject.SetActive(true); status = panel; });
            return;
        }
        if (status.gameObject.activeSelf)
            CosmosEntry.UIManager.DeactiveUIForm(status.UIFormName);
        else
            CosmosEntry.UIManager.ActiveUIForm(status.UIFormName);
    }
    void OperateGroup()
    {
        if (state)
            CosmosEntry.UIManager.ActiveUIGroup("Example");
        else
            CosmosEntry.UIManager.DeactiveUIGroup("Example");
        state = !state;
    }
}
