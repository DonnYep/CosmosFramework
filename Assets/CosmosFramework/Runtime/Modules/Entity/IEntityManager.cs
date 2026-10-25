using System;
namespace Cosmos.Entity
{
    //================================================
    /*
     * 实体模块：框架内唯一负责"对象存活与刷新"的模块。
     *
     * 1、两种载体，同一套流程：
     *    - EntityObject（持有 GameObject，用于表现）；
     *    - EntityLogic （不持有 GameObject，用于纯逻辑）。
     *
     * 2、实例实现刷新接口才会被轮询：
     *    IEntityTickable / IEntityFixedTickable / IEntityLateTickable；
     *    未实现的无轮询开销，派发不含反射。
     *
     * 3、实例生成时拥有唯一 Id（同一实体内唯一），可归属组别；
     *    组可整体冻结 / 解冻，与个体暂停（IEntityObject.Paused）互相独立。
     *
     * 4、实体由模块注册 / 生成 / 隐藏 / 回收，业务侧不要自行 Destroy 实体对象。
     */
    //================================================
    public interface IEntityManager : IModuleManager
    {
        /// <summary>
        /// 已注册实体数量；
        /// </summary>
        int EntityCount { get; }
        /// <summary>
        /// 实体组数量；
        /// </summary>
        int EntityGroupCount { get; }
        /// <summary>
        /// 当前存活的实体实例总数（两种载体合计）；
        /// </summary>
        int EntityObjectCount { get; }
        /// <summary>
        /// 实体注册成功事件
        /// </summary>
        event Action<EntityRegisterSuccessEventArgs> EntityRegisterSuccess;
        /// <summary>
        /// 实体注册失败事件；
        /// </summary>
        event Action<EntityRegisterFailureEventArgs> EntityRegisterFailure;
        /// <summary>
        /// 设置实体帮助体；
        /// </summary>
        /// <param name="entityHelper">自定义实现的实体帮助体</param>
        void SetEntityHelper(IEntityHelper entityHelper);
        /// <summary>
        /// 注册预制体实体（GameObject 载体）；
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="entityAssetInfo">实体资源信息</param>
        void RegisterEntityAsync<T>(EntityAssetInfo entityAssetInfo) where T : EntityObject;
        /// <summary>
        /// 注册纯逻辑实体（不持有 GameObject）；
        /// <para>不加载任何资源，立即注册成功；之后与预制体实体同权：
        /// 可生成、可隐藏、可回收、可按组冻结，需要刷新时实现对应刷新接口。</para>
        /// </summary>
        /// <typeparam name="T">逻辑实体类型</typeparam>
        /// <param name="entityName">实体名（注册名）</param>
        /// <param name="entityGroupName">实体组名</param>
        void RegisterLogicEntity<T>(string entityName, string entityGroupName = "") where T : EntityLogic, new();
        /// <summary>
        /// 注销实体；其存活实例与池内实例一并释放。
        /// </summary>
        /// <param name="entityName">实体名</param>
        void DeregisterEntity(string entityName);
        /// <summary>
        /// 是否存在实体；
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <returns>存在结果</returns>
        bool HasEntity(string entityName);
        /// <summary>
        /// 是否存在实体组；
        /// </summary>
        /// <param name="entityGroupName">实体组名</param>
        /// <returns>存在结果</returns>
        bool HasEntityGroup(string entityGroupName);
        /// <summary>
        /// 是否存在指定实体实例；
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <param name="entityObjectId">实体实例编号</param>
        /// <returns>存在结果</returns>
        bool HasEntityObject(string entityName, int entityObjectId);
        /// <summary>
        /// 获取指定实体实例；
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <param name="entityObjectId">实体实例编号</param>
        /// <param name="entityObject">返回的实体实例</param>
        /// <returns>是否存在</returns>
        bool GetEntityObject(string entityName, int entityObjectId, out IEntityObject entityObject);
        /// <summary>
        /// 生成一个实体实例（两种载体通用）；
        /// <para>未注册的实体名会抛出异常，避免字符串写错时静默失败。</para>
        /// </summary>
        /// <typeparam name="T">期望的载体类型</typeparam>
        /// <param name="entityName">实体名</param>
        /// <returns>实体实例</returns>
        T ShowEntity<T>(string entityName) where T : class, IEntityObject;
        /// <summary>
        /// 隐藏指定实体实例；
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <param name="entityObjectId">实体实例编号</param>
        void HideEntityObject(string entityName, int entityObjectId);
        /// <summary>
        /// 隐藏指定实体实例；
        /// </summary>
        /// <param name="entityObject">实体实例</param>
        void HideEntityObject(IEntityObject entityObject);
        /// <summary>
        /// 隐藏某个实体下的所有存活实例；
        /// </summary>
        /// <param name="entityName">实体名</param>
        void HideEntities(string entityName);
        /// <summary>
        /// 为实体设置组别；已生成实例的组标签在下次生成时生效，冻结状态立即同步。
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <param name="entityGroupName">实体组名</param>
        void SetEntityGroup(string entityName, string entityGroupName);
        /// <summary>
        /// 生成实体组中的所有实体（每个实体生成一个实例）；
        /// </summary>
        /// <param name="entityGroupName">实体组名</param>
        void ShowGroupEntities(string entityGroupName);
        /// <summary>
        /// 隐藏实体组中所有实体的存活实例；
        /// </summary>
        /// <param name="entityGroupName">实体组名</param>
        void HideGroupEntities(string entityGroupName);
        /// <summary>
        /// 离开实体组；
        /// </summary>
        /// <param name="entityName">实体名</param>
        void LeaveEntityGroup(string entityName);
        /// <summary>
        /// 解散实体组；
        /// </summary>
        /// <param name="entityGroupName">实体组名</param>
        void DissolveEntityGroup(string entityGroupName);
        /// <summary>
        /// 冻结实体组：组内所有存活实例停止刷新，且冻结期间新生成的实例同样冻结；
        /// <para>与个体暂停（IEntityObject.Paused）互相独立，解冻不会覆盖个体暂停。</para>
        /// </summary>
        /// <param name="entityGroupName">实体组名</param>
        void PauseEntityGroup(string entityGroupName);
        /// <summary>
        /// 解冻实体组；
        /// </summary>
        /// <param name="entityGroupName">实体组名</param>
        void UnPauseEntityGroup(string entityGroupName);
        /// <summary>
        /// 实体组是否处于冻结状态；
        /// </summary>
        /// <param name="entityGroupName">实体组名</param>
        /// <returns>是否冻结</returns>
        bool IsEntityGroupPaused(string entityGroupName);
        /// <summary>
        /// 获取实体的信息；
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <returns>实体信息</returns>
        EntityInfo GetEntityInfo(string entityName);
        /// <summary>
        /// 获取实体组的信息；
        /// </summary>
        /// <param name="entityGroupName">实体组名</param>
        /// <returns>实体组信息</returns>
        EntityGroupInfo GetEntityGroupInfo(string entityGroupName);
    }
}
