using Cosmos;
using Cosmos.Procedure;
public class YBotState : ProcedureState
{
    public override void OnDestroy(ProcedureFsm<IProcedureManager> fsm)
    {
    }

    public override void OnEnter(ProcedureFsm<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Enter YBotState");
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneAssetInfo("24_Procedure_YBotState"));
    }

    public override void OnExit(ProcedureFsm<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Exit YBotState",DebugColor.orange);
    }

    public override void OnInit(ProcedureFsm<IProcedureManager> fsm)
    {
    }

    public override void OnUpdate(ProcedureFsm<IProcedureManager> fsm)
    {
    }
}
