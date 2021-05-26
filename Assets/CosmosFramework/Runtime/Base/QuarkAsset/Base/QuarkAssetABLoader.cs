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
        public string URL{ get; private set; }
        string buildInfoFileName = "BuildInfo.json";
        QuarkAssetABBuildInfo buildInfo;
        public QuarkAssetABLoader(string uRL)
        {
            URL = uRL;
        }
        //public T Load<T>(string filenName)
        //    where T :UnityEngine.Object
        //{

        //}
        public void LoadBuildInfo()
        {
            Utility.Unity.StartCoroutine(EnumLoadBuildInfo());
        }
        IEnumerator EnumLoadBuildInfo()
        {
            var fullPath = Utility.Unity.CombinePath(URL, buildInfoFileName);
            using (UnityWebRequest request = UnityWebRequest.Get(fullPath))
            {
                yield return request.SendWebRequest();
                var jsonText = request.downloadHandler.text;
                buildInfo = Utility.Json.ToObject<QuarkAssetABBuildInfo>(jsonText);
            }
        }
        public void LoadAssetBundle(string assetBundleName)
        {
            var fullPath = Utility.Unity.CombinePath(URL, assetBundleName);
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
