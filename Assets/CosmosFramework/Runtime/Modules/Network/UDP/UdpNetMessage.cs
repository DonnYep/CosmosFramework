using System;
using System.Collections.Generic;
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
        public ushort Len { get; private set; }
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
        ///通讯码，用于区分数据包含的是连接、断开、心跳、消息；
        ///<see cref="UdpHeader">
        /// </summary>
        public UdpHeader HeaderCode { get; private set; }
        /// <summary>
        /// 业务报文
        /// </summary>
        public byte[] ServiceData { get; set; }
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
            Len = 0;
            Conv = conv;
            Cmd = cmd;
        }
        /// <summary>
        /// MSG报文构造
        /// </summary>
        /// <param name="conv">会话ID</param>
        /// <param name="sn">msg序号</param>
        /// <param name="cmd">协议ID</param>
        /// <param name="headerCode">操作码</param>
        /// <param name="message">消息内容</param>
        public UdpNetMessage(long conv, uint sn, byte cmd, UdpHeader headerCode, byte[] message)
        {
            if (message == null)
                Len = 0;
            else
                Len = (ushort)message.Length;
            Conv = conv;
            SN = sn;
            Cmd = cmd;
            ServiceData = message;
            HeaderCode = headerCode;
        }
        /// <summary>
        /// ACK报文构造
        /// </summary>
        /// <param name="conv">会话ID</param>
        /// <param name="sn">msgID</param>
        /// <param name="cmd">协议</param>
        /// <param name="headerCode">前后端消息ID</param>
        public UdpNetMessage(long conv, uint sn, ushort cmd, UdpHeader headerCode)
        {
            Conv = conv;
            SN = sn;
            Cmd = cmd;
            HeaderCode = headerCode;
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
                Len = 0;
            else
                Len = (ushort)message.Length;
            Conv = udpNetMsg.Conv;
            SN = udpNetMsg.SN;
            Cmd = udpNetMsg.Cmd;
            ServiceData = message;
            HeaderCode = udpNetMsg.HeaderCode;
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
                Len = BitConverter.ToUInt16(buffer, 0);
                if (buffer.Length == Len + 38)
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
            HeaderCode = (UdpHeader)BitConverter.ToUInt16(buffer, 24);
            if (Cmd == UdpProtocol.MSG)
            {
                ServiceData = new byte[Len];
                Array.Copy(buffer, 26, ServiceData, 0, Len);
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
                Len = 0;
            int msgLen = 0;
            if (ServiceData != null)
            {
                msgLen = ServiceData.Length;
                Len = Convert.ToUInt16(msgLen);
            }
            byte[] data = new byte[38 + Len];
            byte[] len = BitConverter.GetBytes(Len);
            byte[] conv = BitConverter.GetBytes(Conv);
            byte[] cmd = BitConverter.GetBytes(Cmd);
            byte[] ts = BitConverter.GetBytes(TS);
            byte[] sn = BitConverter.GetBytes(SN);
            byte[] headerCode = BitConverter.GetBytes((ushort)HeaderCode);
            Array.Copy(len, 0, data, 0, 2);
            Array.Copy(conv, 0, data, 2, 8);
            Array.Copy(cmd, 0, data, 10, 2);
            Array.Copy(ts, 0, data, 12, 8);
            Array.Copy(sn, 0, data, 20, 4);
            Array.Copy(headerCode, 0, data, 24, 2);
            //如果不是ACK报文，则追加数据
            if (Cmd == UdpProtocol.MSG)
                if (ServiceData != null)//空包保护
                    Array.Copy(ServiceData, 0, data, 26, ServiceData.Length);
            Buffer = data;
            return data;
        }
        /// <summary>
        /// 获取编码后的数据buffer
        /// </summary>
        /// <returns>序列化后的buffer</returns>
        public byte[] GetBuffer()
        {
            return IsFull == true ? Buffer : EncodeMessage();
        }
        public void Release()
        {
            Len = 0;
            Conv = 0;
            SN = 0;
            TS = 0;
            Cmd = UdpProtocol.NIL;
            HeaderCode = 0;
            RecurCount = 0;
            ServiceData = null;
            IsFull = false;
        }
        public override string ToString()
        {
            string str = $"Length:{Len} ; Conv:{Conv} ;Cmd:{Cmd};TS :{TS } ;  SN:{SN} ;  HeaderCode: {HeaderCode} ; RecurCount:{RecurCount} ";
            return str;
        }
        public static UdpNetMessage ConvertToACK(UdpNetMessage srcMsg)
        {
            UdpNetMessage ack = ReferencePool.Accquire<UdpNetMessage>();
            ack.Conv = srcMsg.Conv;
            ack.SN = srcMsg.SN;
            ack.Cmd = UdpProtocol.ACK;
            ack.HeaderCode = srcMsg.HeaderCode;
            ack.RecurCount = 0;
            ack.EncodeMessage();
            return ack;
        }
        /// <summary>
        /// 生成心跳数据
        /// </summary>
        /// <param name="conv">会话ID</param>
        /// <returns></returns>
        public static UdpNetMessage HeartbeatMessageC2S(long conv)
        {
            var udpNetMsg = ReferencePool.Accquire<UdpNetMessage>();
            udpNetMsg.Conv = conv;
            udpNetMsg.Cmd = UdpProtocol.MSG;
            udpNetMsg.Len = 0;
            udpNetMsg.HeaderCode = UdpHeader.Hearbeat;
            return udpNetMsg;
        }
        public static UdpNetMessage EncodeMessage(long conv)
        {
            var udpNetMsg = ReferencePool.Accquire<UdpNetMessage>();
            udpNetMsg.Conv = conv;
            udpNetMsg.Cmd = UdpProtocol.MSG;
            udpNetMsg.Len = 0;
            udpNetMsg.HeaderCode = UdpHeader.Message;
            return udpNetMsg;
        }
        /// <summary>
        /// 编码默认消息
        /// </summary>
        /// <param name="headerCode">操作码</param>
        /// <param name="message">二进制业务报文</param>
        /// <returns>编码成功后的数据</returns>
        public static UdpNetMessage EncodeMessage(UdpHeader headerCode, byte[] message)
        {
            var udpNetMsg = ReferencePool.Accquire<UdpNetMessage>();
            udpNetMsg.Cmd = UdpProtocol.MSG;
            udpNetMsg.Len = 0;
            udpNetMsg.HeaderCode = headerCode;
            udpNetMsg.ServiceData = message;
            udpNetMsg.EncodeMessage();
            return udpNetMsg;
        }
        /// <summary>
        ///普通消息；MSG 
        /// </summary>
        public static UdpNetMessage EncodeMessage(long conv, byte[] message)
        {
            var udpNetMsg = ReferencePool.Accquire<UdpNetMessage>();
            udpNetMsg.Conv = conv;
            udpNetMsg.Cmd = UdpProtocol.MSG;
            udpNetMsg.Len = 0;
            udpNetMsg.HeaderCode = UdpHeader.Message;
            udpNetMsg.ServiceData = message;
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
        public static async Task<UdpNetMessage> EncodeMessageAsync(long conv, byte[] message)
        {
            return await Task.Run<UdpNetMessage>(() =>
            {
                return EncodeMessage(conv, message);
            });
        }
    }
}
