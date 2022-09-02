using UnityEngine.UI;
using Cosmos.UI;
public class StatusPanel : UGUIUIForm
{
    void Awake()
    {
        GetUILabel<Button>("BtnQuit").onClick.AddListener(QuitClick);
        Order = 1;
    }
    void QuitClick()
    {
        Active = false;
    }
}
