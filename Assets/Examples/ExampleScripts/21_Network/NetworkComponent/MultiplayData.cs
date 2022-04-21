using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
namespace Cosmos.Test
{
    [Serializable]
    public class MultiplayData : IDisposable
    {
        public byte OperationCode { get; set; }
        public ushort SubOperationCode { get; set; }
        public object DataContract { get; set; }
        public short ReturnCode { get; set; }
        public MultiplayData() { }
        public MultiplayData(byte operationCode)
        {
            OperationCode = operationCode;
        }
        public void Dispose()
        {
            OperationCode = 0;
            DataContract = null;
            ReturnCode = 0;
            SubOperationCode = 0;
        }
    }
}