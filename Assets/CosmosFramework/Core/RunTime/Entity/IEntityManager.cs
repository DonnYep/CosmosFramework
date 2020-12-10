using Cosmos.Entity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    public interface IEntityManager:IModuleManager
    {
        int EntityGroupCount { get; }
        /// <summary>
        /// 注册EntityGroup (同步)；
        /// </summary>
        /// <param name="entityAssetInfo">实体对象信息</param>
        void RegisterEntityGroup(EntityAssetInfo entityAssetInfo);
        /// <summary>
        /// 注册EntityGroup (同步)；
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <param name="entityType">挂载EntityAssetAttribute特性的类型</param>
        void RegisterEntityGroup(Type entityType);
        /// <summary>
        /// 注册EntityGroup (同步)；
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <typeparam name="T">挂载EntityAssetAttribute特性的类型</typeparam>
        void RegisterEntityGroup<T>() where T : class;

        /// <summary>
        /// 注册EntityGroup (异步)；
        /// </summary>
        /// <param name="entityAssetInfo">实体对象信息</param>
        Coroutine RegisterEntityGroupAsync(EntityAssetInfo entityAssetInfo);

        /// <summary>
        /// 注册EntityGroup (异步)；
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <param name="entityType">挂载EntityAssetAttribute特性的类型</param>
        /// <returns>协程对象</returns>
        Coroutine RegisterEntityGroupAsync(Type entityType);
        /// <summary>
        /// 注册EntityGroup (异步)；
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <typeparam name="T">挂载EntityAssetAttribute特性的类型</typeparam>
        /// <returns>协程对象</returns>
        Coroutine RegisterEntityGroupAsync<T>() where T : class;

        /// <summary>
        /// 注销EntityGroup；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        void DeregisterEntityGroup(string entityGroupName);

        /// <summary>
        /// 注销EntityGroup;
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <typeparam name="T">挂载EntityAssetAttribute特性的类型</typeparam>
        void DeregisterEntityGroup<T>();

        /// <summary>
        /// 注销EntityGroup;
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <param name="entityType">挂载EntityAssetAttribute特性的类型</param>
       void DeregisterEntityGroup(Type entityType);

        /// <summary>
        /// 是否存在实体组；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <returns>是否存在</returns>
        bool HasEntityGroup(string entityGroupName);

        /// <summary>
        /// 获得实体组的实体资源；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <returns>是否存在</returns>
        object GetGroupEntityAsset(string entityGroupName);
        /// <summary>
        /// 是否存在实体；
        /// </summary>
        /// <param name="entityId">自定义的实体id;</param>
        /// <returns>是否存在</returns>
        bool HasEntity(int entityId);

        /// <summary>
        /// 失活&移除实体组‘；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        void DeactiveEntities(string entityGroupName);

        /// <summary>
        /// 激活&添加实体对象；
        /// </summary>
        /// <param name="entityId">自定义的实体Id</param>
        /// <param name="entityName">实体名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        void ActiveEntity(int entityId, string entityName, string entityGroupName);

        /// <summary>
        /// 失活&移除实体对象
        /// </summary>
        /// <param name="entityId">实体Id</param>
        void DeactiveEntity(int entityId);

        /// <summary>
        /// 失活&移除实体对象
        /// </summary>
        /// <param name="entity">实体对象接口索引</param>
        void DeactiveEntity(IEntity entity);


        /// <summary>
        /// 挂载子实体到父实体；
        /// </summary>
        /// <param name="childEntityId">子实体Id</param>
        /// <param name="parentEntityId">父实体Id</param>
        void AttachEntity(int childEntityId, int parentEntityId);

        /// <summary>
        /// 挂载子实体到父实体；
        /// </summary>
        /// <param name="childEntityId">子实体Id</param>
        /// <param name="parentEntity">父实体</param>
        void AttachEntity(int childEntityId, IEntity parentEntity);

        /// <summary>
        /// 挂载子实体到父实体；
        /// </summary>
        /// <param name="childEntity">子实体</param>
        /// <param name="parentEntity">父实体</param>
        void AttachEntity(IEntity childEntity, IEntity parentEntity);


        /// <summary>
        /// 解除子实体的挂载；
        /// 断开与父实体的连接；
        /// </summary>
        /// <param name="childEntityId">需要与父实体断开的子实体Id</param>
        void DeatchEntity(int childEntityId);
        /// <summary>
        /// 解除子实体的挂载；
        /// 断开与父实体的连接；
        /// </summary>
        /// <param name="childEntity">需要与父实体断开的子实体</param>
        void DeatchEntity(IEntity childEntity);

        /// <summary>
        /// 解除实体的子实体挂载；
        /// </summary>
        /// <param name="parentEntityId">父实体Id</param>
        void DeatchChildEntities(int parentEntityId);

        /// <summary>
        /// 解除实体的子实体挂载；
        /// </summary>
        /// <param name="parentEntity">父实体</param>
        void DeatchChildEntities(IEntity parentEntity);

        /// <summary>
        /// 自动获取所有程序集中挂载EntityAssetAttribute的类，并注册EntityGroup (异步)；
        /// </summary>
        /// <returns>协程对象</returns>
        Coroutine AutoRegisterEntityGroupsAsync();

    }
}
