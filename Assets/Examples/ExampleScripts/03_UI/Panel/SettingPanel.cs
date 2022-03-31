using UnityEngine.UI;
using Cosmos.UI;
[UIAsset(nameof(SettingPanel), "Example", "UI/SettingPanel" )]
public class SettingPanel : UIForm
{
    protected override void Awake()
    {
        GetUILable<Button>("BtnQuit").onClick.AddListener(QuitClick);
    }
    void QuitClick()
    {
        UIManager.DeactiveUIForm(UIFormName);
    }
}
