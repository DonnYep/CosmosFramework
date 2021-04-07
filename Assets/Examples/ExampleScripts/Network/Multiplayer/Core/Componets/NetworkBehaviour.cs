using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Test
{
    public class NetworkBehaviour:MonoBehaviour
    {
        public int NetId { get { return netId; } set { netId = value; netIdpending = netId; } }
        [SerializeField] int netId;
        int netIdpending;
        public virtual NetworkdComponetType NetworkdComponetType { get; set; }
        /// <summary>
        /// 是否是本地；
        /// </summary>
        public bool IsAuthority { get; set; }
        public virtual void OnDeserialize(NetworkReader reader){}
        public virtual void OnSerialize(NetworkWriter writer) { }
        void OnValidate()
        {
            netId = netIdpending;
        }
    }
}
