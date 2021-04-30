using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RUDP
{
    public class RUDPConnection : RUDPSocket
    {
        public bool IsServer { get; set; }
        public bool IsClient { get { return !IsServer; } }
        public ConnectionState State { get; set; }
        public int RecvFrequencyMs { get; set; }
        public int PacketIdLimit { get; set; }
        public int SequenceLimit { get; set; }
        public int ClientStartSequence { get; set; }
        public int ServerStartSequence { get; set; }
        public int MTU { get; set; }

        public delegate void dlgEventVoid();
        public delegate void dlgEventConnection(IPEndPoint ep);
        public delegate void dlgEventUserData(RUDPPacket p);
        public event dlgEventConnection OnClientConnect;
        public event dlgEventConnection OnClientDisconnect;
        public event dlgEventConnection OnConnected;
        public event dlgEventUserData OnPacketReceived;

        private Dictionary<string, IPEndPoint> _clients { get; set; }
        private Dictionary<string, RUDPConnectionData> _sequences { get; set; }

        private bool _isAlive = false;
        private int _maxMTU { get { return (int)(MTU * 0.80); } }
        private object _debugMutex = new object();
        private byte[] _packetHeader = { 0xDE, 0xAD, 0xBE, 0xEF };
        private byte[] _internalHeader = { 0xFA, 0xCE, 0xFE, 0xED };

        private Thread _thRecv;

        public RUDPConnection()
        {
            IsServer = false;
            MTU = 500;
            RecvFrequencyMs = 10;
            PacketIdLimit = int.MaxValue / 2;
            SequenceLimit = int.MaxValue / 2;
            ClientStartSequence = 100;
            ServerStartSequence = 200;
            State = ConnectionState.CLOSED;
        }

        private void Debug(string context)
        {
            lock (_debugMutex)
            {
                //Console.ForegroundColor = IsServer ? ConsoleColor.Cyan : ConsoleColor.Green;
                var str = $"{(IsServer ? "[SERVER]" : "[CLIENT]")}";
                RUDPLog.Info($"{str} : {context}");
                //Console.ResetColor();
            }
        }

        public void Connect(string address, int port)
        {
            IsServer = false;
            State = ConnectionState.OPENING;
            Client(address, port);
            Initialize();
            Send(RemoteEndPoint, RUDPPacketType.SYN);
        }

        public void Listen(string address, int port)
        {
            IsServer = true;
            Server(address, port);
            State = ConnectionState.LISTEN;
            Initialize();
        }

        public virtual void Initialize(bool startThreads = true)
        {
            _isAlive = true;
            _sequences = new Dictionary<string, RUDPConnectionData>();
            _clients = new Dictionary<string, IPEndPoint>();
            InitThreads(startThreads);
        }

        public void InitThreads(bool start)
        {
            _thRecv = new Thread(() =>
            {
                while(_isAlive)
                {
                    ProcessRecvQueue();
                    Thread.Sleep(10);
                }
            });
            if(start)
                _thRecv.Start();
        }

        public void Disconnect()
        {
            State = ConnectionState.CLOSING;
            _isAlive = false;
            _socket.Shutdown(SocketShutdown.Both);
            if (IsServer)
                _socket.Close();
            if (_thRecv != null)
                while (_thRecv.IsAlive)
                    Thread.Sleep(10);
            State = ConnectionState.CLOSED;
        }

        public override void PacketReceive(IPEndPoint ep, byte[] data, int length)
        {
            base.PacketReceive(ep, data, length);
            if (length > _packetHeader.Length && data.Take(_packetHeader.Length).SequenceEqual(_packetHeader))
            {
                RUDPPacket p = RUDPPacket.Deserialize(_packetHeader, data);
                p.Src = IsServer ? ep : RemoteEndPoint;
                p.Received = DateTime.Now;
                InitSequence(p.Src);
                RUDPConnectionData sq = _sequences[p.Src.ToString()];
                Send(p.Src, new RUDPInternalPacket.ACKPacket() { header = _internalHeader, sequence = p.Seq }.Serialize());
                Debug($"ACK SEND -> {p.Src}: {p.Seq}");
                lock (sq.ReceivedPackets)
                    sq.ReceivedPackets.Add(p);
            }
            else if (length > _internalHeader.Length && data.Take(_internalHeader.Length).SequenceEqual(_internalHeader))
            {
                IPEndPoint src = IsServer ? ep : RemoteEndPoint;
                InitSequence(src);
                RUDPConnectionData sq = _sequences[src.ToString()];
                RUDPInternalPacket.ACKPacket ack = RUDPInternalPacket.ACKPacket.Deserialize(data);
                Debug($"ACK RECV <- {src}: {ack.sequence}");
                lock (sq.Pending)
                    sq.Pending.RemoveAll(x => x.Seq == ack.sequence);
            }
            else
                Console.WriteLine($"[{GetType()}] RAW RECV: [{Encoding.ASCII.GetString(data, 0, length)}]");
        }

        public void Send(string data)
        {
            Send(RemoteEndPoint, RUDPPacketType.DAT, RUDPPacketFlags.NUL, Encoding.ASCII.GetBytes(data));
        }

        public void Send(IPEndPoint destination, RUDPPacketType type = RUDPPacketType.DAT, RUDPPacketFlags flags = RUDPPacketFlags.NUL, byte[] data = null, int[] intData = null)
        {
            InitSequence(destination);
            RUDPConnectionData sq = _sequences[destination.ToString()];
            if ((data != null && data.Length < _maxMTU) || data == null)
            {
                SendPacket(new RUDPPacket()
                {
                    Dst = destination,
                    Id = sq.PacketId,
                    Type = type,
                    Flags = flags,
                    Data = data
                });
                sq.PacketId++;
            }
            else if (data != null && data.Length >= _maxMTU)
            {
                int i = 0;
                List<RUDPPacket> PacketsToSend = new List<RUDPPacket>();
                while (i < data.Length)
                {
                    int min = i;
                    int max = _maxMTU;
                    if ((min + max) > data.Length)
                        max = data.Length - min;
                    byte[] buf = data.Skip(i).Take(max).ToArray();
                    PacketsToSend.Add(new RUDPPacket()
                    {
                        Dst = destination,
                        Id = sq.PacketId,
                        Type = type,
                        Flags = flags,
                        Data = buf
                    });
                    i += _maxMTU;
                }
                foreach (RUDPPacket p in PacketsToSend)
                {
                    p.Qty = PacketsToSend.Count;
                    SendPacket(p);
                }
                sq.PacketId++;
            }
            else
                throw new Exception("This should not happen");
            if (sq.PacketId > PacketIdLimit)
                sq.PacketId = 0;
        }

        // ###############################################################################################################################
        // ###############################################################################################################################
        // ###############################################################################################################################

        private bool InitSequence(RUDPPacket p)
        {
            return InitSequence(p.Src == null ? p.Dst : p.Src);
        }

        private bool InitSequence(IPEndPoint ep)
        {
            bool rv = false;
            lock(_sequences)
            {
                if (!_sequences.ContainsKey(ep.ToString()))
                {
                    _sequences[ep.ToString()] = new RUDPConnectionData()
                    {
                        EndPoint = ep,
                        Local = IsServer ? ServerStartSequence : ClientStartSequence,
                        Remote = IsServer ? ClientStartSequence : ServerStartSequence
                    };
                    while (!_sequences.ContainsKey(ep.ToString()))
                        Thread.Sleep(10);
                    Debug($"NEW SEQUENCE: { _sequences[ep.ToString()]}");
                    rv = true;
                }
            }
            return rv;
        }
        
        // ###############################################################################################################################
        // ###############################################################################################################################
        // ###############################################################################################################################

        private void RetransmitPacket(RUDPPacket p)
        {
            p.Retransmit = true;
            SendPacket(p);
        }

        private void SendPacket(RUDPPacket p)
        {
            DateTime dtNow = DateTime.Now;

            InitSequence(p.Dst);
            RUDPConnectionData sq = _sequences[p.Dst.ToString()];

            if (!p.Retransmit)
            {
                p.Seq = sq.Local;
                sq.Local++;
                p.Sent = dtNow;
                lock (sq.Pending)
                    foreach (RUDPPacket unconfirmed in sq.Pending.Where(x => (dtNow - p.Sent).Seconds >= 1))
                        RetransmitPacket(unconfirmed);
                Debug($"SEND -> { p.Dst}: {p}");
            }
            else
                Debug($"RETRANSMIT -> {p.Dst}: {p}");

            lock(sq.Pending)
            {
                sq.Pending.RemoveAll(x => x.Dst.ToString() == p.Dst.ToString() && x.Seq == p.Seq);
                sq.Pending.Add(p);
            }

            Send(p.Dst, p.ToByteArray(_packetHeader));
        }

        public void ProcessRecvQueue()
        {
            foreach(var cdata in _sequences)
            {
                RUDPConnectionData sq = cdata.Value;

                List<RUDPPacket> PacketsToRecv = new List<RUDPPacket>();
                lock (sq.ReceivedPackets)
                    PacketsToRecv.AddRange(sq.ReceivedPackets.OrderBy(x => x.Seq));
                PacketsToRecv = PacketsToRecv.GroupBy(x => x.Seq).Select(g => g.First()).ToList();

                for (int i = 0; i < PacketsToRecv.Count; i++)
                {
                    RUDPPacket p = PacketsToRecv[i];

                    lock (sq.ReceivedPackets)
                        sq.ReceivedPackets.Remove(p);

                    if (p.Seq < sq.Remote)
                        continue;

                    if (p.Seq > sq.Remote)
                    {
                        sq.ReceivedPackets.Add(p);
                        break;
                    }                        

                    Debug($"RECV <- {p.Src}: {p}");

                    if (p.Qty == 0)
                    {
                        sq.Remote++;

                        if (p.Type == RUDPPacketType.SYN)
                        {
                            if (IsServer)
                            {
                                Send(p.Src, RUDPPacketType.SYN, RUDPPacketFlags.ACK);
                                OnClientConnect?.Invoke(p.Src);
                            }
                            else if (p.Flags == RUDPPacketFlags.ACK)
                            {
                                State = ConnectionState.OPEN;
                                OnConnected?.Invoke(p.Src);
                            }
                            continue;
                        }

                        OnPacketReceived?.Invoke(p);
                    }
                    else
                    {
                        // Multipacket!
                        List<RUDPPacket> multiPackets = PacketsToRecv.Skip(i).Take(p.Qty).ToList();
                        if (multiPackets.Count == p.Qty)
                        {
                            Debug($"MULTIPACKET {p.Id}");

                            byte[] buf;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (BinaryWriter bw = new BinaryWriter(ms))
                                    foreach (RUDPPacket mp in multiPackets)
                                    {
                                        bw.Write(mp.Data);
                                        Debug($"RECV MP <- {p.Src}: {mp}");
                                    }
                                buf = ms.ToArray();
                            }
                            Debug($"MULTIPACKET ID {p.Id} DATA: {Encoding.ASCII.GetString(buf)}");

                            OnPacketReceived?.Invoke(new RUDPPacket()
                            {
                                //ACK = p.ACK,
                                Retransmit = p.Retransmit,
                                Sent = p.Sent,
                                Data = buf,
                                Dst = p.Dst,
                                Flags = p.Flags,
                                Id = p.Id,
                                Qty = p.Qty,
                                Received = p.Received,
                                Seq = p.Seq,
                                Src = p.Src,
                                Type = p.Type
                            });

                            sq.Remote += p.Qty;
                            i += p.Qty;
                        }
                        else
                        {
                            if (multiPackets.Count < p.Qty)
                            {
                                sq.ReceivedPackets.Add(p);
                                break;
                            }
                            else
                            {
                                Debug($"P.QTY > MULTIPACKETS.COUNT ({p.Qty} > {multiPackets.Count})");
                                throw new Exception();
                            }
                        }
                    }
                }
            }
        }        
    }
}
