using UnityEngine;
namespace Cosmos.Test
{
    public class FindPeer : MonoBehaviour
    {
        [SerializeField] Transform targetPeer;
        void Start()
        {
            var childs =targetPeer.PeerComponets<PeerBase>(true);
            Utility.Unity.SortCompsByAscending(childs, (pb) => pb.Index);
        }
    }
}