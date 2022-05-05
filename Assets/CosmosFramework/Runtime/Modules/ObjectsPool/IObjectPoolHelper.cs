using System;
using UnityEngine;
namespace Cosmos.ObjectPool
{
    public interface IObjectPoolHelper
    {
        /// <summary>
        /// 异步加载资源；
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="callback">加载回调，失败传入空</param>
        /// <returns>协程对象</returns>
        Coroutine LoadObjectAssetAsync(ObjectPoolAssetInfo assetInfo, Action<ObjectPoolAssetInfo, GameObject> callback);
        /// <summary>
        /// 卸载资源；
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        void UnloadObjectAsset(ObjectPoolAssetInfo assetInfo);
    }
}