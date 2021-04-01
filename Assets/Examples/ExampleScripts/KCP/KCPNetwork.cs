using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kcp;
using System;
using System.Text;

namespace Cosmos.Test
{
    public class KCPNetwork : MonoSingleton<KCPNetwork>
    {
        KcpClientService kcpClientService = new KcpClientService();
        public KcpClientService KcpClientService { get { return kcpClientService; } }
        [SerializeField] string ip = "127.0.0.1";
        [SerializeField] int port = 8521;
        [SerializeField] GameObject localPlayerPrefab;
        public GameObject LocalPlayerPrefab { get { return localPlayerPrefab; } }
        [SerializeField] GameObject remotePlayerPrefab;
        public GameObject RemotePlayerPrefab { get { return remotePlayerPrefab; } }

        public event Action OnClientConnected
        {
            add { kcpClientService.OnClientConnected += value; }
            remove { kcpClientService.OnClientConnected -= value; }
        }
        public event Action OnClientDisconnected
        {
            add { kcpClientService.OnClientDisconnected += value; }
            remove { kcpClientService.OnClientDisconnected -= value; }
        }
        public void SendNetKcpMessage(byte[] data)
        {
            var arraySegment = new ArraySegment<byte>(data);
            kcpClientService.ServiceSend(KcpChannel.Reliable, arraySegment);
        }
        public void SendKcpMessage(string msg)
        {
            var buffer = Encoding.UTF8.GetBytes(msg);
            Utility.Debug.LogInfo($"向服务器发送消息，信息长度为：{buffer.Length}");
            var arraySegment = new ArraySegment<byte>(buffer);
            kcpClientService.ServiceSend(KcpChannel.Reliable, arraySegment);
        }
        public void Connect()
        {
            kcpClientService.Port = (ushort)port;
            kcpClientService.ServiceConnect(ip);
        }
        public void Disconnect()
        {
            if (kcpClientService.Connected)
                kcpClientService.ServiceDisconnect();
        }
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
            Log.Info = (s) => Utility.Debug.LogInfo(s);
            Log.Warning = (s) => Utility.Debug.LogInfo(s, MessageColor.YELLOW);
            Log.Error = (s) => Utility.Debug.LogError(s);

            kcpClientService.ServiceSetup();
            kcpClientService.OnClientDataReceived += RsvKcpMsg;
            kcpClientService.OnClientConnected += () => { Utility.Debug.LogInfo("KCP 服务器建立连接"); };
            kcpClientService.OnClientDisconnected += () => { Utility.Debug.LogInfo("KCP 服务器断开连接"); };
        }
        private void OnEnable()
        {
            kcpClientService.ServiceUnpause();
        }
        private void OnDisable()
        {
            kcpClientService.ServicePause();
        }
        private void Update()
        {
            kcpClientService.ServiceTick();
        }
        protected override void OnDestroy()
        {
            kcpClientService.ServiceDisconnect();
        }
        void RsvKcpMsg(ArraySegment<byte> msg, byte channel)
        {
            var str = Encoding.UTF8.GetString(msg.Array);
            Utility.Debug.LogInfo(str, MessageColor.YELLOW);
            try
            {
                var mp = MessagePacket.Deserialize(msg.Array);
                MsgCmdEventCore.Instance.Dispatch(mp.OperationCode, mp);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }

    }
}