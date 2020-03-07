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
    protected override void OnInitialization()
    {
        txtProgress = GetUIPanel<Text>("TxtProgress");
        sldProgress = GetUIPanel<Slider>("SldProgress");
        LoadLevel();
    }
    void LoadLevel()
    {
        sldProgress.value = 0;
        if (Utility.IsNumeric(Utility.Globle.TargetLevel))
        {
            int index = int.Parse(Utility.Globle.TargetLevel);
            Facade.Instance.LoadSceneAsync(index, UpdateSlider);
        }
        else
            Facade.Instance.LoadSceneAsync(Utility.Globle.TargetLevel, UpdateSlider);
    }
    void UpdateSlider(float value)
    {
        sldProgress.value = value * 100;
        txtProgress.text = (int)sldProgress.value + "%";
        if (value >= 0.9)
            sldProgress.value = 100;
    }
}
