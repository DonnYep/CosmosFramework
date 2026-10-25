namespace Cosmos.Entity
{
    /// <summary>
    /// 不持有 GameObject 的纯逻辑实体基类；
    /// <para>用于逻辑帧 / 帧同步回放 / 服务器算 / 纯数据与 AI 计算等不需要场景对象的对象。</para>
    /// <para>注册方式：<see cref="IEntityManager.RegisterLogicEntity{T}"/>；之后与 GameObject 实体
    /// 完全同权：可生成、可隐藏、可回收、可按组冻结。</para>
    /// <para>需要刷新时再实现 <see cref="IEntityTickable"/> / <see cref="IEntityFixedTickable"/> /
    /// <see cref="IEntityLateTickable"/> 之一，未实现的不会有任何刷新开销。</para>
    /// </summary>
    public abstract class EntityLogic : IEntityObject
    {
        EntityPauseState pauseState;
        /// <summary>
        /// 实体实例编号；在同一实体（同名注册）内部唯一。
        /// </summary>
        public int EntityObjectId { get; private set; }
        /// <summary>
        /// 实体名（注册名）。
        /// </summary>
        public string EntityName { get; private set; }
        /// <summary>
        /// 实体组名。
        /// </summary>
        public string EntityGroupName { get; private set; }
        /// <summary>
        /// 是否暂停刷新；组冻结会叠加到个体状态上。
        /// </summary>
        public bool Paused
        {
            get { return pauseState.Paused; }
            set { pauseState.Paused = value; }
        }
        /// <inheritdoc/>
        public void SetGroupPaused(bool paused)
        {
            pauseState.SetGroupPaused(paused);
        }
        /// <summary>
        /// 实体初始化；仅在生成新实例时调用一次。
        /// </summary>
        public virtual void OnInit(string entityName, int entityObjectId, string entityGroupName)
        {
            EntityName = entityName;
            EntityObjectId = entityObjectId;
            EntityGroupName = entityGroupName;
        }
        /// <summary>
        /// 实体显示。
        /// </summary>
        public virtual void OnShow() { }
        /// <summary>
        /// 实体隐藏。
        /// </summary>
        public virtual void OnHide() { }
        /// <summary>
        /// 实体回收（进入池）。
        /// </summary>
        public virtual void OnRecycle()
        {
            pauseState.Reset();
        }
    }
}
