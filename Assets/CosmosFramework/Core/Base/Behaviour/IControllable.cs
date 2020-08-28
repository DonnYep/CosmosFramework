namespace Cosmos
{
    /// <summary>
    /// 可控生命周期接口，
    /// 能够暂停、恢复
    /// </summary>
    public interface IControllable
    {
        bool IsPause { get; }
        /// <summary>
        /// 暂停
        /// </summary>
        void OnPause();
        /// <summary>
        /// 恢复暂停
        /// </summary>
        void OnUnPause();
    }
}
