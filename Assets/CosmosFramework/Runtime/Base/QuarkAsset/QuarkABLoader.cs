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
    public class QuarkABLoader
    {
        Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>();
        public string URL { get; private set; }
        readonly string buildInfoFileName = "BuildInfo.json";
        readonly string manifestName = "Manifest.json";
        QuarkABBuildInfo buildInfo;
        QuarkManifest quarkAssetManifest;
        public QuarkABLoader(string url)
        {
            URL = url;
        }
        public void LoadBuildInfo()
        {
            var buildInfoUrl = Utility.IO.WebPathCombine(URL, buildInfoFileName);
            Utility.Unity.DownloadTextAsync(buildInfoUrl, null, json =>
            {
                buildInfo = Utility.Json.ToObject<QuarkABBuildInfo>(json);
                Utility.Debug.LogInfo("LoadBuildInfo Done");
            });
        }
        public void LoadManifest(Action<string> loadDoneCallback=null)
        {
            var manifestUrl = Utility.IO.WebPathCombine(URL, manifestName);
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
            var fullPath = Utility.IO.WebPathCombine(URL, assetBundleName);
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
