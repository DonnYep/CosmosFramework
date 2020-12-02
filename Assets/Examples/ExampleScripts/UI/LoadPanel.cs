using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.UI;
using Cosmos.Scene;
/// <summary>
/// 按钮按下载入的脚本
/// 流程：载入loading场景，loading场景通过Utility.Globle.TargetLevel参数自动在初始化时候异步加载目标场景。
/// </summary>
public class LoadPanel : UILogicResident
{
    InputField inputTargetLevel;
    InputField inputLoadLevel;
    ISceneManager sceneManager;

    protected override void OnInitialization()
    {
        GetUIPanel<Button>("BtnLoad").onClick.AddListener(LoadClick);
        inputLoadLevel = GetUIPanel<InputField>("InputLoadLevel");
        inputTargetLevel = GetUIPanel<InputField>("InputTargetLevel");
        sceneManager = GameManager.GetModule<ISceneManager>();
    }
    protected override void OnTermination()
    {
        GetUIPanel<Button>("BtnLoad").onClick.RemoveAllListeners();
    }
    void LoadClick()
    {
       Utility.Global.TargetLevel = inputTargetLevel.text;
        string loadingLevel = inputLoadLevel.text;
        if (Utility.Text.IsNumeric(loadingLevel))
        {
            int index = int.Parse(loadingLevel);
            sceneManager.LoadSceneAsync(index,()=>Utility.Debug.LogInfo("Scene load Done"));
        }
        else
            sceneManager.LoadSceneAsync(loadingLevel, () => Utility.Debug.LogInfo("Scene load Done"));
    }
}
