using UnityEngine;
namespace Cosmos.Lockstep
{
    public class MultiplayConfig : MonoSingleton<MultiplayConfig>
    {
        [SerializeField] GameObject localPlayerPrefab;
        public GameObject LocalPlayerPrefab { get { return localPlayerPrefab; } }
        [SerializeField] GameObject remotePlayerPrefab;
        public GameObject RemotePlayerPrefab { get { return remotePlayerPrefab; } }
        [Header("发送周期，毫秒")]
        [SerializeField] int sendIntervalMS = 100;
        public int SendIntervalMS { get { return sendIntervalMS; } }
    }
}
