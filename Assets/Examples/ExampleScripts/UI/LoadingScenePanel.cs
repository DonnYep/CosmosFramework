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
    protected override void Awake()
    {
        txtProgress = GetUILable<Text>("TxtProgress");
        sldProgress = GetUILable<Slider>("SldProgress");
        LoadLevel();
    }
    void LoadLevel()
    {
        sldProgress.value = 0;
        Utility.Debug.LogInfo(LevelLoadInfo.TargetLevel,MessageColor.YELLOW);
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneInfo(LevelLoadInfo.TargetLevel), UpdateSlider);
    }
    void UpdateSlider(float value)
    {
        sldProgress.value = value * 100;
        txtProgress.text = (int)sldProgress.value + "%";
        if (value >= 0.9)
            sldProgress.value = 100;
    }
}
