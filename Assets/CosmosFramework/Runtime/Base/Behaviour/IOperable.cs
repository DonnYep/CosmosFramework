namespace Cosmos
{
    /// <summary>
    /// 此接口为对象提供了开启与关闭的功能
    /// </summary>
    public interface IOperable
    {
        /// <summary>
        /// 开启
        /// </summary>
        void OnActive();
        /// <summary>
        /// 关闭
        /// </summary>
        void OnDeactive();
    }
}