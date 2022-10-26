using UnityEngine;
namespace Cosmos
{
    public static class ModuleInstanceExtensions
    {
        public static GameObject Instance(this IModuleInstance @this)
        {
            return MonoGameManager.Instance.GetModuleGameObject(@this);
        }
    }
}
