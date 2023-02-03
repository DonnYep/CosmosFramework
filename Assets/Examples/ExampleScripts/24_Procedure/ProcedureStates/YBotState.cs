using Cosmos;
using Cosmos.Procedure;
public class YBotState : ProcedureNode
{
    public override void OnDestroy(ProcedureProcessor<IProcedureManager> fsm)
    {
    }

    public override void OnEnter(ProcedureProcessor<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Enter YBotState");
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneAssetInfo("24_Procedure_YBotState"));
    }

    public override void OnExit(ProcedureProcessor<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Exit YBotState",DebugColor.orange);
    }

    public override void OnInit(ProcedureProcessor<IProcedureManager> fsm)
    {
    }

    public override void OnUpdate(ProcedureProcessor<IProcedureManager> fsm)
    {
    }
}
