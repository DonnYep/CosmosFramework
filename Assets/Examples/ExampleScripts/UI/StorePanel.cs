using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
[UIAsset(nameof(StorePanel), "Example", "UI/StorePanel")]
public class StorePanel : UIForm
{
    protected override void Awake()
    {
        GetUILable<Button>("BtnQuit").onClick.AddListener(QuitClick);
    }
    void QuitClick()
    {
        UIManager.DeactiveUIForm(UIFormName);
    }
}
