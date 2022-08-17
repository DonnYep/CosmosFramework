using UnityEngine.UI;
using Cosmos.UI;
public class SettingPanel : UGUIUIForm
{
    void Awake()
    {
        GetUILabel<Button>("BtnQuit").onClick.AddListener(QuitClick);
    }
    void QuitClick()
    {
        //UIManager.DeactiveUIForm(UIFormName);
        Active = false;
    }
}
