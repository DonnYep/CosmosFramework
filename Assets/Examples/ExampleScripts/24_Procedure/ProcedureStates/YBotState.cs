using Cosmos;
using Cosmos.Procedure;
public class YBotState : ProcedureState
{
    public override void OnDestroy(SimpleFsm<IProcedureManager> fsm)
    {
    }

    public override void OnEnter(SimpleFsm<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Enter YBotState");
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneAssetInfo("24_Procedure_YBotState"));
    }

    public override void OnExit(SimpleFsm<IProcedureManager> fsm)
    {
        Utility.Debug.LogInfo("Exit YBotState",DebugColor.orange);
    }

    public override void OnInit(SimpleFsm<IProcedureManager> fsm)
    {
    }

    public override void OnUpdate(SimpleFsm<IProcedureManager> fsm)
    {
    }
}
