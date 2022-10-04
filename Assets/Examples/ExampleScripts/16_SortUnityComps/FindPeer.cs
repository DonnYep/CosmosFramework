using UnityEngine;
namespace Cosmos.Test
{
    public class FindPeer : MonoBehaviour
    {
        [SerializeField] Transform targetPeer;
        void Start()
        {
            var childs = Utility.Unity.PeerComponets<PeerBase>(targetPeer,true);
            Utility.Unity.SortCompsByAscending(childs, (pb) => pb.Index);
        }
    }
}