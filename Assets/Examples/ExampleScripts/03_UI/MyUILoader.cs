using UnityEngine;
using Cosmos;
public class MyUILoader: MonoBehaviour
{
    async void Start()
    {
        var mainUICanvas = await CosmosEntry.ResourceManager.LoadPrefabAsync("UI/MainUICanvas",true);
        CosmosEntry.UIManager.SetUIRoot(mainUICanvas.transform);
        PureMVC.MVC.RegisterCommand<CMD_Navigate>(MVCEventDefine.CMD_Navigate);
        PureMVC.MVC.Dispatch(MVCEventDefine.CMD_Navigate);
    }
}
