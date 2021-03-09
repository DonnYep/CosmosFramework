using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if COSMOS_ADDRESSABLE
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
namespace Cosmos
{
    public class DefaultAddressableHelper : IResourceHelper
    {
        public void LoadAssetsAsync<T>(AssetInfo info, Action<T> callback) where T : UnityEngine.Object
        {
            Addressables.LoadAssetsAsync(info.ResourcePath, callback);
        }
        public void LoadAssetsAsync<T>(AssetInfo info, Action<IList< T>> callback) where T : UnityEngine.Object
        {
            Addressables.LoadAssetsAsync(info.ResourcePath,callback);
        }
        public void LoadPrefabAsync(AssetInfo info,Action<GameObject>onLoadedCallback)
        {
           var go= Addressables.InstantiateAsync(info.ResourcePath).Result;
            onLoadedCallback?.Invoke(go);
        }
        public void Release<T>(T obj)
            where T : UnityEngine.Object
        {
            Addressables.Release(obj);
        }
        public void ReleaseInstance(GameObject obj)
        {
            Addressables.ReleaseInstance(obj);
        }
        public void LoadSceneAsync(SceneAssetInfo sceneInfo)
        {
            Addressables.LoadSceneAsync(sceneInfo.SceneName, (LoadSceneMode)Convert.ToByte(sceneInfo.Additive),true,sceneInfo.Priority);
        }
        public void UnloadSceneAsync(SceneInstance scene)
        {
            Addressables.UnloadSceneAsync(scene);
        }
    }
}
#endif
