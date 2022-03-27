using UnityEngine.UI;
using Cosmos.UI;
using PureMVC;

public class NavigatePanel : UIForm
{
    public Button BtnWelcome { get; private set; }
    public Button BtnInventory { get; private set; }
    public Button BtnStore { get; private set; }
    public Button BtnStatus { get; private set; }
    public Button BtnSetting { get; private set; }
    public Button BtnOpGroup { get; private set; }
    protected override void Awake()
    {
        BtnWelcome = GetUILable<Button>("BtnWelcome");
        BtnInventory = GetUILable<Button>("BtnInventory");
        BtnStore = GetUILable<Button>("BtnStore");
        BtnStatus = GetUILable<Button>("BtnStatus");
        BtnSetting = GetUILable<Button>("BtnSetting");
        BtnOpGroup = GetUILable<Button>("BtnOpGroup");
        MVC.RegisterMediator(new MED_Navigate(this));
    }
}
