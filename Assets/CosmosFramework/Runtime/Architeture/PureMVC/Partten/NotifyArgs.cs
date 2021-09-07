namespace PureMVC
{
    public class NotifyArgs : INotifyArgs
    {
        /// <summary>
        /// 通知&事件名；
        /// 记录为EventKey；
        /// </summary>
        public string NotifyName { get; private set; }
        /// <summary>
        /// 通知的数据；
        /// </summary>
        public object NotifyData { get; private set; }
        /// <summary>
        /// 消息发送者；
        /// (非重要，可以不设置！)
        /// </summary>
        public object NotifySender { get; private set; }
        public NotifyArgs(string notifyName) : this(notifyName, null) { }
        public NotifyArgs(string notifyName, object notifyData):this(notifyName,notifyData,null){}
        public NotifyArgs(string notifyName, object notifyData,object notifySender)
        {
            this.NotifyName = notifyName;
            this.NotifyData = notifyData;
            this.NotifySender = notifySender;
        }
        public override string ToString()
        {
            return $"NotifyName : {NotifyName}\nNotifyData : {(NotifyData!=null?NotifyData:"NULL")}\nNotifySender : {(NotifySender!=null?NotifySender:"NULL")}";
        }
    }
}
