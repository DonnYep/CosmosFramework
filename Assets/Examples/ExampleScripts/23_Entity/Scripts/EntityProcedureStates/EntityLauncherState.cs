using Cosmos.Procedure;
using Cosmos;
using Cosmos.UI;
using Cosmos.Entity;

public class EntityLauncherState : ProcedureState
{
    public override void OnDestroy(ProcedureFsm<IProcedureManager> fsm)
    {
    }
    public override void OnEnter(ProcedureFsm<IProcedureManager> fsm)
    {
    }
    public override void OnExit(ProcedureFsm<IProcedureManager> fsm)
    {
    }
    public async override void OnInit(ProcedureFsm<IProcedureManager> fsm)
    {
        var mainUICanvas = await CosmosEntry.ResourceManager.LoadPrefabAsync("UICanvas", true);
        mainUICanvas.transform.SetAlignParent(CosmosEntry.UIManager.Instance().transform);
        //设置UGUI资源方案
        CosmosEntry.UIManager.SetUIFormAssetHelper(new UGUIAssetHelper(mainUICanvas.transform));
        var sld = await CosmosEntry.UIManager.OpenUIFormAsync<EntityGameLoadingSlider>(new UIAssetInfo("EntityGameLoadingSlider"));
        //await new WaitForNextFrame();
        sld.Active = false;
        await CosmosEntry.UIManager.OpenUIFormAsync<EntityGameLauncherPanel>(new UIAssetInfo("EntityGameLauncherPanel"));
        CosmosEntry.EntityManager.RegisterEntityAsync<EnmeyEntity>(new EntityAssetInfo(EntityContants.EntityEnmey));
        CosmosEntry.EntityManager.RegisterEntityAsync<BulletEntity>(new EntityAssetInfo(EntityContants.EntityBullet));
    }
    public override void OnUpdate(ProcedureFsm<IProcedureManager> fsm)
    {
    }
}
