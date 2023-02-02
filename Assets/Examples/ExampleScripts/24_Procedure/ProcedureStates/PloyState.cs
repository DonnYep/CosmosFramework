using Cosmos;
using Cosmos.Procedure;
public class PloyState : ProcedureState
{
    public override void OnDestroy(ProcedureFsm<IProcedureManager> fsm)
    {
    }

    public override void OnEnter(ProcedureFsm<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Enter PloyState");
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneAssetInfo("24_Procedure_PloyState"));
    }

    public override void OnExit(ProcedureFsm<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Exit PloyState", DebugColor.orange);
    }

    public override void OnInit(ProcedureFsm<IProcedureManager> fsm)
    {
    }

    public override void OnUpdate(ProcedureFsm<IProcedureManager> fsm)
    {
    }
}
