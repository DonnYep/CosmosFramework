using System;
namespace Cosmos.Test
{
    /// <summary>
    /// Message消息处理基类；
    /// </summary>
    public abstract class MessageCommand
    {
        /// <summary>
        /// 消息Key;
        /// </summary>
        public MessageKey MessageKey { get; private set; }
        /// <summary>
        /// 发送消息委托；
        /// </summary>
        public Action<MessageData> SendMessage{ get; set; }
        /// <summary>
        /// 基类构造；
        /// </summary>
        /// <param name="messageCode">操作主码</param>
        /// <param name="messageSubCode">操作子码</param>
        protected MessageCommand(byte messageCode,ushort messageSubCode)
        {
            MessageKey = new MessageKey(messageCode, messageSubCode);
        }
        /// <summary>
        /// 收到网关消息；
        /// </summary>
        /// <param name="message">传输数据</param>
        public abstract void OnMessage(MessageData message);
    }
}
