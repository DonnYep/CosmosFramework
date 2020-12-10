using Cosmos;
using Cosmos.UI;
using Cosmos.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ReSceneLoader : UIResidentForm
{
    InputField inputTargetLevel;
    ISceneManager sceneManager;

    protected override void OnInitialization()
    {
        GetUIForm<Button>("BtnLoad").onClick.AddListener(LoadClick);
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
        if (Utility.Text.IsNumeric(inputTargetLevel.text))
        {
            int index = int.Parse(inputTargetLevel.text);
            sceneManager.LoadSceneAsync(index, () => Utility.Debug.LogInfo("Scene load Done"));
        }
        else
            sceneManager.LoadSceneAsync(inputTargetLevel.text, () => Utility.Debug.LogInfo("Scene load Done"));
        //Facade.LoadSceneAsync(inputTargetLevel.text, LoadDoneCallBack);
    }
    void LoadDoneCallBack(Scene scene,LoadSceneMode loadMode)
    {
        Utility.Debug.LogInfo("LoadDoneCallBack Done");
        GameObject go = new GameObject("Done Go");
    }
}