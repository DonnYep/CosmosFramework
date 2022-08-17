using Cosmos.UI;
using UnityEngine.UI;
public class WelcomePanel : UGUIUIForm
{
    Text info;
    InputField inputMsg;
    void Awake()
    {
        GetUILabel<Button>("BtnShowInfo").onClick.AddListener(ShowInfo);
        GetUILabel<Button>("BtnHideInfo").onClick.AddListener(HideInfo);
        GetUILabel<Button>("BtnQuit").onClick.AddListener(Quit);
        info = GetUILabel<Image>("TxtInfo").transform.Find("Info").GetComponent<Text>();
        inputMsg = GetUILabel<InputField>("InputMsg");
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
