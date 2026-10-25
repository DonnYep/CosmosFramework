namespace Cosmos.Download
{
    /// <summary>
    /// 下载写入模式
    /// </summary>
    public enum DownloadMode : byte
    {
        /// <summary>
        /// 覆盖：丢弃本地临时文件，从第 0 字节重下
        /// </summary>
        Overwrite = 0,
        /// <summary>
        /// 断点续传：本地已有临时文件则从文件长度继续；服务端不支持 Range 时自动丢弃并从 0 重下
        /// </summary>
        ResumeIfPossible = 1,
        /// <summary>
        /// 追加：从 <see cref="DownloadOptions.ByteOffset"/> 指定的位置继续（调用方自行保证语义）
        /// </summary>
        Append = 2,
    }
}
