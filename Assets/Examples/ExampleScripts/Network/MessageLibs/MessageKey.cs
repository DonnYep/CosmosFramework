using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos.Test
{
    public struct MessageKey
    {
        readonly byte messageCode;
        readonly ushort messageSubCode;
        /// <summary>
        /// 消息主码；
        /// </summary>
        public byte MessageCode { get { return messageCode; } }
        /// <summary>
        /// 消息操作子码；
        /// </summary>
        public ushort MessageSubCode { get { return messageSubCode; } }
        public MessageKey(byte messageCode, ushort messageSubCode)
        {
            this.messageCode = messageCode;
            this.messageSubCode = messageSubCode;
        }
        public bool Equals(MessageKey other)
        {
            return messageCode == other.messageCode && messageSubCode == other.messageSubCode;
        }
        public static bool operator ==(MessageKey a, MessageKey b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(MessageKey a, MessageKey b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return obj is MessageKey && Equals((MessageKey)obj);
        }
        public override int GetHashCode()
        {
            return messageCode.GetHashCode() ^ messageSubCode.GetHashCode();
        }
        public override string ToString()
        {
            return $"MessageCode : {MessageCode} ; MessageSubCode : {MessageSubCode}";
        }
    }
}
