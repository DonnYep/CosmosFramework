using UnityEngine;
using Cosmos;
public class MyUILoader : MonoBehaviour
{
    async void Start()
    {
        var mainUICanvas = await CosmosEntry.ResourceManager.LoadPrefabAsync("UI/MainUICanvas", true);
        mainUICanvas.transform.SetAlignParent(CosmosEntry.UIManager.Instance().transform);
        //设置UGUI资源方案
        CosmosEntry.UIManager.SetUIFormAssetHelper(new UGUIAssetHelper(mainUICanvas.transform));
        PureMVC.MVC.RegisterCommand<CMD_Navigate>(MVCEventDefine.CMD_Navigate);
        PureMVC.MVC.Dispatch(MVCEventDefine.CMD_Navigate);
    }
}
