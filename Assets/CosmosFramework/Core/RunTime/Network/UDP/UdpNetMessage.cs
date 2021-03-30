using Cosmos.Reference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    public class UdpNetMessage : INetworkMessage, IReference
    {
        /// <summary>
        /// 业务报文消息包体大小；
        /// 取值范围0~65535；
        /// 约64K一个包
        /// </summary>
        public ushort ServiceMsgLength { get; private set; }
        /// <summary>
        /// 会话ID;
        /// 为一个表示会话编号的整数，
        /// 和TCP的 conv一样，通信双方需保证 conv相同，
        /// 相互的数据包才能够被接受。conv唯一标识一个会话，但通信双方可以同时存在多个会话。
        /// </summary>
        public long Conv { get; set; }
        /// <summary>
        /// Kcp协议类型:
        /// 用来区分分片的作用。
        /// IKCP_CMD_PUSH：数据分片；
        /// IKCP_CMD_ACK：ack分片； 
        /// IKCP_CMD_WASK：请求告知窗口大小；
        /// IKCP_CMD_WINS：告知窗口大小。
        /// </summary>
        public ushort Cmd { get; set; }
        /// <summary>
        /// 时间戳TimeStamp
        /// </summary>
        public long TS { get; set; }
        /// <summary>
        /// serial number
        ///当前 message序号，按1累次递增。
        /// </summary>
        public uint SN { get; set; }
        /// <summary>
        /// 第一个未确认的包
        /// </summary>
        public uint Snd_una { get; set; }
        /// <summary>
        ///下一个需要发送的msg序号
        /// </summary>
        public uint Snd_nxt { get; set; }
        /// <summary>
        /// 待接收消息序号；
        /// </summary>
        public uint Rcv_nxt { get; set; }
        /// <summary>
        /// 前后端通讯的操作码；
        /// </summary>
        public ushort OperationCode { get; set; }
        /// <summary>
        /// 业务报文
        /// </summary>
        public byte[] ServiceMsg { get; set; }
        /// <summary>
        /// 消息重传次数；
        /// 标准KCP库中（C版）重传上线是20；
        /// </summary>
        public ushort RecurCount { get; set; }
        /// <summary>
        /// 是否是完整报文
        /// </summary>
        public bool IsFull { get; private set; }
        /// <summary>
        /// 存储消息字节流的内存
        /// </summary>
        byte[] Buffer { get; set; }
        /// <summary>
        /// 默认构造
        /// </summary>
        public UdpNetMessage() { }
        /// <summary>
        /// 用于验证的消息构造
        /// </summary>
        /// <param name="conv">会话ID</param>
        /// <param name="cmd">协议ID</param>
        public UdpNetMessage(long conv, byte cmd)
        {
            ServiceMsgLength = 0;
            Conv = conv;
            Cmd = cmd;
        }
        /// <summary>
        /// MSG报文构造
        /// </summary>
        /// <param name="conv">会话ID</param>
        /// <param name="sn">msg序号</param>
        /// <param name="cmd">协议ID</param>
        /// <param name="opCode">操作码</param>
        /// <param name="message">消息内容</param>
        public UdpNetMessage(long conv, uint sn, byte cmd, ushort opCode, byte[] message)
        {
            if (message == null)
                ServiceMsgLength = 0;
            else
                ServiceMsgLength = (ushort)message.Length;
            Conv = conv;
            SN = sn;
            Cmd = cmd;
            ServiceMsg = message;
            Rcv_nxt = sn;
            Snd_nxt = SN + 1;
            OperationCode = opCode;
        }
        /// <summary>
        /// ACK报文构造
        /// </summary>
        /// <param name="conv">会话ID</param>
        /// <param name="snd_una">未确认的报文</param>
        /// <param name="sn">msgID</param>
        /// <param name="cmd">协议</param>
        /// <param name="opCode">前后端消息ID</param>
        public UdpNetMessage(long conv, uint snd_una, uint sn, ushort cmd, ushort opCode)
        {
            Conv = conv;
            Snd_una = snd_una;
            SN = sn;
            Rcv_nxt = SN;
            Snd_nxt = SN + 1;
            Cmd = cmd;
            OperationCode = opCode;
        }
        /// <summary>
        /// 消息构造
        /// </summary>
        /// <param name="buffer">包含所有信息的buffer</param>
        public UdpNetMessage(byte[] buffer)
        {
            DecodeMessage(buffer);
        }
        /// <summary>
        /// 消息构造
        /// </summary>
        /// <param name="udpNetMsg">另一个消息体</param>
        /// <param name="message">转换为二进制的消息体</param>
        public UdpNetMessage(UdpNetMessage udpNetMsg, byte[] message)
        {
            if (message == null)
                ServiceMsgLength = 0;
            else
                ServiceMsgLength = (ushort)message.Length;
            Conv = udpNetMsg.Conv;
            SN = udpNetMsg.SN;
            Cmd = udpNetMsg.Cmd;
            ServiceMsg = message;
            Rcv_nxt = udpNetMsg.SN;
            Snd_nxt = SN + 1;
            OperationCode = udpNetMsg.OperationCode;
        }
        /// <summary>
        /// 解析UDP数据报文
        /// </summary>
        /// <param name="buffer"></param>
        public bool DecodeMessage(byte[] buffer)
        {
            if (buffer == null)
            {
                IsFull = false;
                return false;
            }
            if (buffer.Length >= 2)
            {
                ServiceMsgLength = BitConverter.ToUInt16(buffer, 0);
                if (buffer.Length == ServiceMsgLength + 38)
                {
                    IsFull = true;
                }
            }
            else
            {
                IsFull = false;
                return false;
            }
            Conv = BitConverter.ToInt64(buffer, 2);
            Cmd = BitConverter.ToUInt16(buffer, 10);
            TS = BitConverter.ToInt64(buffer, 12);
            SN = BitConverter.ToUInt32(buffer, 20);
            Snd_una = BitConverter.ToUInt32(buffer, 24);
            Snd_nxt = BitConverter.ToUInt32(buffer, 28);
            Rcv_nxt = BitConverter.ToUInt32(buffer, 32);
            OperationCode = BitConverter.ToUInt16(buffer, 36);
            if (Cmd == UdpProtocol.MSG)
            {
                ServiceMsg = new byte[ServiceMsgLength];
                Array.Copy(buffer, 38, ServiceMsg, 0, ServiceMsgLength);
            }
            return true;
        }
        /// <summary>
        /// 编码UDP报文消息
        /// </summary>
        /// <returns>编码后的消息字节流</returns>
        public byte[] EncodeMessage()
        {
            if (Cmd == UdpProtocol.ACK)
                ServiceMsgLength = 0;
            int srvMsgLen=0;
            if (ServiceMsg != null)
            {
                srvMsgLen = ServiceMsg.Length;
                ServiceMsgLength = Convert.ToUInt16(srvMsgLen);
            }
            byte[] data = new byte[38 + srvMsgLen];
            byte[] len = BitConverter.GetBytes(srvMsgLen);
            byte[] conv = BitConverter.GetBytes(Conv);
            byte[] cmd = BitConverter.GetBytes(Cmd);
            byte[] ts = BitConverter.GetBytes(TS);
            byte[] sn = BitConverter.GetBytes(SN);
            byte[] snd_una = BitConverter.GetBytes(Snd_una);
            byte[] snd_nxt = BitConverter.GetBytes(Snd_nxt);
            byte[] rcv_nxt = BitConverter.GetBytes(Rcv_nxt);
            byte[] opCode = BitConverter.GetBytes(OperationCode);
            Array.Copy(len, 0, data, 0, 2);
            Array.Copy(conv, 0, data, 2, 8);
            Array.Copy(cmd, 0, data, 10, 2);
            Array.Copy(ts, 0, data, 12, 8);
            Array.Copy(sn, 0, data, 20, 4);
            Array.Copy(snd_una, 0, data, 24, 4);
            Array.Copy(snd_nxt, 0, data, 28, 4);
            Array.Copy(rcv_nxt, 0, data, 32, 4);
            Array.Copy(opCode, 0, data, 36, 2);
            //如果不是ACK报文，则追加数据
            if (Cmd == UdpProtocol.MSG)
                if (ServiceMsg != null)//空包保护
                    Array.Copy(ServiceMsg, 0, data, 38, srvMsgLen);
            Buffer = data;
            return data;
        }
        public byte[] GetBuffer()
        {
            return IsFull == true ? Buffer : EncodeMessage();
        }
        public void Clear()
        {
            ServiceMsgLength = 0;
            Conv = 0;
            Snd_una = 0;
            Rcv_nxt = 0;
            Snd_nxt = 0;
            SN = 0;
            TS = 0;
            Cmd = UdpProtocol.NIL;
            OperationCode = 0;
            RecurCount = 0;
            ServiceMsg = null;
            IsFull = false;
        }
        public override string ToString()
        {
            string str = $"Length:{ServiceMsgLength} ; Conv:{Conv} ;Cmd:{Cmd};TS :{TS } ;  SN:{SN} ; Snd_una:{Snd_una} ; Snd_nxt :{Snd_nxt} ;Rcv_nxt:{Rcv_nxt} ; OperationCode : {OperationCode} ; RecurCount:{RecurCount} ";
            return str;
        }
        public static UdpNetMessage ConvertToACK(UdpNetMessage srcMsg)
        {
            UdpNetMessage ack = GameManager.GetModule<IReferencePoolManager>().Spawn<UdpNetMessage>();
            ack.Conv = srcMsg.Conv;
            ack.Snd_una = srcMsg.Snd_una;
            ack.SN = srcMsg.SN;
            ack.Cmd = UdpProtocol.ACK;
            ack.OperationCode = srcMsg.OperationCode;
            ack.RecurCount = 0;
            ack.EncodeMessage();
            return ack;
        }
        /// <summary>
        /// 生成心跳数据
        /// </summary>
        /// <param name="conv">会话ID</param>
        /// <returns></returns>
        public static UdpNetMessage HeartbeatMessage(long conv)
        {
            var udpNetMsg = GameManager.GetModule<IReferencePoolManager>().Spawn<UdpNetMessage>();
            udpNetMsg.Conv = conv;
            udpNetMsg.Cmd = UdpProtocol.MSG;
            udpNetMsg.ServiceMsgLength = 0;
            udpNetMsg.OperationCode = InnerOpCode._Heartbeat;
            return udpNetMsg;
        }
        public static UdpNetMessage EncodeMessage(long conv)
        {
            var udpNetMsg = GameManager.GetModule<IReferencePoolManager>().Spawn<UdpNetMessage>();
            udpNetMsg.Conv = conv;
            udpNetMsg.Cmd = UdpProtocol.MSG;
            udpNetMsg.ServiceMsgLength = 0;
            udpNetMsg.OperationCode = 0;
            return udpNetMsg;
        }
        /// <summary>
        /// 编码默认消息
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="message">二进制业务报文</param>
        /// <returns>编码成功后的数据</returns>
        public static UdpNetMessage EncodeMessage(ushort opCode, byte[] message)
        {
            var udpNetMsg = GameManager.GetModule<IReferencePoolManager>().Spawn<UdpNetMessage>();
            udpNetMsg.Cmd = UdpProtocol.MSG;
            udpNetMsg.ServiceMsgLength = 0;
            udpNetMsg.OperationCode = opCode;
            udpNetMsg.ServiceMsg = message;
            udpNetMsg.EncodeMessage();
            return udpNetMsg;
        }
        public static async Task<UdpNetMessage> EncodeMessageAsync(long conv)
        {
            return await Task.Run<UdpNetMessage>(() =>
            {
                return EncodeMessage(conv);
            });
        }
    }
}
