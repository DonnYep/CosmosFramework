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
public class LoadPanel : UIForm
{
    InputField inputTargetLevel;
    InputField inputLoadLevel;
    protected override void Awake()
    {
        GetUILable<Button>("BtnLoad").onClick.AddListener(LoadClick);
        inputLoadLevel = GetUILable<InputField>("InputLoadLevel");
        inputTargetLevel = GetUILable<InputField>("InputTargetLevel");
    }
    void LoadClick()
    {
        LevelLoadInfo.TargetLevel = inputTargetLevel.text;
        string loadingLevel = inputLoadLevel.text;
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneInfo( loadingLevel), () => Utility.Debug.LogInfo("Scene load Done"));
    }
}
