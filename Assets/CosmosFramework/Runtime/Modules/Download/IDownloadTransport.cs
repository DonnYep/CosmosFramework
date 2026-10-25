namespace Cosmos.Download
{
    /// <summary>
    /// 一次传输（= 一次 HTTP 请求）的抽象；由调度器每帧调用 <see cref="Tick"/> 推进。
    /// <para>之所以做成"轮询式"而不是回调式：调度器的状态机本来就按帧推进，
    /// 轮询能保证状态迁移只发生在一个地方（不会出现回调在传输层线程/时机里乱入）。</para>
    /// </summary>
    public interface IDownloadTransfer
    {
        /// <summary>
        /// 推进传输（Unity 实现里检查请求是否完成）
        /// </summary>
        void Tick();
        /// <summary>
        /// 传输状态
        /// </summary>
        DownloadTransferState State { get; }
        /// <summary>
        /// 本次传输已写入的字节数（不含断点偏移）
        /// </summary>
        long DownloadedBytes { get; }
        /// <summary>
        /// 文件总字节数，-1 表示未知
        /// </summary>
        long TotalBytes { get; }
        /// <summary>
        /// HTTP 响应码；尚未收到响应时为 0
        /// </summary>
        int ResponseCode { get; }
        /// <summary>
        /// 错误信息
        /// </summary>
        string Error { get; }
        /// <summary>
        /// 取消传输
        /// </summary>
        void Cancel();
    }
    /// <summary>
    /// 传输状态
    /// </summary>
    public enum DownloadTransferState : byte
    {
        /// <summary>
        /// 传输中
        /// </summary>
        Running = 0,
        /// <summary>
        /// 成功（HTTP 无错误且已写完）
        /// </summary>
        Succeeded = 1,
        /// <summary>
        /// 失败
        /// </summary>
        Failed = 2,
        /// <summary>
        /// 已取消
        /// </summary>
        Canceled = 3,
    }
    /// <summary>
    /// 传输请求参数
    /// </summary>
    public struct DownloadTransferRequest
    {
        /// <summary>
        /// 资源地址
        /// </summary>
        public string DownloadUrl { get; set; }
        /// <summary>
        /// 临时文件路径（数据写入这里）
        /// </summary>
        public string TempPath { get; set; }
        /// <summary>
        /// 起始字节偏移，&gt;0 时要求服务端返回 206
        /// </summary>
        public long StartOffset { get; set; }
        /// <summary>
        /// 超时时间（秒），&lt;=0 表示不限
        /// </summary>
        public float TimeoutSeconds { get; set; }
        /// <summary>
        /// 优先级（供实现层参考，例如接入自定义连接池时使用）
        /// </summary>
        public int Priority { get; set; }
    }
    /// <summary>
    /// 下载传输层工厂：把"怎么下载"与"怎么排队/怎么续传"解耦。
    /// <para>默认实现是 <see cref="UnityWebRequestDownloadTransport"/>；替换实现即可换 HTTP 栈（如 HttpClient / 自定义 CDN SDK）。</para>
    /// </summary>
    public interface IDownloadTransport
    {
        /// <summary>
        /// 创建一次传输（实现方应立即开始请求）
        /// </summary>
        /// <param name="request">传输参数</param>
        /// <returns>传输对象</returns>
        IDownloadTransfer CreateTransfer(DownloadTransferRequest request);
    }
}
