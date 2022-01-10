using System;
namespace Cosmos.ObjectPool
{
    public interface IObjectPoolManager: IModuleManager
    {
        /// <summary>
        /// 注册对象池（异步）;
        /// </summary>
        /// <param name="objectAssetInfo">对象资源信息</param>
        /// <param name="onRegisterCallback">注册成功后的回调，若失败则不回调</param>
        void RegisterObjectPoolAsync(ObjectAssetInfo objectAssetInfo, Action<IObjectPool> onRegisterCallback = null);
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <param name="objectAssetInfo">对象资源信息</param>
        /// <returns>注册生成后的池对象接口</returns>
        IObjectPool RegisterObjectPool(ObjectAssetInfo objectAssetInfo);
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <param name="objectKey">对象池key</param>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        IObjectPool RegisterObjectPool(ObjectPoolKey objectKey, object spawnItem);
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <param name="name">对象的名称</param>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        IObjectPool RegisterObjectPool(string name, object spawnItem);

        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="objectKey">对象池key</param>
        void DeregisterObjectPool(ObjectPoolKey objectKey);
        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="name">对象的名称</param>
        void DeregisterObjectPool(string name);

        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <param name="objectKey">对象池key</param>
        /// <returns>对象池对象的接口</returns>
        IObjectPool GetObjectPool(ObjectPoolKey objectKey);
        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <param name="name">对象的名称</param>
        /// <returns>对象池对象的接口</returns>
        IObjectPool GetObjectPool(string name);

        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <param name="objectKey">对象池key</param>
        /// <returns>是否存在</returns>
        bool HasObjectPool(ObjectPoolKey objectKey);
        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <param name="name">对象的名称</param>
        /// <returns>是否存在</returns>
        bool HasObjectPool(string name);

        /// <summary>
        /// 注销所有池对象
        /// </summary>
        void DeregisterAllObjectPool();
    }
}
