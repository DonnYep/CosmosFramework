using UnityEngine;
namespace Cosmos.Test
{
    public abstract class PeerBase : MonoBehaviour
    {
        public int Index;
        public abstract void PeerMsg();
    }
}