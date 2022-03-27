using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace Cosmos.Lockstep
{
    public class GameEntry:CosmosEntry
    {
        public static IServiceManager  ServiceManager{ get { return GetModule<IServiceManager>(); } }
        public static IMultiplayManager   MultiplayManager{ get { return GetModule<IMultiplayManager>(); } }
    }
}
