using System;
using UnityEngine;

namespace Cosmos.ObjectPool
{
    public interface IObjectPoolManager : IModuleManager
    {
        /// <summary>
        /// 对象池注册成功；
        /// </summary>
        event Action<ObjectPoolRegisterSuccessEventArgs> ObjectPoolRegisterSuccess;
        /// <summary>
        /// 对象池注册失败；
        /// </summary>
        event Action<ObjectPoolRegisterFailureEventArgs> ObjectPoolRegisterFailure;
        /// <summary>
        /// 对象池的数量；
        /// </summary>
        int PoolCount { get; }
        /// <summary>
        /// 异步注册对象池；
        /// </summary>
        /// <param name="assetInfo">对象池资源信息</param>
        /// <param name="callback">注册回调</param>
        /// <returns>协程对象</returns>
        Coroutine RegisterObjectPoolAsync(ObjectPoolAssetInfo assetInfo, Action<IObjectPool> callback);
        /// <summary>
        /// 注册自定义资源对象池；
        /// </summary>
        /// <param name="poolName">对象池名</param>
        /// <param name="spawnAsset">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        IObjectPool RegisterObjectPool(string poolName, GameObject spawnAsset);
        /// <summary>
        /// 注销对象池；
        /// </summary>
        /// <param name="poolName">对象池名</param>
        void DeregisterObjectPool(string poolName);
        /// <summary>
        /// 注销对象池
        /// </summary>
        /// <param name="pool">对象池</param>
        void DeregisterObjectPool(IObjectPool pool);
        /// <summary>
        /// 获取对象池；
        /// </summary>
        /// <param name="poolName">对象池名</param>
        /// <param name="pool">对象池</param>
        /// <returns>获取结果</returns>
        bool GetObjectPool(string poolName, out IObjectPool pool);
        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <param name="poolName">对象池名</param>
        /// <returns>是否存在</returns>
        bool HasObjectPool(string poolName);
        /// <summary>
        /// 注销所有池对象
        /// </summary>
        void DeregisterAllObjectPool();
    }
}
