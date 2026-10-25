namespace Cosmos.Entity
{
    /// <summary>
    /// 实体暂停状态；
    /// <para>个体暂停与组冻结分开记录，最终状态是两者的并集：组冻结不会覆盖个体暂停，
    /// 解冻组时也不会把个体主动暂停的实体一起恢复。</para>
    /// </summary>
    internal struct EntityPauseState
    {
        bool paused;
        bool groupPaused;
        /// <summary>
        /// 是否处于暂停状态（个体暂停 或 组冻结）。
        /// </summary>
        public bool Paused
        {
            get { return paused || groupPaused; }
            set { paused = value; }
        }
        /// <summary>
        /// 设置组冻结状态。
        /// </summary>
        /// <param name="value">是否冻结</param>
        public void SetGroupPaused(bool value)
        {
            groupPaused = value;
        }
        /// <summary>
        /// 清空两种暂停状态；回收进池时调用，避免复用后继承上一次的暂停状态。
        /// </summary>
        public void Reset()
        {
            paused = false;
            groupPaused = false;
        }
    }
}
