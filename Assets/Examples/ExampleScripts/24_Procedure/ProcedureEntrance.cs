using Cosmos;
using UnityEngine;

public class ProcedureEntrance : MonoBehaviour
{
    private void Start()
    {
        CosmosEntry.ProcedureManager.AddProcedures(new InitState());
    }
}
