using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    public class NetworkIdentity : MonoBehaviour
    {
        List<NetworkBehaviour> BehaviourCache
        {
            get
            {
                if (behaviourCache == null)
                {
                    behaviourCache = new List<NetworkBehaviour>();
                    var comps = GetComponents<NetworkBehaviour>();
                    behaviourCache.AddRange(comps);
                }
                return behaviourCache;
            }
        }
        List<NetworkBehaviour> behaviourCache;
        public int NetId { get { return netId; } set { netId = value; netIdpending = netId; } }
        [SerializeField] int netId;
        int netIdpending;
        public bool IsAuthority { get; set; }
        internal void OnDeserialize(FixTransportData transportData)
        {
            NetworkBehaviour[] components = BehaviourCache.ToArray();
            var compData = transportData.CompData;
            var length = components.Length;
            for (int i = 0; i < length; i++)
            {
                if (compData.TryGetValue((byte)components[i].NetworkdComponetType, out var jsonData))
                {
                    components[i].OnDeserialize(jsonData);
                }
            }
        }
        internal void OnSerialize(out FixTransportData transportData)
        {
            transportData = new FixTransportData();
            transportData.Conv = netId;
            var length = BehaviourCache.Count;
            var compData = new Dictionary<byte, string>();
            for (int i = 0; i < length; i++)
            {
                var component = BehaviourCache[i];
                compData.TryAdd((byte)component.NetworkdComponetType, component.OnSerialize());
            }
            transportData.CompData = compData;
        }

        void OnValidate()
        {
            netId = netIdpending;
        }
    }
}