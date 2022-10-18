using Cosmos;
using Cosmos.Procedure;
public class PloyState : ProcedureState
{
    public override void OnDestroy(SimpleFsm<IProcedureManager> fsm)
    {
    }

    public override void OnEnter(SimpleFsm<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Enter PloyState");
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneAssetInfo("24_Procedure_PloyState"));
    }

    public override void OnExit(SimpleFsm<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Exit PloyState", DebugColor.orange);
    }

    public override void OnInit(SimpleFsm<IProcedureManager> fsm)
    {
    }

    public override void OnUpdate(SimpleFsm<IProcedureManager> fsm)
    {
    }
}
