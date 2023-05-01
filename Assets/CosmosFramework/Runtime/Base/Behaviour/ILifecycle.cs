namespace Cosmos
{
    public interface ILifecycle
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 模块准备工作，在OnInitialization()函数之后执行
        /// Start方法中被调用
        /// </summary>
        void OnPreparatory();
        /// <summary>
        /// 开启
        /// </summary>
        void OnActive();
        /// <summary>
        /// 暂停
        /// </summary>
        void OnPause();
        /// <summary>
        /// 恢复暂停
        /// </summary>
        void OnUnPause();
        /// <summary>
        /// 时间流逝轮询;
        /// </summary>
        /// <param name="msNow">utc毫秒当前时间</param>
        void OnElapseRefresh(long msNow);
        /// <summary>
        /// 更新接口
        /// </summary>
        void OnRefresh();
        /// <summary>
        /// 关闭
        /// </summary>
        void OnDeactive();
        /// <summary>
        /// 被回收
        /// </summary>
        void OnRecycle();
        /// <summary>
        /// 重置状态
        /// </summary>
        void OnRenewal();
        /// <summary>
        /// 终止
        /// </summary>
        void OnTermination();
    }
}
