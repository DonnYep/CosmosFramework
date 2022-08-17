using UnityEngine.UI;
using Cosmos.UI;
using PureMVC;

public class NavigatePanel : UGUIUIForm
{
    public Button BtnWelcome { get; private set; }
    public Button BtnInventory { get; private set; }
    public Button BtnStore { get; private set; }
    public Button BtnStatus { get; private set; }
    public Button BtnSetting { get; private set; }
    public Button BtnOpGroup { get; private set; }
    void Awake()
    {
        BtnWelcome = GetUILabel<Button>("BtnWelcome");
        BtnInventory = GetUILabel<Button>("BtnInventory");
        BtnStore = GetUILabel<Button>("BtnStore");
        BtnStatus = GetUILabel<Button>("BtnStatus");
        BtnSetting = GetUILabel<Button>("BtnSetting");
        BtnOpGroup = GetUILabel<Button>("BtnOpGroup");
        MVC.RegisterMediator(new MED_Navigate(this));
    }
}
