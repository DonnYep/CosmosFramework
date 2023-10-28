using Cosmos;
using Cosmos.Procedure;
public class InitState : ProcedureNode
{
    public override void OnDestroy(ProcedureProcessor processor)
    {
    }

    public override void OnEnter(ProcedureProcessor processor)
    {
        Utility.Debug.LogInfo("Enter InitState");
    }

    public override void OnExit(ProcedureProcessor processor)
    {
        Utility.Debug.LogInfo("Exit InitState");
    }

    public override void OnInit(ProcedureProcessor processor)
    {
        CosmosEntry.ProcedureManager.AddProcedureNodes(new YBotState(), new PloyState());
        Utility.Debug.LogInfo("OnInit InitState");
    }

    public override void OnUpdate(ProcedureProcessor processor)
    {
    }

}
