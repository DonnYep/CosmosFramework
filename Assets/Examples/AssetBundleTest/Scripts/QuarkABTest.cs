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
    QuarkABLoader quarkAssetABLoader;
    void Start()
    {
        quarkAssetABLoader = new QuarkABLoader(Utility.Unity.StreamingAssetsPathURL);
        quarkAssetABLoader.LoadBuildInfo();
        quarkAssetABLoader.LoadManifest();
    }
}
