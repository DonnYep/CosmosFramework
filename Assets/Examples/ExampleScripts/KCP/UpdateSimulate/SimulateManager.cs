using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace Cosmos.Test
{
    public class SimulateManager:MonoSingleton<SimulateManager>
    {
        Dictionary<byte, object> dataDict;
        Dictionary<int, NetworkIndentity> networkIdentityDict;
        List<NetworkIndentity> netIdCache;
        NetworkWriter writer;
        Dictionary<int, Pool<NetworkWriter>> writePool;
        protected override void Awake()
        {
            base.Awake();
            netIdCache = new List<NetworkIndentity>();
            networkIdentityDict = new Dictionary<int, NetworkIndentity>();
            dataDict = new Dictionary<byte, object>();
            CosmosEntry.NetworkManager.OnReceiveData += NetworkManager_OnReceiveData;
        }
        void NetworkManager_OnReceiveData(ArraySegment<byte> arrSeg)
        {
            var mp= MessagePacket.Deserialize(arrSeg.Array);
            if (mp.OperationCode == CmdDefine.PlayerInput)
            {
                var length=netIdCache.Count;
                for (int i = 0; i < length; i++)
                {
                 //   dataDict.Add(netIdCache[i], netIdCache[i].OnSerializeAllSafely(writer));
                }
            }
        }
    }
}
