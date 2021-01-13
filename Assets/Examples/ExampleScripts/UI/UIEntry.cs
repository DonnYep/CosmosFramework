using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;
using Cosmos.Mvvm;
namespace Cosmos.Test {
    public class UIEntry : MonoBehaviour
    {
        private void Start()
        {
            MVVM.BindViewModel<VM_Navigate>(UIEventDefine.VM_Navigate);
            MVVM.Fire(UIEventDefine.VM_Navigate);
            CosmosEntry.UIManager.SetHelper(new DefaultUIFormHelper());
            Utility.Json.SetHelper(new JsonUtilityHelper());
            var mianUICanvas = CosmosEntry.ResourceManager.LoadPrefabAsync(new AssetInfo(null, null, "UI/MainUICanvas"),(go=> 
            {
                CosmosEntry.UIManager.SetUIRoot(go);
            }),null,true);
        }
    }
}