namespace Cosmos
{
    /// <summary>
    /// 实现此接口的对象，可以被托管的管理器轮询更新
    /// 具体更新区域为Update
    /// </summary>
    public interface  IRefreshable
    {
        /// <summary>
        /// 更新接口
        /// </summary>
        void OnRefresh();
    }
}