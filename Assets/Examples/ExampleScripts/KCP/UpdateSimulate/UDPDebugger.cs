using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
namespace Cosmos.Test
{
    public class UDPDebugger : MonoBehaviour
    {
        [SerializeField] bool sendMsg;
        [SerializeField] int sendSN = 0;
        [Header("毫秒")]
        [SerializeField] int sendInterval = 1000;
        long nextSendTime;
        void Update()
        {
            if (sendMsg)
            {
                if (nextSendTime < Utility.Time.MillisecondNow())
                {
                    nextSendTime = Utility.Time.MillisecondNow() + sendInterval;
                    var str = Encoding.UTF8.GetBytes($"{++sendSN}"); ;
                    CosmosEntry.NetworkManager.SendNetworkMessage(str);
                }
            }
        }
    }
}