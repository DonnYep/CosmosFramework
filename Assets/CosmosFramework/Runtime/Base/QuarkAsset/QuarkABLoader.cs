using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace Cosmos.Quark 
{
    /// <summary>
    /// QuarkAssetBundle加载器；使用时需要注意持久化路径中包含所需资源；
    /// </summary>
    public class QuarkABLoader
    {
        Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>();
        /// <summary>
        /// 本地持久化路径；
        /// </summary>
        public string PersistencePath { get; private set; }
        readonly string buildInfoFileName = "BuildInfo.json";
        readonly string manifestName = "Manifest.json";
        QuarkABBuildInfo buildInfo;
        QuarkManifest quarkAssetManifest;
        public QuarkABLoader(string persistencePath)
        {
            PersistencePath = persistencePath;
        }
        public void LoadBuildInfo()
        {
            var buildInfoUrl = Utility.IO.WebPathCombine(PersistencePath, buildInfoFileName);
            Utility.Unity.DownloadTextAsync(buildInfoUrl, null, json =>
            {
                buildInfo = Utility.Json.ToObject<QuarkABBuildInfo>(json);
                Utility.Debug.LogInfo("LoadBuildInfo Done");
            });
        }
        public void LoadManifest(Action<string> loadDoneCallback=null)
        {
            var manifestUrl = Utility.IO.WebPathCombine(PersistencePath, manifestName);
            Utility.Unity.DownloadTextAsync(manifestUrl, null, json =>
            {
                quarkAssetManifest = Utility.Json.ToObject<QuarkManifest>(json);
                if (quarkAssetManifest != null)
                {
                    QuarkManager.Instance.SetBuiltAssetBundleModeData(quarkAssetManifest);
                    Utility.Debug.LogInfo("LoadManifest Done");
                }
                loadDoneCallback?.Invoke(json);
            });
        }
        public void LoadAssetBundle(string assetBundleName)
        {
            var fullPath = Utility.IO.WebPathCombine(PersistencePath, assetBundleName);
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(fullPath))
            {
                if (!request.isNetworkError && !request.isHttpError)
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                    if (bundle)
                    {
                        assetBundleDict.Add(assetBundleName, bundle);
                    }
                    else
                    {
                        throw new ArgumentException($"ResourceManager-->>请求：{request.url }未下载到AB包！");
                    }
                }
            }
        }
    }
}
