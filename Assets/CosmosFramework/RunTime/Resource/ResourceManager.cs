//------------------------------------------------------------------------------
//      当前算一个早期版本，持续更新中。后期会做分类处理
//      当前资源管理需要精简，拆分功能
//在移动平台下，Application.streamingAssetsPath是只读的，不能写入数据。
//Application.persistentDataPath 可以读取和写入数据。

//在PC下，可以用File类API（如File.ReadAllText）读写StreamingAssets文件夹中的文件；
//在IOS和Android平台下，不能用File类API读取。

//所有平台上都可以用www方式异步读取StreamingAssets文件夹，
//PC和IOS平台下，读取路径必须加上"file://"，而安卓不需要。

//在IOS和Android下，还能用AssetBundle.LoadFromFile来同步读取数据。

//----
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;
using Cosmos.Config;
using System.Collections;
using UnityEngine.Networking;
namespace Cosmos.Resource
{

    public enum ResourceLoadMode : byte
    {
        Resource = 0,
        AssetBundle = 1
    }
    public sealed class ResourceManager : Module<ResourceManager>
    {
        //缓存的所有AssetBundle包 <AB包名称、AB包>
        Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>();
        //所有AssetBundle验证的Hash128值 <AB包名称、Hash128值>
        Dictionary<string, Hash128> assetBundleHashDict = new Dictionary<string, Hash128>();
        //所有AssetBundle资源包清单
        AssetBundleManifest assetBundleManifest;
        string assetBundleManifestName;
        string assetBundleRootPath;




        #region 基于Resources
        /// <summary>
        /// 同步加载资源，若可选参数为true，则返回实例化后的对象，否则只返回资源对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">相对Resource路径</param>
        /// <param name="instantiateGameObject">是否实例化GameObject类型</param>
        /// <returns></returns>
        public T LoadResAsset<T>(string path, bool instantiateGameObject = false)
            where T : UnityEngine.Object
        {
            T res = Resources.Load<T>(path);
            if (res == null)
            {
                Utility.DebugError("ResourceManager-->>" + "Assets: " + path + " not exist,check your path!");
                return null;
            }
            if (instantiateGameObject)
            {
                if (res is GameObject)
                {
                    var result = GameObject.Instantiate(res);
                    return result;
                }
            }
            return res;
        }
        /// <summary>
        /// 异步加载资源,如果目标是Gameobject，则实例化
        /// </summary>
        public void LoadResAysnc<T>(string path, CFAction<T> callBack = null)
            where T : UnityEngine.Object
        {
            Facade.Instance.StartCoroutine(EnumLoadResAsync(path, callBack));
        }
        IEnumerator EnumLoadResAsync<T>(string path, CFAction<T> callBack = null)
           where T : UnityEngine.Object
        {
            ResourceRequest req = Resources.LoadAsync<T>(path);
            yield return req;
            if (req.asset is GameObject)
                callBack?.Invoke(GameObject.Instantiate(req.asset) as T);
        }
        /// <summary>
        /// 异步加载资源,不实例化任何类型
        /// </summary>
        public void LoadResAssetAysnc<T>(string path, CFAction<T> callBack = null)
            where T : UnityEngine.Object
        {
            Facade.Instance.StartCoroutine(EnumLoadResAssetAsync(path, callBack));
        }
        IEnumerator EnumLoadResAssetAsync<T>(string path, CFAction<T> callBack = null)
   where T : UnityEngine.Object
        {
            ResourceRequest req = Resources.LoadAsync<T>(path);
            yield return req;
            callBack?.Invoke(req.asset as T);
        }
        /// <summary>
        /// 载入resources文件夹下的指定文件夹下某一类型的所有资源
        /// </summary>
        public List<T> LoadResFolderAssets<T>(string path)
       where T : class
        {
            List<T> list = new List<T>();
            var resList = Resources.LoadAll(path, typeof(T)) as T[];
            for (int i = 0; i < resList.Length; i++)
            {
                list.Add(resList[i]);
            }
            return list;
        }
        /// <summary>
        /// 载入resources文件夹下的指定文件夹下某一类型的所有资源
        /// </summary>
        public T[] LoadResAll<T>(string path)
            where T : UnityEngine.Object
        {
            T[] res = Resources.LoadAll<T>(path);
            if (res == null)
            {
                Utility.DebugError("ResourceManager-->>" + "Assets: " + path + "  not exist,check your path!");
                return null;
            }
            if (res is GameObject)
            {
                for (int i = 0; i < res.Length; i++)
                    GameObject.Instantiate(res[i]);
            }
            return res;
        }
        #endregion


        #region 基于AssetBundles
        public void SetManifestName(string name)
        {
            this.assetBundleManifestName = name;
        }
        /// <summary>
        /// 从一个AB包中获得资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public T LoadABAsset<T>(string path, string name)
            where T : UnityEngine.Object
        {
            var ab = AssetBundle.LoadFromFile(path);
            var asset = ab.LoadAsset<T>(name);
            return asset;
        }


