using Cosmos;
using UnityEngine;

public class ProcedureEntrance : MonoBehaviour
{
    private void Start()
    {
        CosmosEntry.ProcedureManager.AddProcedureNodes(new InitState());
    }
}
