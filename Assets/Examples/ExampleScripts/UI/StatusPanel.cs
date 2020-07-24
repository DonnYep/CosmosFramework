using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
[PrefabUnit("UI/StatusPanel",PrefabName = "StatusPanel")]
public class StatusPanel : UILogicResident
{
    protected override void OnInitialization()
    {
        GetUIPanel<Button>("BtnQuit").onClick.AddListener(QuitClick);
    }
    protected override void OnTermination()
    {
        GetUIPanel<Button>("BtnQuit").onClick.RemoveAllListeners();
    }
    void QuitClick()
    {
        HidePanel();
    }
}
