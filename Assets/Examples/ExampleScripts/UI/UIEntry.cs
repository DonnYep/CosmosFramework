using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;
using PureMVC;

namespace Cosmos.Test
{
    public class UIEntry : Entry
    {
        private void Start()
        {
            Utility.Json.SetHelper(new JsonUtilityHelper());
            var mianUICanvas = CosmosEntry.ResourceManager.LoadPrefabAsync(new AssetInfo(null, "UI/MainUICanvas"), go =>
             {
                 CosmosEntry.UIManager.SetUIRoot(go.transform);
             }, null, true);
            PureMVC.MVC.RegisterCommand<CMD_Navigate>(MVCEventDefine.CMD_Navigate);
            PureMVC.MVC.Dispatch(new GameNotifyArgs(MVCEventDefine.CMD_Navigate));
        }
    }
}