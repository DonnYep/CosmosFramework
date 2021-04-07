using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    [RequireComponent(typeof(NetworkTransform))]
    public class NetworkIndentity : MonoBehaviour
    {
        Dictionary<byte, NetworkBehaviour> networkBehaviourDict;
        public Dictionary<byte, NetworkBehaviour> NetworkBehaviourDict
        {
            get
            {
                if (networkBehaviourDict == null)
                {
                    networkBehaviourDict = new Dictionary<byte, NetworkBehaviour>();
                    var comps = GetComponents<NetworkBehaviour>();
                    for (int i = 0; i < comps.Length; i++)
                    {
                        networkBehaviourDict.Add((byte)comps[i].NetworkdComponetType, comps[i]);
                    }
                }
                return networkBehaviourDict;
            }
        }
        List<NetworkBehaviour> BehaviourCache
        {
            get
            {
                if (behaviourCache == null)
                {
                    behaviourCache = new List<NetworkBehaviour>();
                    behaviourCache.AddRange(NetworkBehaviourDict.Values.ToList());
                }
                return behaviourCache;
            }
        }
        List<NetworkBehaviour> behaviourCache;
        public int NetId { get { return netId; } set { netId = value; netIdpending = netId; } }
        [SerializeField] int netId;
        int netIdpending;
        public bool IsAuthority { get; set; }
        internal void OnDeserializeAllSafely(NetworkReader reader, bool initialState)
        {
            // deserialize all components that were received
            NetworkBehaviour[] components = BehaviourCache.ToArray();
            while (reader.Position < reader.Length)
            {
                // read & check index [0..255]
                byte index = reader.ReadByte();
                if (index < components.Length)
                {
                    // deserialize this component
                    OnDeserializeSafely(components[index], reader, initialState);
                }
            }
        }
        internal void OnSerializeAllSafely(NetworkWriter Writer)
        {
            var length = BehaviourCache.Count;
            for (int i = 0; i < length; i++)
            {
                BehaviourCache[i].OnSerialize(Writer);
            }
        }
        void OnDeserializeSafely(NetworkBehaviour comp, NetworkReader reader, bool initialState)
        {
            int contentSize = reader.ReadInt32();
            int chunkStart = reader.Position;
            int chunkEnd = reader.Position + contentSize;
            try
            {
                comp.OnDeserialize(reader);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            if (reader.Position != chunkEnd)
            {
                // warn the user
                int bytesRead = reader.Position - chunkStart;
                Debug.LogWarning("OnDeserialize was expected to read " + contentSize + " instead of " + bytesRead + " bytes for object:" + name + " component=" + comp.GetType() + " sceneId=" + ("X") + ". Make sure that OnSerialize and OnDeserialize write/read the same amount of data in all cases.");
                reader.Position = chunkEnd;
            }
        }
        void OnValidate()
        {
            netId = netIdpending;
        }
    }
}