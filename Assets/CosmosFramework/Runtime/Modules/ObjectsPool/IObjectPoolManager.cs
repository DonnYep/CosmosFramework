using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos
{
    public interface IObjectPoolManager: IModuleManager
    {
        /// <summary>
        /// 注册对象池（异步）;
        /// </summary>
        /// <param name="objectAssetInfo">对象资源信息</param>
        /// <param name="onRegisterCallback">注册成功后的回调，若失败则不回调</param>
        /// <returns>协程对象</returns>
        Coroutine RegisterObjectPoolAsync(ObjectAssetInfo objectAssetInfo, Action<IObjectPool> onRegisterCallback = null);
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
        IObjectPool RegisterObjectPool(TypeStringPair objectKey, GameObject spawnItem);
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        /// <param name="name">对象的名称</param>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        IObjectPool RegisterObjectPool(Type objectType, string name, GameObject spawnItem);
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        IObjectPool RegisterObjectPool(Type objectType, GameObject spawnItem);

        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="name">对象的名称</param>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        IObjectPool RegisterObjectPool<T>(string name, GameObject spawnItem) where T : class;
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        IObjectPool RegisterObjectPool<T>(GameObject spawnItem) where T : class;
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <param name="name">对象的名称</param>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        IObjectPool RegisterObjectPool(string name, GameObject spawnItem);
        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="objectKey">对象池key</param>
        void DeregisterObjectPool(TypeStringPair objectKey);

        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        /// <param name="name">对象的名称</param>
        void DeregisterObjectPool(Type objectType, string name);

        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        void DeregisterObjectPool(Type objectType);
        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="name">对象的名称</param>
        void DeregisterObjectPool<T>(string name) where T : class;
        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        void DeregisterObjectPool<T>() where T : class;
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
        IObjectPool GetObjectPool(TypeStringPair objectKey);
        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        /// <param name="name">对象的名称</param>
        /// <returns>对象池对象的接口</returns>
        IObjectPool GetObjectPool(Type objectType, string name);
        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        /// <returns>对象池对象的接口</returns>
        IObjectPool GetObjectPool(Type objectType);
        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="name">对象的名称</param>
        /// <returns>对象池对象的接口</returns>
        IObjectPool GetObjectPool<T>(string name) where T : class;
        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <returns>对象池对象的接口</returns>
        IObjectPool GetObjectPool<T>() where T : class;
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
        bool HasObjectPool(TypeStringPair objectKey);
        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        /// <param name="name">对象的名称</param>
        /// <returns>是否存在</returns>
        bool HasObjectPool(Type objectType, string name);
        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        /// <returns>是否存在</returns>
        bool HasObjectPool(Type objectType);

        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="name">对象的名称</param>
        /// <returns>是否存在</returns>
        bool HasObjectPool<T>(string name) where T : class;
        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <returns>是否存在</returns>
        bool HasObjectPool<T>() where T : class;
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
