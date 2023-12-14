using System;
using System.Threading.Tasks;
namespace Cosmos.Entity
{
    //================================================
    /*
     * 使用步骤：
     * 
     * 1、实体对象管理模块。
     * 
     */
    //================================================
    public interface IEntityManager : IModuleManager
    {
        /// <summary>
        /// 实体数量；
        /// </summary>
        int EntityCount { get; }
        /// <summary>
        /// 实体组数量；
        /// </summary>
        int EntityGroupCount { get; }
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
        /// 注册实体；
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="entityAssetInfo">实体信息</param>
        void RegisterEntityAsync<T>(EntityAssetInfo entityAssetInfo) where T : EntityObject;
        /// <summary>
        /// 注销实体；
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
        /// 显示实体；
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <param name="entity">实体对象</param>
        /// <returns>显示结果</returns>
        bool ShowEntity(string entityName, out EntityObject entity);
        /// <summary>
        /// 隐藏实体；
        /// </summary>
        /// <param name="entityName">实体名</param>
        void HideEntities(string entityName);
        /// <summary>
        /// 隐藏实体；
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <param name="entityObjectId">实体实例Id</param>
        void HideEntityObject(string entityName, int entityObjectId);
        /// <summary>
        ///  隐藏实体；
        /// </summary>
        /// <param name="entityObject">实体实例</param>
        void HideEntityObject(EntityObject entityObject);
        /// <summary>
        /// 为实体设置组别；
        /// </summary>
        /// <param name="entityName">实体名</param>
        /// <param name="entityGroupName">实体组名</param>
        void SetEntityGroup(string entityName, string entityGroupName);
        /// <summary>
        /// 显示实体组中的所有实体；
        /// </summary>
        /// <param name="entityGroupName">实体组名</param>
        void ShowGroupEntities(string entityGroupName);
        /// <summary>
        /// 隐藏实体组中的所有实体；
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
