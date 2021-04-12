using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// 网络通讯用数据模型；
/// </summary>
namespace Cosmos.Test
{
    [Serializable]
    public class OperationData : IDisposable
    {
        public byte OperationCode { get; set; }
        public ushort SubOperationCode { get; set; }
        public object DataMessage { get; set; }
        public short ReturnCode { get; set; }
        public OperationData() { }
        public OperationData(byte operationCode)
        {
            OperationCode = operationCode;
        }
        public void Dispose()
        {
            OperationCode = 0;
            DataMessage = null;
            ReturnCode = 0;
            SubOperationCode = 0;
        }
    }
}