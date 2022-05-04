using System;
using UnityEngine;

namespace Cosmos.ObjectPool
{
    public interface IObjectPoolHelper
    {
        event Action<GameObject> OnLoadSuccess;
        event Action<GameObject> OnLoadFailure;
        void LoadObjectAssetAsync(ObjectAssetInfo assetInfo);
        void UnloadObjectAsset(ObjectAssetInfo assetInfo);
    }
}
