using UnityEngine;
namespace Cosmos
{
    public static class ModuleInstanceExtensions
    {
        public static GameObject InstanceObject(this IModuleManager @this)
        {
            return MonoGameManager.Instance.GetModuleInstanceObject(@this);
        }
    }
}
