using UnityEngine.UI;
using Cosmos.UI;
[UIAsset(nameof(StatusPanel), "Example", "UI/StatusPanel")]
public class StatusPanel : UIForm
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
