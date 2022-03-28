using System.Threading.Tasks;
namespace Cosmos.Entity
{
    //================================================
    /*
     * 使用步骤：
     * 1. SetHelper 设置IEntityHelper 帮助体
     * 2.注册实体组RegisterEntityGroup
     * 3.ShowEntity，并接收返回值
     * 4.DeactiveEntity，回收实体
     * 5.注销实体组DeregisterEntityGroup
     * 可选：
     * 1.SetEntityGroupRoot设置根节点
     * 2.DeactiveEntityGroup失活组
     * 
     * 1、实体对象管理模块。
     * 
     * 2、实体组表示为，一组使用同一个预制体资源的组别。
     * 
     * 3、独立实体表示为，一个拥有独立预制体资源的对象。
     * 
     * 4、若使用pool生成，则由对象池生成。
     * 
     * 5、若不使用pool，则IEntityHelper生成实体。
     * 
     * 6、实体对象被实例化后才会存在与此。若被回收，则清理实体相关缓存。
     * 
     */
    //================================================
    public interface IEntityManager:IModuleManager
    {
        /// <summary>
        /// 实体组数量；
        /// </summary>
        int EntityGroupCount { get; }
        /// <summary>
        /// 设置实体帮助体；
        /// </summary>
        /// <param name="helper">自定义实现的实体帮助体</param>
        void SetHelper(IEntityHelper helper);
        /// <summary>
        /// 设置默认实体组帮助体；
        /// </summary>
        /// <param name="entityGroupHelper">自定义实现的实体组帮助体</param>
        void SetDefautEntityGroupHelper(IEntityGroupHelper entityGroupHelper);
        /// <summary>
        /// 注册EntityGroup (同步)；
        /// 若传入的entityGroupHelper 为空，则使用默认的entityGroupHelper；
        /// </summary>
        /// <param name="entityAssetInfo">实体对象信息</param>
        /// <param name="entityGroupHelper">实体组帮助体</param>
        /// <returns>是否注册成功</returns>
        bool RegisterEntityGroup(EntityAssetInfo entityAssetInfo, IEntityGroupHelper entityGroupHelper = null);
        /// <summary>
        /// 注册EntityGroup (异步)；
        /// 若传入的entityGroupHelper 为空，则使用默认的entityGroupHelper；
        /// </summary>
        /// <param name="entityAssetInfo">实体对象信息</param>
        /// <param name="entityGroupHelper">实体组帮助体</param>
        /// <returns>异步任务</returns>
        Task RegisterEntityGroupAsync(EntityAssetInfo entityAssetInfo, IEntityGroupHelper entityGroupHelper = null);

        /// <summary>
        /// 注销EntityGroup；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        void DeregisterEntityGroup(string entityGroupName);
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
        /// 激活&添加实体对象；
        /// </summary>
        /// <param name="entityId">自定义的实体Id</param>
        /// <param name="entityName">自定义实体名称</param>
        /// <param name="entityGroupName">注册的实体组名称</param>
        /// <returns>实体对象</returns>
        IEntity ShowEntity(int entityId, string entityName, string entityGroupName);
        /// <summary>
        ///  激活&添加实体对象；
        /// </summary>
        /// <param name="entityName">自定义实体名称</param>
        /// <param name="entityGroupName">注册的实体组名称</param>
        /// <returns>实体对象</returns>
        IEntity ShowEntity(string entityName, string entityGroupName);
        /// <summary>
        /// 激活&添加实体对象；
        ///  显示指定实体组的任意一个；
        /// </summary>
        /// <param name="entityGroupName">注册的实体组名称</param>
        /// <returns>实体对象</returns>
        IEntity ShowEntity(string entityGroupName);
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
        /// 失活&移除实体组；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        void DeactiveEntityGroup(string entityGroupName);

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

    }
}
