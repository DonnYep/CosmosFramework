using Cosmos.Resource;
using System;
using UnityEngine;
namespace Cosmos.ObjectPool
{
    public class ObjectPoolAssetHelper
    {
        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="callback">加载回调，失败传入空</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadObjectAssetAsync(ObjectPoolAssetInfo assetInfo, Action<ObjectPoolAssetInfo, GameObject> callback)
        {
            return GameManager.GetModule<IResourceManager>().LoadPrefabAsync(assetInfo.AssetName, (go) => { callback?.Invoke(assetInfo, go); });
        }
        /// <summary>
        /// 卸载资源。
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        public void UnloadObjectAsset(ObjectPoolAssetInfo assetInfo)
        {
            GameManager.GetModule<IResourceManager>().UnloadAsset(assetInfo.AssetName);
        }
    }
}