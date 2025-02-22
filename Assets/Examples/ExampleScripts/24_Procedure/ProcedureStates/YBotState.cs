﻿using Cosmos;
using Cosmos.Procedure;
public class YBotState : ProcedureNode
{
    public override void OnDestroy(ProcedureProcessor processor)
    {
    }

    public override void OnEnter(ProcedureProcessor processor)
    {
        Utility.Debug.LogInfo("Enter YBotState");
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneAssetInfo("24_Procedure_YBotState"));
    }

    public override void OnExit(ProcedureProcessor processor)
    {
        Utility.Debug.LogInfo("Exit YBotState",DebugColor.orange);
    }

    public override void OnInit(ProcedureProcessor processor)
    {
    }

    public override void OnUpdate(ProcedureProcessor processor)
    {
    }
}
