using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Test
{
    public class NetworkBehaviour : MonoBehaviour
    {
        public int NetId { get { return netIdentity.NetId; }set { netIdentity.NetId = value; } }
        public virtual NetworkdComponetType NetworkdComponetType { get; protected set; }
        NetworkIdentity netIdentityCache;
        public NetworkIdentity netIdentity
        {
            get
            {
                if (netIdentityCache is null)
                {
                    netIdentityCache = GetComponent<NetworkIdentity>();
                    if (netIdentityCache is null)
                    {
                        Utility.Debug.LogError("There is no NetworkIdentity on " + name + ". Please add one.");
                    }
                }
                return netIdentityCache;
            }
        }
        public bool IsAuthority { get { return netIdentityCache.IsAuthority; }set { netIdentityCache.IsAuthority = value; } }
        public virtual void OnDeserialize(NetworkReader reader) { }
        public virtual void OnSerialize(NetworkWriter writer) { }

    }
}
