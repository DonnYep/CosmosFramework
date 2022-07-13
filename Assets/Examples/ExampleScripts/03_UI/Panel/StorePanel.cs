using UnityEngine.UI;
using Cosmos.UI;
public class StorePanel : UGUIUIForm
{
    void Awake()
    {
        GetUILable<Button>("BtnQuit").onClick.AddListener(QuitClick);
    }
    void QuitClick()
    {
        Active = false;
    }
}
