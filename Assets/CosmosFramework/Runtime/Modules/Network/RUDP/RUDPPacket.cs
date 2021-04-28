using System;
using System.Linq;
using System.Net;
using System.Text;
namespace RUDP
{
    public class RUDPPacket
    {
        public IPEndPoint Src { get; set; }
        public IPEndPoint Dst { get; set; }
        public DateTime Sent { get; set; }
        public DateTime Received { get; set; }
        public bool Retransmit { get; set; }

        public int Seq { get; set; }
        public int Id { get; set; }
        public int Qty { get; set; }
        public RUDPPacketType Type { get; set; }
        public RUDPPacketFlags Flags { get; set; }
        public byte[] Data { get; set; }
        public static RUDPPacket Deserialize(byte[] header, byte[] data)
        {
            var buffer = data.Skip(header.Length).ToArray();
            var rudpPacket = new RUDPPacket();
            rudpPacket.Seq = BitConverter.ToInt32(buffer, 0);
            rudpPacket.Id = BitConverter.ToInt32(buffer, 4);
            rudpPacket.Qty = BitConverter.ToInt32(buffer, 8);
            rudpPacket.Type = (RUDPPacketType)buffer[13];
            rudpPacket.Flags = (RUDPPacketFlags)buffer[14];
            Array.Copy(buffer, 14, rudpPacket.Data, 0, data.Length - 14);
            return rudpPacket;
        }

        public byte[] ToByteArray(byte[] header)
        {
            int dataLen = 0;
            if (Data == null || Data.Length == 0)
                dataLen = 0;
            else
                dataLen = Data.Length;
            byte[] buffer = new byte[header.Length + 14 + dataLen];
            var seq = BitConverter.GetBytes(Seq);
            var id = BitConverter.GetBytes(Id);
            var qty = BitConverter.GetBytes(Qty);
            Array.Copy(seq, 0, buffer, 0, 4);
            Array.Copy(id, 0, buffer, 4, 4);
            Array.Copy(qty, 0, buffer, 8, 4);
            buffer[13] = (byte)Type;
            buffer[14] = (byte)Flags;
            if (Data != null)
                Array.Copy(Data, 0, buffer, 14, dataLen);
            return buffer;
        }
        public override string ToString()
        {
            var str = $"Seq:{Seq} ; Id : {Id} ; Qty : {Qty}; Type:{Type};Flags:{Flags};Data.Length:{(Data == null ? 0 : Data.Length)}";
            return str;
        }
    }
}
