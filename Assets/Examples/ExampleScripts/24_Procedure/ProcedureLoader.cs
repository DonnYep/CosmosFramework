using UnityEngine;
using Cosmos;
public class ProcedureLoader : MonoBehaviour
{
    public void RunYBotState()
    {
        CosmosEntry.ProcedureManager.RunProcedure<YBotState>();
    }
    public void RunPolyState()
    {
        CosmosEntry.ProcedureManager.RunProcedure<PloyState>();
    }
}
