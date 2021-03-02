using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
namespace Cosmos.Test
{
    public class FindPeer : MonoBehaviour
    {
        [SerializeField]
        List<Transform> peers;
        [SerializeField]
        List<PeerBase> peerComps;
        void Start()
        {
            var childs = Utility.Unity.PeersComponet<PeerBase>(transform);
            Utility.Unity.SortCompsByAscending(childs, (pb) => pb.Index);
        }
    }
}