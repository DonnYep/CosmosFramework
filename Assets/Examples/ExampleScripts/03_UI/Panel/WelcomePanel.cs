using Cosmos.UI;
using UnityEngine.UI;
public class WelcomePanel : UGUIUIForm
{
    Text info;
    InputField inputMsg;
    void Awake()
    {
        GetUILable<Button>("BtnShowInfo").onClick.AddListener(ShowInfo);
        GetUILable<Button>("BtnHideInfo").onClick.AddListener(HideInfo);
        GetUILable<Button>("BtnQuit").onClick.AddListener(Quit);
        info = GetUILable<Image>("TxtInfo").transform.Find("Info").GetComponent<Text>();
        inputMsg = GetUILable<InputField>("InputMsg");
    }
    void ShowInfo()
    {
        info.enabled = true;
    }
    void HideInfo()
    {
        info.enabled = false;
    }
    void Quit()
    {
        Active = false;
    }
}
