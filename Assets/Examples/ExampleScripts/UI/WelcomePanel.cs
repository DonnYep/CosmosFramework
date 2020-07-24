using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;
using UnityEngine.UI;
using Cosmos;
[PrefabUnit("UI/WelcomePanel",PrefabName = "WelcomePanel")]
public class WelcomePanel : UILogicTemporary
{
    Text info;
    InputField inputMsg;
    protected override void OnInitialization()
    {
        GetUIPanel<Button>("BtnShowInfo").onClick.AddListener(ShowInfo);
        GetUIPanel<Button>("BtnHideInfo").onClick.AddListener(HideInfo);
        GetUIPanel<Button>("BtnQuit").onClick.AddListener(Quit);
        info = GetUIPanel<Image>("TxtInfo").transform.Find("Info").GetComponent<Text>();
        inputMsg = GetUIPanel<InputField>("InputMsg");
    }
    protected override void OnTermination()
    {
        GetUIPanel<Button>("BtnShowInfo").onClick.RemoveAllListeners();
        GetUIPanel<Button>("BtnHideInfo").onClick.RemoveAllListeners();
        GetUIPanel<Button>("BtnQuit").onClick.RemoveAllListeners();
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
        Facade.RemovePanel<WelcomePanel>();
    }
}
