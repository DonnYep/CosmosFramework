using Cosmos;
using Cosmos.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SceneLoader : UILogicResident
{
    InputField inputTargetLevel;
    protected override void OnInitialization()
    {
        GetUIPanel<Button>("BtnLoad").onClick.AddListener(LoadClick);
        inputTargetLevel = GetUIPanel<InputField>("InputTargetLevel");
    }
    protected override void OnTermination()
    {
        GetUIPanel<Button>("BtnLoad").onClick.RemoveAllListeners();
    }
    void LoadClick()
    {
        Utility.Globle.TargetLevel = inputTargetLevel.text;
        if (Utility.Text.IsNumeric(inputTargetLevel.text))
        {
            int index = int.Parse(inputTargetLevel.text);
            Facade.LoadSceneAsync(index, () => Utility.DebugLog("Scene load Done"));
        }
        else
            Facade.LoadSceneAsync(inputTargetLevel.text,()=>{
                Utility.DebugLog("LoadDoneCallBack Done");
                GameObject go = new GameObject("Done Go");
            });
    }
    void LoadDoneCallBack(Scene scene,LoadSceneMode loadMode)
    {
        Utility.DebugLog("LoadDoneCallBack Done");
        GameObject go = new GameObject("Done Go");
    }
}