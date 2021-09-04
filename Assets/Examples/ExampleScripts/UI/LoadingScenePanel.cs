using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos.UI;
using Cosmos;
using Cosmos.Scene;
/// <summary>
/// 显示进度的脚本
/// </summary>
public class LoadingScenePanel : UIForm
{
    Text txtProgress;
    Slider sldProgress;
    float currentProgress;

    protected override void Awake()
    {
        txtProgress = GetUILable<Text>("TxtProgress");
        sldProgress = GetUILable<Slider>("SldProgress");
        LoadLevel();
    }
    void Update()
    {
        var percent = currentProgress * 100;
        sldProgress.value = Mathf.Lerp(sldProgress.value, percent, Time.deltaTime);
        txtProgress.text = (int)sldProgress.value + "%";
    }
    void LoadLevel()
    {
        sldProgress.value = 0;
        Utility.Debug.LogInfo(LevelLoadInfo.TargetLevel,MessageColor.YELLOW);
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneInfo(LevelLoadInfo.TargetLevel), UpdateProgressValue);
    }
    void UpdateProgressValue(float value)
    {
        currentProgress = value;
    }
}
