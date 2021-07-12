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
    protected override void OnDeactive()
    {
        Utility.Debug.LogInfo($"{UIFormName} OnDeactive");
    }
    protected override void OnActive()
    {
        Utility.Debug.LogInfo($"{UIFormName} OnActive");
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
