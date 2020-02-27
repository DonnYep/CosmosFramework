using UnityEngine;
using System.Collections;
using Cosmos;
namespace Cosmos.Network
{
    public sealed class NetworkManager : Module<NetworkManager>
    {
        protected override void InitModule()
        {
            RegisterModule(CFModules.NETWORK);
        }
    }
}
