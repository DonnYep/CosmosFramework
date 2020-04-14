using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;
using UnityEngine.UI;
using Cosmos;
public class WelcomePanel : UILogicTemporary
{
    Text info;
    InputField inputMsg;
    protected override void OnInitialization()
    {
        GetUIComp<Button>("BtnShowInfo").onClick.AddListener(ShowInfo);
        GetUIComp<Button>("BtnHideInfo").onClick.AddListener(HideInfo);
        GetUIComp<Button>("BtnQuit").onClick.AddListener(Quit);
        info = GetUIComp<Image>("TxtInfo").transform.Find("Info").GetComponent<Text>();
        inputMsg = GetUIComp<InputField>("InputMsg");
    }
    protected override void OnTermination()
    {
        GetUIComp<Button>("BtnShowInfo").onClick.RemoveAllListeners();
        GetUIComp<Button>("BtnHideInfo").onClick.RemoveAllListeners();
        GetUIComp<Button>("BtnQuit").onClick.RemoveAllListeners();
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
        Facade.Instance.RemovePanel(Utility.UI.GetUIFullRelativePath("WelcomePanel"));
    }
}
