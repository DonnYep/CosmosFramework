using System;
namespace Cosmos.Test
{
    public struct MessageData
    {
        /// <summary>
        /// 消息操作码；
        /// </summary>
        public byte MessageCode { get; set; }
        /// <summary>
        /// 消息子码
        /// </summary>
        public ushort MessageSubCode { get; set; }
        /// <summary>
        /// 消息返回码；
        /// </summary>
        public byte ReturnCode { get; set; }
        /// <summary>
        /// 消息体的类类型；
        /// </summary>
        public ushort MessageBytesType { get; set; }
        /// <summary>
        /// 消息体；
        /// </summary>
        public byte[] MessageBytes { get; set; }
        public MessageData(byte messageCode, ushort messageSubCode, byte returnCode, ushort messageBytesType, byte[] messageBytes)
        {
             MessageCode = messageCode;
            MessageSubCode = messageSubCode;
            ReturnCode = returnCode;
            MessageBytesType = messageBytesType;
            MessageBytes = messageBytes;
        }
        public static byte[] Serialize(MessageData messageData)
        {
            var opCode = new byte[1];
            opCode[0] = messageData.MessageCode;
            var subOpCode = BitConverter.GetBytes(messageData.MessageSubCode);
            var returnCode = new byte[1];
            returnCode[0] = messageData.ReturnCode;
            var msgDataType = BitConverter.GetBytes(messageData.MessageBytesType);
            var msgBytes = messageData.MessageBytes;
            var data = new byte[msgBytes.Length + 6];
            Array.Copy(opCode, 0, data, 0, 1);
            Array.Copy(subOpCode, 0, data, 1, 2);
            Array.Copy(returnCode, 0, data, 3, 1);
            Array.Copy(msgDataType, 0, data, 4, 2);
            Array.Copy(msgBytes, 0, data, 6, msgBytes.Length);
            return data;
        }
        public static MessageData Deserialize(byte[] data)
        {
            var opCodeArr = new byte[1];
            var subOpCodeArr = new byte[2];
            var returnCodeArr = new byte[1];
            var mpdataTypeArr = new byte[2];
            var mpdata = new byte[data.Length - 6];
            Array.Copy(data, 0, opCodeArr, 0, 1);
            Array.Copy(data, 1, subOpCodeArr, 0, 2);
            Array.Copy(data, 3, returnCodeArr, 0, 1);
            Array.Copy(data, 6, mpdataTypeArr, 0, 2);
            Array.Copy(data, 6, mpdata, 0, mpdata.Length);
            var opcode = opCodeArr[0];
            var subOpCode = BitConverter.ToUInt16(subOpCodeArr, 0);
            var returnCode = returnCodeArr[0];
            var mpdataType = BitConverter.ToUInt16(mpdataTypeArr, 0);
            return new MessageData(opcode, subOpCode, returnCode, mpdataType, mpdata);
        }
    }
}
