using System;
using kcp2k;
namespace Cosmos
{
    public class KcpServerEndPoint : KcpServer
    {
        public KcpServerEndPoint(Action<int> OnConnected, Action<int, ArraySegment<byte>, KcpChannel> OnData, Action<int> OnDisconnected, Action<int, ErrorCode, string> OnError, bool DualMode, bool NoDelay, uint Interval, int FastResend = 0, bool CongestionWindow = true, uint SendWindowSize = 32, uint ReceiveWindowSize = 128, int Timeout = 10000, uint MaxRetransmits = 20, bool MaximizeSendReceiveBuffersToOSLimit = false) 
            : base(OnConnected, OnData, OnDisconnected, OnError, DualMode, NoDelay, Interval, FastResend, CongestionWindow, SendWindowSize, ReceiveWindowSize, Timeout, MaxRetransmits, MaximizeSendReceiveBuffersToOSLimit)
        {
        }
        public string IPAddress { get { return socket.LocalEndPoint.ToString(); } }
    }
}
