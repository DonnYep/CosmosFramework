using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cosmos.Resource
{
    public interface IResourceManager: IModuleManager
    {
        #region  Resources 
        T LoadResAsset<T>(string path, bool instantiate = false) where T : UnityEngine.Object;
        /// <summary>
        /// 利用挂载特性的泛型对象同步加载Prefab；
        /// </summary>
        /// <typeparam name="T">需要加载的类型</typeparam>
        /// <param name="instantiate">是否生实例化对象</param>
        /// <returns>返回实体化或未实例化的资源对象</returns>
        GameObject LoadResPrefab<T>(bool instantiate) where T : MonoBehaviour;
        T LoadResPrefab<T>() where T : MonoBehaviour;
        /// <summary>
        /// 利用挂载特性的泛型对象同步加载PrefabObject；
        /// </summary>
        /// <typeparam name="T">实现了引用池的非Mono对象</typeparam>
        /// <param name="go">载入的资源对象</param>
        /// <param name="instantiate">是否实例化</param>
        /// <returns>载入的对象</returns>
        GameObject LoadResPrefabInstance<T>(bool instantiate = false) where T : class, IReference, new();
        /// <summary>
        /// 利用挂载特性的泛型对象异步加载Prefab；
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="callBack">载入完毕后的回调</param>
        void LoadResPrefabAsync<T>(Action<T> callBack = null) where T : MonoBehaviour;
        /// <summary>
        /// 利用挂载特性的泛型对象异步加载Prefab；
        /// 泛型对象为Mono类型；
        /// </summary>
        /// <typeparam name="T">非Mono对象</typeparam>
        /// <param name="callBack">加载完毕后的回调</param>
        void LoadResPrefabInstanceAsync<T>(Action<T, GameObject> callBack = null) where T : class, IReference, new();
        T LoadResAsset<T>(bool instantiateGameObject = false) where T : UnityEngine.Component;
        /// <summary>
        /// 异步加载资源,如果目标是Gameobject，则实例化
        /// </summary>
        void LoadResAysnc<T>(string path, Action<T> callBack = null) where T : UnityEngine.Object;
        /// <summary>
        /// 异步加载资源,不实例化任何类型
        /// </summary>
        void LoadResAssetAysnc<T>(string path, Action<T> callBack = null) where T : UnityEngine.Object;
        /// <summary>
        /// 载入resources文件夹下的指定文件夹下某一类型的所有资源
        /// </summary>
        List<T> LoadResFolderAssets<T>(string path) where T : class;
        /// <summary>
        /// 载入resources文件夹下的指定文件夹下某一类型的所有资源
        /// </summary>
        T[] LoadResAll<T>(string path) where T : UnityEngine.Object;
        #endregion
        #region  AssetBundles
        //TODO ResourceManager AB资源的特性加载
        void SetManifestName(string name);
        /// <summary>
        /// 从一个AB包中获得资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        T LoadABAsset<T>(string path, string name) where T : UnityEngine.Object;
        /// <summary>
        /// 异步加载AB资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resUnit"></param>
        /// <param name="loadingCallBack"></param>
        /// <param name="loadDoneCallBack"></param>
        void LoadABAssetAsync<T>(ResourceUnit resUnit, Action<float> loadingCallBack, Action<T> loadDoneCallBack) where T : UnityEngine.Object;
        /// <summary>
        /// 异步加载AB依赖包
        /// </summary>
        /// <param name="abName"></param>
        void LoadDependenciesABAsync(string abName);
        /// <summary>
        /// 异步加载AB包，若不存在，则从web端加载
        /// </summary>
        /// <param name="abName">AssetBundle Name</param>
        /// <param name="isManifest">是否为AB清单</param>
        void LoadABAsync(string abName, bool isManifest = false);
        /// <summary>
        /// 异步加载AB包清单
        /// </summary>
        void LoadABManifestAsync();
        void UnloadAsset(string abName, bool unloadAllAssets = false);
        /// <summary>
        /// 卸载所有资源
        /// </summary>
        /// <param name="unloadAllAssets">是否卸所有实体对象</param>
        void UnloadAllAsset(bool unloadAllAssets = false);
        #endregion
    }
}
