using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;
using UnityEngine.UI;
using Cosmos;
[PrefabAsset("UI/WelcomePanel",PrefabName = "WelcomePanel")]
public class WelcomePanel : UILogicTemporary
{
    Text info;
    InputField inputMsg;
    protected override void OnInitialization()
    {
        GetUIForm<Button>("BtnShowInfo").onClick.AddListener(ShowInfo);
        GetUIForm<Button>("BtnHideInfo").onClick.AddListener(HideInfo);
        GetUIForm<Button>("BtnQuit").onClick.AddListener(Quit);
        info = GetUIForm<Image>("TxtInfo").transform.Find("Info").GetComponent<Text>();
        inputMsg = GetUIForm<InputField>("InputMsg");
    }
    protected override void OnTermination()
    {
        GetUIForm<Button>("BtnShowInfo").onClick.RemoveAllListeners();
        GetUIForm<Button>("BtnHideInfo").onClick.RemoveAllListeners();
        GetUIForm<Button>("BtnQuit").onClick.RemoveAllListeners();
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
        HidePanel();
    }
    public override void HidePanel()
    {
        GameManager.GetModule<IUIManager>().RemoveUI(UIName,out _ );
    }
}
