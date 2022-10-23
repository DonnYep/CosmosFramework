using Cosmos.Procedure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using Cosmos.UI;

public class EntityLauncherState : ProcedureState
{
    public override void OnDestroy(SimpleFsm<IProcedureManager> fsm)
    {
    }
    public override void OnEnter(SimpleFsm<IProcedureManager> fsm)
    {
    }
    public override void OnExit(SimpleFsm<IProcedureManager> fsm)
    {
    }
    public async override void OnInit(SimpleFsm<IProcedureManager> fsm)
    {
        var mainUICanvas = await CosmosEntry.ResourceManager.LoadPrefabAsync("UICanvas", true);
        mainUICanvas.transform.SetAlignParent(CosmosEntry.UIManager.Instance().transform);
        //设置UGUI资源方案
        CosmosEntry.UIManager.SetUIFormAssetHelper(new UGUIAssetHelper(mainUICanvas.transform));
        var sld = await CosmosEntry.UIManager.OpenUIFormAsync<EntityGameLoadingSlider>(new UIAssetInfo("EntityGameLoadingSlider"));
        await new WaitForNextFrame();
        sld.Active = false;
        await CosmosEntry.UIManager.OpenUIFormAsync<EntityGameLauncherPanel>(new UIAssetInfo("EntityGameLauncherPanel"));
    }
    public override void OnUpdate(SimpleFsm<IProcedureManager> fsm)
    {
    }
}
