using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;
using Cosmos.Mvvm;
namespace Cosmos.Test {
    [DefaultExecutionOrder(1100)]
    public class UIEntry : MonoBehaviour
    {
        private void Awake()
        {
            CosmosEntry.LaunchAppDomainHelpers();
            CosmosEntry.LaunchAppDomainModules();
            CosmosEntry.InputManager.SetInputDevice(new StandardInputDevice());
            CosmosEntry.ResourceManager.AddLoadChannel(new Resource.ResourceLoadChannel(0, new DefaultResourceLoader()));
        }
        private void Start()
        {
            CosmosEntry.UIManager.SetHelper(new DefaultUIFormHelper());
            Utility.Json.SetHelper(new JsonUtilityHelper());
            var mianUICanvas = CosmosEntry.ResourceManager.LoadPrefabAsync(0,new AssetInfo(null, null, "UI/MainUICanvas"),(go=> 
            {
                CosmosEntry.UIManager.SetUIRoot(go);
            }),null,true);
            MVVM.RegisterCommand<CMD_Navigate>(MVVMDefine.CMD_Navigate);
            MVVM.Dispatch(MVVMDefine.CMD_Navigate);
        }
    }
}