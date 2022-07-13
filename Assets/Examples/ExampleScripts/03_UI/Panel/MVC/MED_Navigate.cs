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
            CosmosEntry.UIManager.OpenUIFormAsync<WelcomePanel>(new UIAssetInfo("UI/WelcomePanel", "WelcomePanel", "Example"), panel =>
            { panel.gameObject.SetActive(true); welcome = panel; });
            return;
        }
        welcome.Active = !welcome.Active;
    }
    void InventoryClick()
    {
        if (inventory == null)
        {
            CosmosEntry.UIManager.OpenUIFormAsync<InventoryPanel>(new UIAssetInfo("UI/InventoryPanel", "InventoryPanel", "Example"), panel =>
           { panel.gameObject.name = "InventoryPanel"; panel.gameObject.SetActive(true); inventory = panel; });
            return;
        }
        inventory.Active = !inventory.Active;
    }
    void StoreClick()
    {
        if (store == null)
        {
            CosmosEntry.UIManager.OpenUIFormAsync<StorePanel>(new UIAssetInfo("UI/StorePanel", "StorePanel", "Example"), panel =>
            { panel.gameObject.SetActive(true); store = panel; });
            return;
        }
        store.Active = !store.Active;
    }
    void SettingClick()
    {
        if (setting == null)
        {
            CosmosEntry.UIManager.OpenUIFormAsync<SettingPanel>(new UIAssetInfo("UI/SettingPanel", "SettingPanel", "Example"), panel =>
            { panel.gameObject.SetActive(true); setting = panel; });
            return;
        }
        setting.Active = !setting.Active;
    }
    void StatusClick()
    {
        if (status == null)
        {
            CosmosEntry.UIManager.OpenUIFormAsync<StatusPanel>(new UIAssetInfo("UI/StatusPanel", "StatusPanel", "Example"), panel =>
            { panel.gameObject.SetActive(true); status = panel; });
            return;
        }
        status.Active = !status.Active;
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
