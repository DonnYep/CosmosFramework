using Cosmos;
using Cosmos.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cosmos.Scene;

public class SceneLoader : UIForm
{
    InputField inputTargetLevel;
    protected override void Awake()
    {
        GetUILable<Button>("BtnLoad").onClick.AddListener(LoadClick);
        inputTargetLevel = GetUILable<InputField>("InputTargetLevel");
    }
    void LoadClick()
    {
        LevelLoadInfo.TargetLevel = inputTargetLevel.text;
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneInfo(inputTargetLevel.text), () =>
        {
            Utility.Debug.LogInfo("LoadDoneCallBack Done");
            GameObject go = new GameObject("Done Go");
        });
    }
    void LoadDoneCallBack(Scene scene, LoadSceneMode loadMode)
    {
        Utility.Debug.LogInfo("LoadDoneCallBack Done");
        GameObject go = new GameObject("Done Go");
    }
}