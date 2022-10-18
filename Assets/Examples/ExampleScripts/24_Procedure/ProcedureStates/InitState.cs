using Cosmos;
using Cosmos.Procedure;
public class InitState : ProcedureState
{
    public override void OnDestroy(SimpleFsm<IProcedureManager> fsm)
    {
    }

    public override void OnEnter(SimpleFsm<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Enter InitState");
    }

    public override void OnExit(SimpleFsm<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Exit InitState");
    }

    public override void OnInit(SimpleFsm<IProcedureManager> fsm)
    {
        CosmosEntry.ProcedureManager.AddProcedures(new YBotState(), new PloyState());
        Utility.Debug.LogInfo("OnInit InitState");
    }

    public override void OnUpdate(SimpleFsm<IProcedureManager> fsm)
    {
    }
}
