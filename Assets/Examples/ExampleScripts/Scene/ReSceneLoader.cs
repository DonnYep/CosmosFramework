using Cosmos;
using Cosmos.UI;
using Cosmos.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ReSceneLoader : UIForm
{
    InputField inputTargetLevel;
    ISceneManager sceneManager;

    protected override void Awake()
    {
        GetUILable<Button>("BtnLoad").onClick.AddListener(LoadClick);
        inputTargetLevel = GetUILable<InputField>("InputTargetLevel");
        sceneManager = GameManager.GetModule<ISceneManager>();
    }
    void LoadClick()
    {
        LevelLoadInfo.TargetLevel = inputTargetLevel.text;
        sceneManager.LoadSceneAsync(new SceneInfo(inputTargetLevel.text), () => Utility.Debug.LogInfo("Scene load Done"));
    }
    void LoadDoneCallBack(Scene scene,LoadSceneMode loadMode)
    {
        Utility.Debug.LogInfo("LoadDoneCallBack Done");
        GameObject go = new GameObject("Done Go");
    }
}