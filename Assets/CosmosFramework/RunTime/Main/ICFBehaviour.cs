namespace Cosmos
{
    /// <summary>
    /// 组件生命周期接口
    /// 初始化，暂停，恢复暂停，终止
    /// CosmosFrameworkBehaviour
    /// CFBehaviour
    /// </summary>
    public interface ICFBehaviour
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 暂停
        /// </summary>
        void OnPause();
        /// <summary>
        /// 恢复暂停
        /// </summary>
        void OnUnPause();
        /// <summary>
        /// 终止
        /// </summary>
        void OnTermination();
    }
}