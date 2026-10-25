namespace Cosmos.Entity
{
    /// <summary>
    /// 实体实例契约；
    /// <para>Entity 模块统一管理实体的生命周期与刷新，载体有两种：</para>
    /// <para>1、<see cref="EntityObject"/>：持有 GameObject 的实体，用于表现（模型、特效、挂点）；</para>
    /// <para>2、<see cref="EntityLogic"/>：不持有 GameObject 的纯逻辑实体，用于逻辑帧、服务器算、
    /// 以及任何不存在于场景中的对象。</para>
    /// <para>两者共用同一套注册 / 生成 / 隐藏 / 回收 / 分组冻结流程，刷新由 EntityManager 集中派发。</para>
    /// </summary>
    public interface IEntityObject
    {
        /// <summary>
        /// 实体实例编号；在同一实体（同名注册）内部唯一。
        /// </summary>
        int EntityObjectId { get; }
        /// <summary>
        /// 实体名（注册名）。
        /// </summary>
        string EntityName { get; }
        /// <summary>
        /// 实体组名。
        /// </summary>
        string EntityGroupName { get; }
        /// <summary>
        /// 是否暂停刷新；组冻结会叠加到个体状态上（个体暂停与组冻结互相独立）。
        /// </summary>
        bool Paused { get; set; }
        /// <summary>
        /// 设置组冻结状态；由 <see cref="IEntityManager"/> 维护，业务侧不要直接调用。
        /// </summary>
        /// <param name="paused">是否冻结</param>
        void SetGroupPaused(bool paused);
        /// <summary>
        /// 实体初始化；仅在生成新实例时调用一次（池中复用不会重复调用）。
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <param name="entityObjectId">实体实例编号</param>
        /// <param name="entityGroupName">实体组名</param>
        void OnInit(string entityName, int entityObjectId, string entityGroupName);
        /// <summary>
        /// 实体显示。
        /// </summary>
        void OnShow();
        /// <summary>
        /// 实体隐藏。
        /// </summary>
        void OnHide();
        /// <summary>
        /// 实体回收（进入池）；用于清理一次性状态。
        /// </summary>
        void OnRecycle();
    }
}
