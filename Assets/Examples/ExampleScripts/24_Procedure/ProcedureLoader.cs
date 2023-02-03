using UnityEngine;
using Cosmos;
public class ProcedureLoader : MonoBehaviour
{
    public void RunYBotState()
    {
        CosmosEntry.ProcedureManager.RunProcedureNode<YBotState>();
    }
    public void RunPolyState()
    {
        CosmosEntry.ProcedureManager.RunProcedureNode<PloyState>();
    }
}
