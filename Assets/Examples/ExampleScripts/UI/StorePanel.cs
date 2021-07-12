using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
[UIAsset(nameof(StorePanel), "Example", "UI/StorePanel" )]
public class StorePanel : UIForm
{
    protected override void OnInitialization()
    {
        GetUIPanel<Button>("BtnQuit").onClick.AddListener(QuitClick);
    }
    protected override void OnTermination()
    {
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
    void QuitClick()
    {
        UIManager.DeactiveUIForm(UIFormName);
    }
}
