using Cosmos;
using Cosmos.Procedure;
public class InitState : ProcedureState
{
    public override void OnDestroy(ProcedureFsm<IProcedureManager> fsm)
    {
    }

    public override void OnEnter(ProcedureFsm<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Enter InitState");
    }

    public override void OnExit(ProcedureFsm<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Exit InitState");
    }

    public override void OnInit(ProcedureFsm<IProcedureManager> fsm)
    {
        CosmosEntry.ProcedureManager.AddProcedures(new YBotState(), new PloyState());
        Utility.Debug.LogInfo("OnInit InitState");
    }

    public override void OnUpdate(ProcedureFsm<IProcedureManager> fsm)
    {
    }
}
