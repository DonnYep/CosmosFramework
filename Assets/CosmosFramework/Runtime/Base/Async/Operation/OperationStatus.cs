namespace Cosmos
{
    /// <summary>
    /// 异步操作状态。
    /// </summary>
    public enum OperationStatus
    {
        /// <summary>
        /// 未开始
        /// </summary>
        None,
        /// <summary>
        /// 进行中
        /// </summary>
        Processing,
        /// <summary>
        /// 成功
        /// </summary>
        Succeeded,
        /// <summary>
        /// 失败
        /// </summary>
        Failed
    }
}
