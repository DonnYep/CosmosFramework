using Cosmos;
using Cosmos.Procedure;
public class InitState : ProcedureNode
{
    public override void OnDestroy(ProcedureProcessor<IProcedureManager> fsm)
    {
    }

    public override void OnEnter(ProcedureProcessor<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Enter InitState");
    }

    public override void OnExit(ProcedureProcessor<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Exit InitState");
    }

    public override void OnInit(ProcedureProcessor<IProcedureManager> fsm)
    {
        CosmosEntry.ProcedureManager.AddProcedureNodes(new YBotState(), new PloyState());
        Utility.Debug.LogInfo("OnInit InitState");
    }

    public override void OnUpdate(ProcedureProcessor<IProcedureManager> fsm)
    {
    }
}
