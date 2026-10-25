namespace Cosmos.Entity
{
    /// <summary>
    /// 逐帧刷新契约（对应 Update）；
    /// <para>实体实现本接口后即被纳入 EntityManager 的集中刷新；未实现的实体不进入刷新列表，
    /// 因此不会产生"空轮询"开销。</para>
    /// <para>刷新顺序 = 实体注册顺序 + 实例生成顺序；派发过程为一次接口调用，不含反射。</para>
    /// <para>注意：实现本接口后请勿再写 MonoBehaviour.Update 做同一件事，否则会绕过实体暂停与统一时序。</para>
    /// </summary>
    public interface IEntityTickable : IEntityObject
    {
        /// <summary>
        /// 逐帧刷新。
        /// </summary>
        /// <param name="deltaTime">本帧时间增量</param>
        void OnEntityTick(float deltaTime);
    }
    /// <summary>
    /// 固定帧刷新契约（对应 FixedUpdate）。
    /// </summary>
    public interface IEntityFixedTickable : IEntityObject
    {
        /// <summary>
        /// 固定帧刷新。
        /// </summary>
        /// <param name="deltaTime">固定帧时间增量</param>
        void OnEntityFixedTick(float deltaTime);
    }
    /// <summary>
    /// 帧末刷新契约（对应 LateUpdate），适合处理跟随 / 相机 / 需要在本帧逻辑之后结算的表现。
    /// </summary>
    public interface IEntityLateTickable : IEntityObject
    {
        /// <summary>
        /// 帧末刷新。
        /// </summary>
        /// <param name="deltaTime">本帧时间增量</param>
        void OnEntityLateTick(float deltaTime);
    }
}
