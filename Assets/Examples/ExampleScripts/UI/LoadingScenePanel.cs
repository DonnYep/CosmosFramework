using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos.UI;
using Cosmos;
/// <summary>
/// 显示进度的脚本
/// </summary>
public class LoadingScenePanel : UILogicResident
{
    Text txtProgress;
    Slider sldProgress;
    float currentProgress;
    protected override void OnInitialization()
    {
        txtProgress = GetUIPanel<Text>("TxtProgress");
        sldProgress = GetUIPanel<Slider>("SldProgress");
        LoadLevel();
        Facade.RefreshHandler += RefreshHandler;
    }
    protected override void OnTermination()
    {
        Facade.RefreshHandler -= RefreshHandler;
    }

    void LoadLevel()
    {
        sldProgress.value = 0;
        if (Utility.Text.IsNumeric(Utility.Global.TargetLevel))
        {
            int index = int.Parse(Utility.Global.TargetLevel);
            Facade.LoadSceneAsync(index, UpdateSlider);
        }
        else
            Facade.LoadSceneAsync(Utility.Global.TargetLevel, UpdateSlider);
    }
    void UpdateSlider(float value)
    {
        currentProgress = value;
    }
    void RefreshHandler()
    {
        txtProgress.text = (int)sldProgress.value + "%";
        if (currentProgress >= 0.9f)
        {
            currentProgress = 1;
        }
        sldProgress.value = Mathf.Lerp(sldProgress.value, currentProgress * 100, Time.deltaTime);

        if (sldProgress.value > 0.99f)
        {
            sldProgress.value = 100;
        }
    }
}
