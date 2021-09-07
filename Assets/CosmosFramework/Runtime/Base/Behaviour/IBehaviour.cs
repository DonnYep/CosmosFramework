namespace Cosmos
{
    /// <summary>
    /// 生命周期接口
    /// </summary>
   public interface  IBehaviour
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 终止
        /// </summary>
        void OnTermination();
    }
}
