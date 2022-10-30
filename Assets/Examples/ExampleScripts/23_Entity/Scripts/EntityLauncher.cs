using UnityEngine;
using Cosmos;
public class EntityLauncher : MonoBehaviour
{
    void Start()
    {
        var launcherState = new EntityLauncherState();
        var gameState = new EntityGameState();
        CosmosEntry.ProcedureManager.AddProcedures(gameState, launcherState);
        CosmosEntry.ProcedureManager.RunProcedure<EntityLauncherState>();
        CosmosEntry.InputManager.SetInputHelper(new StandardInputHelper());
    }
}
