using System;
using System.Collections;
using System.Collections.Generic;
namespace kcp
{
    public class KcpService
    {
        public ushort Port = 7777;
        public bool NoDelay = true;
        public uint Interval = 10;
        public int FastResend = 2;
        public bool CongestionWindow = false; // KCP 'NoCongestionWindow' is false by default. here we negate it for ease of use.
        public uint SendWindowSize = 4096; //Kcp.WND_SND; 32 by default. Mirror sends a lot, so we need a lot more.
        public uint ReceiveWindowSize = 4096; //Kcp.WND_RCV; 128 by default. Mirror sends a lot, so we need a lot more.
        // server & client
        public KcpServer server;
        public KcpClient client;
        public Action OnClientConnected;
        public Action<ArraySegment<byte>, byte> OnClientDataReceived;
        public Action OnClientDisconnected;


        public Action<int> OnServerConnected;
        public Action<int, ArraySegment<byte>, int> OnServerDataReceived;
        public Action<int, Exception> OnServerError;
        public Action<int> OnServerDisconnected;

        public void LaunchServer()
        {
            server = new KcpServer(
    (connectionId) => OnServerConnected.Invoke(connectionId),
    (connectionId, message) => OnServerDataReceived.Invoke(connectionId, message, (byte)KcpChannel.Reliable),
    (connectionId) => OnServerDisconnected.Invoke(connectionId),
    NoDelay,
    Interval,
    FastResend,
    CongestionWindow,
    SendWindowSize,
    ReceiveWindowSize
);
        }
        public bool ServerActive() { return server.IsActive(); }
        public void ServerStart() { server.Start(Port); }
        public void ServerSend(int connectionId, KcpChannel channelId, ArraySegment<byte> segment)
        {
            // switch to kcp channel.
            // unreliable or reliable.
            // default to reliable just to be sure.
            switch (channelId)
            {
                case KcpChannel.Unreliable:
                    server.Send(connectionId, segment, KcpChannel.Unreliable);
                    break;
                default:
                    server.Send(connectionId, segment, KcpChannel.Reliable);
                    break;
            }
        }
        public bool ServerDisconnect(int connectionId)
        {
            server.Disconnect(connectionId);
            return true;
        }
        public void ServerStop() { server.Stop(); }
        public void ServerEarlyUpdate()
        {
            // scene change messages disable transports to stop them from
            // processing while changing the scene.
            // -> we need to check enabled here
            // -> and in kcp's internal loops, see Awake() OnCheckEnabled setup!
            // (see also: https://github.com/vis2k/Mirror/pull/379)
            /*if (enabled)*/
            server.TickIncoming();
        }


        public void LaunchClient()
        {
            client = new KcpClient(
    () => OnClientConnected.Invoke(),
    (message) => OnClientDataReceived.Invoke(message, (byte)KcpChannel.Reliable),
    () => OnClientDisconnected.Invoke()
);
        }
        public bool ClientConnected() { return client.connected; }
        public void ClientConnect(string address)
        {
            client.Connect(address, Port, NoDelay, Interval, FastResend, CongestionWindow, SendWindowSize, ReceiveWindowSize);
        }
        public void ClientEarlyUpdate()
        {
            // scene change messages disable transports to stop them from
            // processing while changing the scene.
            // -> we need to check enabled here
            // -> and in kcp's internal loops, see Awake() OnCheckEnabled setup!
            // (see also: https://github.com/vis2k/Mirror/pull/379)
           /* if (enabled)*/ client.TickIncoming();
        }
        public void ClientSend(KcpChannel channelId, ArraySegment<byte> segment)
        {
            // switch to kcp channel.
            // unreliable or reliable.
            // default to reliable just to be sure.
            switch (channelId)
            {
                case KcpChannel.Unreliable:
                    client.Send(segment, KcpChannel.Unreliable);
                    break;
                default:
                    client.Send(segment, KcpChannel.Reliable);
                    break;
            }
        }
        // PrettyBytes function from DOTSNET
        // pretty prints bytes as KB/MB/GB/etc.
        // long to support > 2GB
        // divides by floats to return "2.5MB" etc.
        public static string PrettyBytes(long bytes)
        {
            // bytes
            if (bytes < 1024)
                return $"{bytes} B";
            // kilobytes
            else if (bytes < 1024L * 1024L)
                return $"{(bytes / 1024f):F2} KB";
            // megabytes
            else if (bytes < 1024 * 1024L * 1024L)
                return $"{(bytes / (1024f * 1024f)):F2} MB";
            // gigabytes
            return $"{(bytes / (1024f * 1024f * 1024f)):F2} GB";
        }
    }
}