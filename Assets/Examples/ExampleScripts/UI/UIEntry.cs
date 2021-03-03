using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;
using Cosmos.Mvvm;
namespace Cosmos.Test {

    public class UIEntry : MonoBehaviour
    {
        private void Awake()
        {
            CosmosEntry.LaunchHelpers();
            CosmosEntry.LaunchModules();
            CosmosEntry.SceneManager.SetHelper(new DefaultSceneHelper());
        }
        private void Start()
        {
            CosmosEntry.UIManager.SetHelper(new DefaultUIFormHelper());
            Utility.Json.SetHelper(new JsonUtilityHelper());
            var mianUICanvas = CosmosEntry.ResourceManager.LoadPrefabAsync(new AssetInfo(null, null, "UI/MainUICanvas"),(go=> 
            {
                CosmosEntry.UIManager.SetUIRoot(go);
            }),null,true);
            MVVM.RegisterCommand<CMD_Navigate>(MVVMDefine.CMD_Navigate);
            MVVM.Dispatch(MVVMDefine.CMD_Navigate);
        }
    }
}