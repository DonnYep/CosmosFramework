using UnityEngine;
using Cosmos;
public class EntityLauncher : MonoBehaviour
{
    void Start()
    {
        var gameState = new EntityGameState();
        var launcherState = new EntityLauncherState();
        CosmosEntry.ProcedureManager.AddProcedures(gameState, launcherState);
        CosmosEntry.ProcedureManager.RunProcedure<EntityLauncherState>();
    }
}
