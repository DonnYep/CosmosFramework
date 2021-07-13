using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;
using UnityEngine.UI;
using Cosmos;
[UIAsset(nameof(WelcomePanel), "Example", "UI/WelcomePanel" )]
public class WelcomePanel : UIForm
{
    Text info;
    InputField inputMsg;
    protected override void Awake()
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
        UIManager.ReleaseUIForm(UIFormName);
    }
}
