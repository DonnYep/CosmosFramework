using Cosmos;
using Cosmos.Procedure;
public class PloyState : ProcedureNode
{
    public override void OnDestroy(ProcedureProcessor<IProcedureManager> fsm)
    {
    }

    public override void OnEnter(ProcedureProcessor<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Enter PloyState");
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneAssetInfo("24_Procedure_PloyState"));
    }

    public override void OnExit(ProcedureProcessor<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Exit PloyState", DebugColor.orange);
    }

    public override void OnInit(ProcedureProcessor<IProcedureManager> fsm)
    {
    }

    public override void OnUpdate(ProcedureProcessor<IProcedureManager> fsm)
    {
    }
}
