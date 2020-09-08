﻿using Cosmos;
using Cosmos.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ReSceneLoader : UILogicResident
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
        Utility.Global.TargetLevel = inputTargetLevel.text;
        if (Utility.Text.IsNumeric(inputTargetLevel.text))
        {
            int index = int.Parse(inputTargetLevel.text);
            Facade.LoadSceneAsync(index, () => Utility.Debug.LogInfo("Scene load Done"));
        }
        else
            Facade.LoadSceneAsync(inputTargetLevel.text, () => Utility.Debug.LogInfo("Scene load Done"));
        //Facade.LoadSceneAsync(inputTargetLevel.text, LoadDoneCallBack);
    }
    void LoadDoneCallBack(Scene scene,LoadSceneMode loadMode)
    {
        Utility.Debug.LogInfo("LoadDoneCallBack Done");
        GameObject go = new GameObject("Done Go");
    }
}