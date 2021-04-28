namespace Cosmos
{
    /// <summary>
    /// 可重置接口；用于反复使用的数据类型；
    /// </summary>
    public interface IRenewable
    {
        /// <summary>
        /// 重置状态
        /// </summary>
        void OnRenewal();
    }
}
