using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
[UIAsset(nameof(StatusPanel), "Example", "UI/StatusPanel")]
public class StatusPanel : UIForm
{
    protected override void OnInitialization()
    {
        GetUIPanel<Button>("BtnQuit").onClick.AddListener(QuitClick);
    }
    protected override void OnTermination()
    {
        GetUIPanel<Button>("BtnQuit").onClick.RemoveAllListeners();
    }
    protected override void OnHide()
    {
        Utility.Debug.LogInfo($"{UIFormName} OnHide");
    }
    protected override void OnShow()
    {
        Utility.Debug.LogInfo($"{UIFormName} OnShow");
    }
    void QuitClick()
    {
        UIManager.HideUIForm(UIFormName);
    }
}
