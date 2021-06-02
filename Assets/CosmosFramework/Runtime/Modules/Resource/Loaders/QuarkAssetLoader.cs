using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Quark;
using Cosmos.Resource;
using UnityEngine;

namespace Cosmos
{
    public class QuarkAssetLoader : IResourceLoadHelper
    {
        public bool IsLoading { get { return isLoading; } }
        bool isLoading = false;
        public T[] LoadAllAsset<T>(AssetInfo info) where T : UnityEngine.Object
        {
            return null;
        }
        public T LoadAsset<T>(AssetInfo info) where T : UnityEngine.Object
        {
            return QuarkUtility.LoadAsset<T>(info.AssetName);
        }
        public Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> loadDoneCallback, Action<float> loadingCallback = null) where T : UnityEngine.Object
        {
            isLoading = true;
            return Utility.Unity.StartCoroutine(() => QuarkUtility.LoadAsset<T>(info.AssetName), () => { isLoading = false; }); ;
        }
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Action loadDoneCallback, Action<float> loadingCallback = null)
        {
            return null;
        }
        public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
        {
            QuarkUtility.UnLoadAsset();
        }
        public void UnLoadAsset(object customData, bool unloadAllLoadedObjects = false)
        {
            QuarkUtility.UnLoadAsset();
        }
    }
}
