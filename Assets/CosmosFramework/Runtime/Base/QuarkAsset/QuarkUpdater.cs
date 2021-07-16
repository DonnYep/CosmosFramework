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
    public class QuarkUpdater
    {
        /// <summary>
        /// Latest===Expired
        /// </summary>
        Action<string[], string[]> onComparedDifferences;
        /// <summary>
        /// Latest===Expired
        /// </summary>
        public event Action<string[], string[]> OnComparedDifferences
        {
            add { OnComparedDifferences += value; }
            remove { onComparedDifferences -= value; }
        }

        Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>();
        /// <summary>
        /// 本地持久化路径；
        /// </summary>
        public string DownloadPath { get; private set; }
        QuarkABBuildInfo buildInfo;
        QuarkManifest quarkAssetManifest;
        public QuarkUpdater(string downloadPath)
        {
            DownloadPath = downloadPath;
        }
        public void LoadBuildInfo()
        {
            var buildInfoUrl = Utility.IO.WebPathCombine(DownloadPath, QuarkConsts.BuildInfoFileName);
            QuarkUtility.Unity.DownloadTextAsync(buildInfoUrl, null, json =>
            {
                buildInfo = Utility.Json.ToObject<QuarkABBuildInfo>(json);
                Utility.Debug.LogInfo("LoadBuildInfo Done");
            });
        }
        public void LoadManifest(Action<string> loadDoneCallback=null)
        {
            var manifestUrl = Utility.IO.WebPathCombine(DownloadPath, QuarkConsts.ManifestName);
            QuarkUtility.Unity.DownloadTextAsync(manifestUrl, null, json =>
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
            var fullPath = Utility.IO.WebPathCombine(DownloadPath, assetBundleName);
            QuarkUtility.Unity.DownloadAssetBundleAsync(fullPath, null, ab => 
            {
                assetBundleDict.Add(assetBundleName, ab);
            });
        }
    }
}
