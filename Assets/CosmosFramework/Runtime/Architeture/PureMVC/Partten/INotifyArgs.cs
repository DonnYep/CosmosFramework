using System;
namespace PureMVC
{
    public interface INotifyArgs
    {
        /// <summary>
        /// 通知&事件名；
        /// 记录为EventKey；
        /// </summary>
        string NotifyName { get; }
        /// <summary>
        /// 通知的数据；
        /// </summary>
        object NotifyData { get; }
        /// <summary>
        /// 消息发送者；
        /// </summary>
        object NotifySender { get; }
        string ToString();
    }
}
