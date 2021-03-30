using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kcp;
using System;
using System.Text;

namespace Cosmos.Test
{
    public class KCPNetwork : MonoBehaviour
    {
        KcpService kcpService;
        private void Awake()
        {
            Log.Info = Debug.Log;
            Log.Warning = Debug.LogWarning;
            Log.Error = Debug.LogError;

            kcpService = new KcpService();
            kcpService.LaunchClient();
            kcpService.ClientConnect("localhost");
        }
        void Start()
        {
            Log.Info("KCP向服务器发送消息");
            kcpService.client.Unpause();

            kcpService.ClientSend(KcpChannel.Reliable, new ArraySegment<byte>(Encoding.UTF8.GetBytes("Cosmos KCP 客户端发送消息测试")));
        }
        private void Update()
        {
            kcpService.ClientEarlyUpdate();
        }
    }
}