        /// <summary>
        /// 异步加载AB资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resUnit"></param>
        /// <param name="loadingCallBack"></param>
        /// <param name="loadDoneCallBack"></param>
        public void LoadABAssetAsync<T>(ResourceUnit resUnit, CFAction<float> loadingCallBack, CFAction<T> loadDoneCallBack)
            where T : UnityEngine.Object
        {
            Facade.Instance.StartCoroutine(EnumLoadABAssetAsync(resUnit, loadingCallBack, loadDoneCallBack));
        }
        IEnumerator EnumLoadABAssetAsync<T>(ResourceUnit resUnit, CFAction<float> loadingCallBack, CFAction<T> loadDoneCallBack)
            where T : UnityEngine.Object
        {
            //先加载依赖资源
            yield return Facade.Instance.StartCoroutine(EnumLoadDependenciesABAsyn(resUnit.AssetBundleName));

            var ab = QueryAssetBundle(resUnit.AssetBundleName);
            if (ab != null)
            {
                loadingCallBack?.Invoke(1);
                yield return null;
                UnityEngine.Object asset = ab.LoadAsset<T>(resUnit.AssetPath);
                loadDoneCallBack?.Invoke(asset as T);
            }
        }
        /// <summary>
        /// 异步加载AB依赖包
        /// </summary>
        /// <param name="abName"></param>
        public void LoadDependenciesABAsync(string abName)
        {
            Facade.Instance.StartCoroutine(EnumLoadDependenciesABAsyn(abName));
        }
        IEnumerator EnumLoadDependenciesABAsyn(string abName)
        {
            yield return Facade.Instance.StartCoroutine(EnumLoadABManifestAsync());
            if (assetBundleManifest)
            {
                string[] dependencies = assetBundleManifest.GetAllDependencies(abName);
                foreach (var dep in dependencies)
                {
                    if (HasAssetBundle(dep))
                        continue;
                    yield return Facade.Instance.StartCoroutine(EnumLoadABAsync(dep));
                }
            }
        }
        /// <summary>
        /// 异步加载AB包，若不存在，则从web端加载
        /// </summary>
        /// <param name="abName">AssetBundle Name</param>
        /// <param name="isManifest">是否为AB清单</param>
        public void LoadABAsync(string abName, bool isManifest = false)
        {
            Facade.Instance.StartCoroutine(EnumLoadABAsync(abName, isManifest));
        }
        IEnumerator EnumLoadABAsync(string abName, bool isManifest = false)
        {
            if (!HasAssetBundle(abName))
            {
                using (UnityWebRequest request = isManifest ? UnityWebRequestAssetBundle.GetAssetBundle(assetBundleRootPath + abName) :
                    UnityWebRequestAssetBundle.GetAssetBundle(assetBundleRootPath + abName, GetABHash(abName)))
                {
                    yield return request.SendWebRequest();
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        //若网络通畅
                        var bundle = DownloadHandlerAssetBundle.GetContent(request);
                        if (bundle != null)
                        {
                            assetBundleDict.Add(abName, bundle);
                        }
                        else
                        {
                            //TODO  resMgr异常
                        }
                    }
                    else
                    {
                        //TODO resMgr异常

                    }
                }
            }
            yield return null;
        }
        /// <summary>
        /// 查询是否包含资源单位
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        AssetBundle QueryAssetBundle(string name)
        {
            if (assetBundleDict.ContainsKey(name))
                return assetBundleDict[name];
            return null;
        }
        bool HasAssetBundle(string name)
        {
            return assetBundleDict.ContainsKey(name);
        }
        /// <summary>
        /// 异步加载AB包清单
        /// </summary>
        public void LoadABManifestAsync()
        {
            Facade.Instance.StartCoroutine(EnumLoadABManifestAsync());
        }
        IEnumerator EnumLoadABManifestAsync()
        {
            if (string.IsNullOrEmpty(assetBundleManifestName))
            {
                //TODO 协程异常 resMgr
            }
            else
            {
                if (assetBundleManifest == null)
                {
                    yield return Facade.Instance.StartCoroutine(EnumLoadABAsync(assetBundleManifestName, true));
                    if (HasAssetBundle(assetBundleManifestName))
                    {
                        assetBundleManifest = assetBundleDict[assetBundleManifestName].LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                        UnloadAsset(assetBundleManifestName);
                    }
                }
            }
            yield return null;
        }
        public void UnloadAsset(string abName, bool unloadAllAssets = false)
        {
            var ab = QueryAssetBundle(abName);
            if (ab != null)
            {
                ab.Unload(unloadAllAssets);
                assetBundleDict.Remove(abName);
            }
        }
        /// <summary>
        /// 获得AB包的哈希值
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        Hash128 GetABHash(string abName)
        {
            if (HashAssetBundleHash(abName))
            {
                return assetBundleHashDict[abName];
            }
            else
            {
                Hash128 hash = assetBundleManifest.GetAssetBundleHash(abName);
                assetBundleHashDict.Add(abName, hash);
                return hash;
            }
        }
        bool HashAssetBundleHash(string abName)
        {
            return assetBundleHashDict.ContainsKey(abName);
        }
        /// <summary>
        /// 卸载所有资源
        /// </summary>
        /// <param name="unloadAllAssets">是否卸所有实体对象</param>
        public void UnloadAllAsset(bool unloadAllAssets = false)
        {
            foreach (var ab in assetBundleDict)
            {
                ab.Value.Unload(unloadAllAssets);
            }
            assetBundleDict.Clear();
            AssetBundle.UnloadAllAssetBundles(unloadAllAssets);
        }
        #endregion
    }

}

