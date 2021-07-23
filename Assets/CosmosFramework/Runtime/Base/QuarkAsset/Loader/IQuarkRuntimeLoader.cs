using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Quark
{
    /// <summary>
    /// Runtime加载时的方案；
    /// <see cref="QuarkAssetLoadMode"/>
    /// </summary>
    public interface IQuarkRuntimeLoader
    {
        Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, bool instantiate = false) where T : UnityEngine.Object;
        void UnLoadAsset(string assetBundleName, bool unloadAllLoadedObjects = false);
        void UnLoadAllAsset(bool unloadAllLoadedObjects = false);
    }
}
