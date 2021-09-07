using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class TcpNetMessage : INetworkMessage, IReference
    {
        public long Conv { get; set; }
        public int ServiceMsgLength { get; private set; }
        public byte[] ServiceData { get; private set; }
        /// <summary>
        /// 前后端通讯的操作码；
        /// </summary>
        public ushort OperationCode { get; set; }
        /// <summary>
        /// 是否是完整报文
        /// </summary>
        public bool IsFull { get; private set; }
        public void Release()
        {
        }
        public  bool DecodeMessage(byte[] buffer)
        {
            if (buffer == null)
            {
                IsFull = false;
                return false;
            }
            if (buffer.Length >= 4)
            {
                ServiceMsgLength = BitConverter.ToInt32(buffer, 0);
                if (buffer.Length == ServiceMsgLength)
                {
                    IsFull = true;
                }
            }
            else
            {
                IsFull = false;
                return false;
            }
            Conv = BitConverter.ToInt64(buffer, 4);
            OperationCode = BitConverter.ToUInt16(buffer, 12);
            Array.Copy(buffer, 14, ServiceData, 0, ServiceMsgLength);
            return true;
        }
        public byte[] EncodeMessage()
        {
            int srvMsgLen = ServiceData.Length;
            byte[] data = new byte[14 + srvMsgLen];
            var len= BitConverter.GetBytes(srvMsgLen);
            var conv = BitConverter.GetBytes(Conv);
            var opCode= BitConverter.GetBytes(OperationCode);
            Array.Copy(len, 0, data, 0, 4);
            Array.Copy(conv, 0, data, 4, 8);
            Array.Copy(opCode, 0, data, 12, 2);
            Array.Copy(ServiceData, 0, data, 14, srvMsgLen);
            return data;
        }
        public byte[] GetBuffer()
        {
            return EncodeMessage();
        }
    }
}
