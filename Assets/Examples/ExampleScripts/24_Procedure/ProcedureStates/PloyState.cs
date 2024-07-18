using Cosmos;
using Cosmos.Procedure;
public class PloyState : ProcedureNode
{
    public override void OnDestroy(ProcedureProcessor processor)
    {
    }

    public override void OnEnter(ProcedureProcessor processor)
    {
        Utility.Debug.LogInfo("Enter PloyState");
        CosmosEntry.ResourceManager.LoadSceneAsync(new SceneAssetInfo("24_Procedure_PloyState"),null,null,null,null);
    }

    public override void OnExit(ProcedureProcessor processor)
    {
        Utility.Debug.LogInfo("Exit PloyState", DebugColor.orange);
    }

    public override void OnInit(ProcedureProcessor processor)
    {
    }

    public override void OnUpdate(ProcedureProcessor processor)
    {
    }
}
