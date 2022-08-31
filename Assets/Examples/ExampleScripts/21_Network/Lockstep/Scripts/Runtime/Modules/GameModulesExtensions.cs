using UnityEngine;
namespace Cosmos.Lockstep
{
    public static class GameModulesExtensions
    {
        public static GameObject Instance(this IMultiplayManager multiplayManager)
        {
            return MonoGameManager.Instance.GetModuleInstance<IMultiplayManager>();
        }
    }
}
