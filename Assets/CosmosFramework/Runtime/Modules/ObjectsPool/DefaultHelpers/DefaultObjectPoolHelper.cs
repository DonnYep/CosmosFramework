using System;
using Cosmos.ObjectPool;
using UnityEngine;
namespace Cosmos
{
    public class DefaultObjectPoolHelper : IObjectPoolHelper
    {
        public Coroutine LoadObjectAssetAsync(ObjectPoolAssetInfo assetInfo, Action<ObjectPoolAssetInfo, GameObject> callback)
        {
            return CosmosEntry.ResourceManager.LoadPrefabAsync(assetInfo, (go) => { callback?.Invoke(assetInfo, go); });
        }
        public void UnloadObjectAsset(ObjectPoolAssetInfo assetInfo)
        {
            CosmosEntry.ResourceManager.UnloadAsset(assetInfo);
        }
    }
}
