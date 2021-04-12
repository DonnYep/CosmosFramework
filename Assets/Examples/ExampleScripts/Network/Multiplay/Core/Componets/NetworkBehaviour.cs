using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Test
{
    public abstract class NetworkBehaviour : MonoBehaviour
    {
        public int NetId { get { return NetworkIdentity.NetId; }set { NetworkIdentity.NetId = value; } }
        public virtual NetworkdComponetType NetworkdComponetType { get; protected set; }
        NetworkIdentity netIdentityCache;
        public NetworkIdentity NetworkIdentity
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
        public bool IsAuthority { get { return NetworkIdentity.IsAuthority; }set { NetworkIdentity.IsAuthority = value; } }
        public abstract string OnSerialize();
        public abstract void OnDeserialize(string compJson);
    }
}
