using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace Cosmos.QuarkAsset
{
    public class QuarkAssetABLoader
    {
        Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>();
        public string AssetBundleRootPath { get { return Application.streamingAssetsPath; } }
        string buildInfoFileName = "BuildInfo.json";

        //QuarkAssetABBuildInfo buildInfo;
        public void LoadBuildInfo()
        {
            Utility.Unity.StartCoroutine(EnumLoadBuildInfo());
        }
        IEnumerator EnumLoadBuildInfo()
        {
            var fullPath = Utility.Unity.CombinePath(AssetBundleRootPath, buildInfoFileName);
            using (UnityWebRequest request = UnityWebRequest.Get(fullPath))
            {
                Utility.Debug.LogInfo($"start loading {fullPath}");
                yield return request.SendWebRequest();
                var jsonText = request.downloadHandler.text;
                Utility.Debug.LogInfo($"{jsonText.Length}");
                var buildInfo = Utility.Json.ToObject<QuarkAssetABBuildInfo>(jsonText);
                if (buildInfo != null)
                {
                    var dict = buildInfo.AssetDataMaps;
                    foreach (var d in dict)
                    {
                        Utility.Assembly.TraverseInstanceAllFileds(d, (str, value) =>
                        {
                            Utility.Debug.LogInfo($"{str}:{value}");
                        });
                    }
                }
            }
        }
        public void LoadAssetBundle(string assetBundleName)
        {
            var fullPath = Utility.Unity.CombinePath(AssetBundleRootPath, assetBundleName);
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
