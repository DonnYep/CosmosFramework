namespace Cosmos.DataTable
{
    /// <summary>
    /// 数据表状态
    /// </summary>
    public enum DataTableState : byte
    {
        /// <summary>
        /// 未加载
        /// </summary>
        None = 0,
        /// <summary>
        /// 加载中
        /// </summary>
        Loading = 1,
        /// <summary>
        /// 已加载（数据只读可用）
        /// </summary>
        Loaded = 2,
        /// <summary>
        /// 加载或解析失败（失败原因见 DataTableBase.Error）
        /// </summary>
        Failed = 3,
    }
}
