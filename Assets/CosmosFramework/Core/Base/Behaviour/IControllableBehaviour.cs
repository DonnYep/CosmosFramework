namespace Cosmos
{
    /// <summary>
    /// 完整可控的生命周期
    /// </summary>
    public interface IControllableBehaviour : IBehaviour,IControllable,IRefreshable
    {
        /// <summary>
        /// 模块准备工作，在OnInitialization()函数之后执行
        /// Start方法中被调用
        /// </summary>
        void OnPreparatory();
    }
}