using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
namespace Cosmos.Lockstep
{
    [Serializable]
    public struct MultiplayData
    {
        public byte OperationCode { get; set; }
        public ushort SubOperationCode { get; set; }
        public byte ReturnCode { get; set; }
        public byte[] DataContract { get; set; }
        public MultiplayData(byte operationCode, ushort subOperationCode, byte returnCode, byte[] dataContract)
        {
            OperationCode = operationCode;
            SubOperationCode = subOperationCode;
            ReturnCode = returnCode;
            DataContract = dataContract;
        }
        public static byte[] Serialize(MultiplayData multiplayData)
        {
            var opCode = new byte[1];
            opCode[0] = multiplayData.OperationCode;
            var subOpCode = BitConverter.GetBytes(multiplayData.SubOperationCode);
            var returnCode = new byte[1];
            returnCode[0] = multiplayData.ReturnCode;
            var msgBytes = multiplayData.DataContract;
            var data = new byte[msgBytes.Length + 4];
            Array.Copy(opCode, 0, data, 0, 1);
            Array.Copy(subOpCode, 0, data, 1, 2);
            Array.Copy(returnCode, 0, data, 3, 1);
            Array.Copy(msgBytes, 0, data, 4, msgBytes.Length);
            return data;
        }
        public static MultiplayData Deserialize(byte[] data)
        {
            var opCodeArr = new byte[1];
            var subOpCodeArr = new byte[2];
            var returnCodeArr = new byte[1];
            var mpdata = new byte[data.Length - 4];
            Array.Copy(data, 0, opCodeArr, 0, 1);
            Array.Copy(data, 1, subOpCodeArr, 0, 2);
            Array.Copy(data, 3, returnCodeArr, 0, 1);
            Array.Copy(data, 4, mpdata, 0, mpdata.Length);
            var opcode = opCodeArr[0];
            var subOpCode = BitConverter.ToUInt16(subOpCodeArr, 0);
            var returnCode = returnCodeArr[0];
            return new MultiplayData(opcode, subOpCode, returnCode, mpdata);
        }
    }
}