using Cosmos;
using Cosmos.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cosmos.Scene;

public class SceneLoader : UILogicResident
{
    InputField inputTargetLevel;
    ISceneManager sceneManager;

    protected override void OnInitialization()
    {
        GetUIPanel<Button>("BtnLoad").onClick.AddListener(LoadClick);
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
        if (Utility.Text.IsNumeric(inputTargetLevel.text))
        {
            int index = int.Parse(inputTargetLevel.text);
            sceneManager.LoadSceneAsync(index, () => Utility.Debug.LogInfo("Scene load Done"));
        }
        else
            sceneManager.LoadSceneAsync(inputTargetLevel.text,()=>{
                Utility.Debug.LogInfo ("LoadDoneCallBack Done");
                GameObject go = new GameObject("Done Go");
            });
    }
    void LoadDoneCallBack(Scene scene,LoadSceneMode loadMode)
    {
        Utility.Debug.LogInfo("LoadDoneCallBack Done");
        GameObject go = new GameObject("Done Go");
    }
}