using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Resource
{
    /// <summary>
    ///资源加载适配接口
    /// </summary>
    public interface IResourceLoadHelper
    {
        bool IsLoading { get; }
        T LoadAsset<T>(AssetInfo info)where T : UnityEngine.Object;
        T[] LoadAllAsset<T>(AssetInfo info) where T : UnityEngine.Object;
        Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> loadDoneCallback, Action<float> loadingCallback = null)where T : UnityEngine.Object;
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo info, Action loadDoneCallback, Action<float> loadingCallback = null);
        void UnLoadAsset(object customData, bool unloadAllLoadedObjects = false);
        void UnLoadAllAsset(bool unloadAllLoadedObjects = false);
    }
}
