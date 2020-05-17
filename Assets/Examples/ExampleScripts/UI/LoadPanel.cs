using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
/// <summary>
/// 按钮按下载入的脚本
/// 流程：载入loading场景，loading场景通过Utility.Globle.TargetLevel参数自动在初始化时候异步加载目标场景。
/// </summary>
public class LoadPanel : UILogicResident
{
    InputField inputTargetLevel;
    InputField inputLoadLevel;
    protected override void OnInitialization()
    {
        GetUIPanel<Button>("BtnLoad").onClick.AddListener(LoadClick);
        inputLoadLevel = GetUIPanel<InputField>("InputLoadLevel");
        inputTargetLevel = GetUIPanel<InputField>("InputTargetLevel");
    }
    protected override void OnTermination()
    {
        GetUIPanel<Button>("BtnLoad").onClick.RemoveAllListeners();
    }
    void LoadClick()
    {
       Utility.Globle.TargetLevel = inputTargetLevel.text;
        string loadingLevel = inputLoadLevel.text;
        if (Utility.Text.IsNumeric(loadingLevel))
        {
            int index = int.Parse(loadingLevel);
            Facade.LoadSceneAsync(index);
        }
        else
            Facade.LoadSceneAsync(loadingLevel);
    }
}
