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
public class LoadPanel : UIResidentForm
{
    InputField inputTargetLevel;
    InputField inputLoadLevel;
    ISceneManager sceneManager;

    protected override void OnInitialization()
    {
        GetUIForm<Button>("BtnLoad").onClick.AddListener(LoadClick);
        inputLoadLevel = GetUIForm<InputField>("InputLoadLevel");
        inputTargetLevel = GetUIForm<InputField>("InputTargetLevel");
        sceneManager = GameManager.GetModule<ISceneManager>();
    }
    protected override void OnTermination()
    {
        GetUIForm<Button>("BtnLoad").onClick.RemoveAllListeners();
    }
    void LoadClick()
    {
        LevelLoadInfo.TargetLevel = inputTargetLevel.text;
        string loadingLevel = inputLoadLevel.text;
        sceneManager.LoadSceneAsync(new SceneInfo( loadingLevel), () => Utility.Debug.LogInfo("Scene load Done"));
    }
}
