using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
namespace Hydrogen
{
    /// <summary>
    /// tcp client 异步收发
    /// </summary>
    public class TcpClient
    {
        System.Net.Sockets.TcpClient client;
        public System.Net.Sockets.TcpClient Client { get { return client; } }
        /// <summary>
        /// 是否处于连接状态
        /// </summary>
        public bool Connected
        {
            get
            {
                if (client == null)
                    return false;
                return client.Connected;
            }
        }
        NetworkStream outStream;
        MemoryStream memStream;
        BinaryReader reader;
        IHeaderEncoder headerEncoder;
        private byte[] byteBuffer;
        Action onConnected;
        Action<byte[]> onData;
        Action onDisconnected;
        readonly object locker = new object();
        public event Action OnConnected
        {
            add { onConnected += value; }
            remove { onConnected -= value; }
        }
        /// <summary>
        /// 收到数据
        /// </summary>
        public event Action<byte[]> OnData
        {
            add { onData += value; }
            remove { onData -= value; }
        }
        public event Action OnDisconnected
        {
            add { onDisconnected += value; }
            remove { onDisconnected -= value; }
        }
        public bool NoDelay = true;

        public const int MAX_READ = 1 << 13;//1024*(2^3)
        /// <summary>
        /// 发送过期时间，这里使用毫秒计算；
        /// </summary>
        public int SendTimeout = 5000;
        public int ReceiveTimeout = 1000;
        public TcpClient(IHeaderEncoder encoder)
        {
            byteBuffer = new byte[MAX_READ];
            memStream = new MemoryStream();
            reader = new BinaryReader(memStream);
            this.headerEncoder = encoder;
        }
        public void Connect(string host, int port)
        {
            client = null;
            try
            {
                client = new System.Net.Sockets.TcpClient();
                var address = Dns.GetHostAddresses(host);
                if (address.Length == 0)
                {
                    //无效的host地址
                    Log.Error("host invalid");
                }
                var hostAddress = address[0];
                //检测ipv6
                if (hostAddress.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    client.Client = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                }
                else
                {
                    client.Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                client.SendTimeout = SendTimeout;
                client.ReceiveTimeout = ReceiveTimeout;
                client.NoDelay = NoDelay;
                client.BeginConnect(hostAddress, port, new AsyncCallback(OnConnectHandler), null);
            }
            catch (Exception e)
            {
                Disconnect();
                Log.Error(e.Message);
            }
        }
        public void Disconnect()
        {
            if (client != null)
            {
                if (client.Connected)
                {
                    client.Close();
                }
                client = null;
                onDisconnected?.Invoke();
            }
            reader.Close();
            memStream.Close();
        }
        public void SendMessage(byte[] message)
        {
            WriteMessage(message);
        }
        void WriteMessage(byte[] message)
        {
            MemoryStream ms = null;
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                var msglen_buf = headerEncoder.EncodeHeader((ushort)message.Length);
                //组装[header][message]
                writer.Write(msglen_buf);
                writer.Write(message);
                writer.Flush();
                if (client != null && client.Connected)
                {
                    byte[] payload = ms.ToArray();
                    outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
                }
                else
                {
                    Log.Error("client connect false");
                }
            }
        }
        void OnConnectHandler(IAsyncResult result)
        {
            outStream = client.GetStream();
            outStream.BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnReadHandler), null);
            onConnected?.Invoke();
            //client.EndConnect(result);
        }
        void OnReadHandler(IAsyncResult result)
        {
            ushort rcv_byte_len = 0;
            try
            {
                if (client == null)
                {
                    return;
                }
                lock (locker)
                {
                    rcv_byte_len = (ushort)client.GetStream().EndRead(result);
                }
                if (rcv_byte_len < 1)
                {
                    OnDisconnectedHandler(DisconnectResion.Disconnect, "received byte < 1");
                    return;
                }
                OnReceive(byteBuffer, rcv_byte_len);
                lock (locker)
                {
                    if (client == null)
                    {
                        OnDisconnectedHandler(DisconnectResion.Exception, "NULL TCP CLIENT");
                    }
                    else
                    {
                        Array.Clear(byteBuffer, 0, byteBuffer.Length);
                        client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnReadHandler), null);
                    }
                }
            }
            catch (Exception e)
            {
                OnDisconnectedHandler(DisconnectResion.Exception, e.Message);
            }
        }
        void OnWrite(IAsyncResult result)
        {
            try
            {
                outStream.EndWrite(result);
            }
            catch (Exception e)
            {
                Log.Error($"WriteException: {e}");
            }
        }
        void OnReceive(byte[] bytes, int len)
        {
            memStream.Seek(0, SeekOrigin.End);
            memStream.Write(bytes, 0, len);
            memStream.Seek(0, SeekOrigin.Begin);
            var headerLength = headerEncoder.GetHeaderLength();
            while (RemainingBytes() > headerLength)//大于一个header位
            {
                var header = reader.ReadBytes(headerLength);
                var msglen = headerEncoder.DecodeHeader(header);
                if (RemainingBytes() >= msglen)
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(ms);
                    writer.Write(reader.ReadBytes(msglen));
                    ms.Seek(0, SeekOrigin.Begin);
                    OnReceivedMessage(ms);
                }
                else
                {
                    //回退header位
                    memStream.Position = memStream.Position - headerLength;
                    break;
                }
            }
            var buf = reader.ReadBytes((int)RemainingBytes());
            memStream.SetLength(0);
            memStream.Write(buf, 0, buf.Length);//清除并重新写入
        }
        void OnDisconnectedHandler(DisconnectResion resion, string msg)
        {
            switch (resion)
            {
                case DisconnectResion.Exception:
                    Log.Error(msg);
                    break;
                case DisconnectResion.Disconnect:
                    Log.Info(msg);
                    break;
            }
            Disconnect();
        }
        void OnReceivedMessage(MemoryStream ms)
        {
            BinaryReader br = new BinaryReader(ms);
            byte[] message = br.ReadBytes((int)(ms.Length - ms.Position));
            var seg = new ArraySegment<byte>(message, 0, message.Length);
            onData?.Invoke(seg.ToArray());
        }
        long RemainingBytes()
        {
            return memStream.Length - memStream.Position;
        }
    }
}
