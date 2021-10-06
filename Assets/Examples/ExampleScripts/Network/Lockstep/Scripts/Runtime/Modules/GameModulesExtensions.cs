using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cosmos;
namespace Cosmos.Lockstep
{
    public static class GameModulesExtensions
    {
        public static GameObject Instance(this IMultiplayManager multiplayManager)
        {
            return MonoGameManager.Instance.GetModuleMount<IMultiplayManager>();
        }
    }
}
