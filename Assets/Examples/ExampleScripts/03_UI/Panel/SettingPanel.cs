using UnityEngine.UI;
using Cosmos.UI;
public class SettingPanel : UGUIUIForm
{
    void Awake()
    {
        GetUILable<Button>("BtnQuit").onClick.AddListener(QuitClick);
    }
    void QuitClick()
    {
        //UIManager.DeactiveUIForm(UIFormName);
        Active = false;
    }
}
