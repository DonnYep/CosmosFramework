using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RUDP
{
    public class RUDPSocket
    {
        internal Socket _socket;
        private const int bufSize = 64 * 1024;
        private StateObject state = new StateObject();
        private EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;
        public IPEndPoint LocalEndPoint;
        public IPEndPoint RemoteEndPoint;

        public class StateObject
        {
            public byte[] buffer = new byte[bufSize];
        }

        protected void Server(string address, int port)
        {
            LocalEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(LocalEndPoint);
            Receive();
        }

        protected void Client(string address, int port)
        {
            bool connect = false;
            IPAddress ipAddress;
            if (IPAddress.TryParse(address, out ipAddress))
            {
                Console.WriteLine("Connecting to {0}", ipAddress);
                RemoteEndPoint = new IPEndPoint(ipAddress, port);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.Connect(RemoteEndPoint);
                connect = true;
            }
            else
                foreach (IPAddress ip in Dns.GetHostEntry(address).AddressList)
                {
                    try
                    {
                        Console.WriteLine("Trying {0}", ip);
                        RemoteEndPoint = new IPEndPoint(ip, port);
                        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        _socket.Connect(RemoteEndPoint);
                        connect = true;
                        break;
                    }
                    catch (Exception) { }
                }
            if (!connect)
                throw new Exception("Unable to connect");
            Receive();
        }

        internal void Send(IPEndPoint endPoint, byte[] data)
        {
            PacketSending(endPoint, data, data.Length);
            _socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, endPoint, (ar) =>
            {
                try
                {
                    StateObject so = (StateObject)ar.AsyncState;
                    int bytes = _socket.EndSend(ar);
                }
                catch (Exception ex)
                {
                    SocketError(ex);
                }
            }, state);
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref ep, recv = (ar) =>
            {
                StateObject so = (StateObject)ar.AsyncState;
                try
                {
                    int bytes = _socket.EndReceiveFrom(ar, ref ep);
                    byte[] data = new byte[bytes];
                    Buffer.BlockCopy(so.buffer, 0, data, 0, bytes);
                    _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref ep, recv, so);
                    PacketReceive((IPEndPoint)ep, data, bytes);
                }
                catch (Exception ex)
                {
                    SocketError(ex);
                }
            }, state);
        }

        public virtual void SocketError(Exception ex) { }

        public virtual int PacketSending(IPEndPoint endPoint, byte[] data, int length)
        {
            RUDPLog.Trace($"SEND -> {endPoint}: {Encoding.UTF8.GetString(data, 0, length)}");
            return -1;
        }

        public virtual void PacketReceive(IPEndPoint ep, byte[] data, int length)
        {
            RUDPLog.Trace($"RECV <- {ep}: {Encoding.UTF8.GetString(data, 0, length)}");
        }
    }
}
