using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.QuarkAsset;
using Cosmos;
using System;
using System.IO;
using System.Text;
using System.Net;

public class QuarkABTest : MonoBehaviour
{
    QuarkAssetABLoader quarkAssetABLoader = new QuarkAssetABLoader(Application.streamingAssetsPath);
    [SerializeField] string manifestName= "Manifest.json";
    QuarkAssetManifest quarkAssetManifest;
    void Start()
    {
        var url = Utility.Unity.CombinePath(Utility.Unity.StreamingAssetsPathURL, manifestName);
        Utility.Debug.LogInfo(url);

        Utility.Unity.DownloadTextAsync(url, null, json => 
        {
            Utility.Debug.LogInfo(json);
        });
    }
}
