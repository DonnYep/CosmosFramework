using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Quark;
using Cosmos;
using System;
using System.IO;
using System.Text;
using System.Net;
using UnityEngine.Networking;

public class QuarkABTest : MonoBehaviour
{
    QuarkABLoader quarkAssetABLoader;
    //[TextArea]
    //[SerializeField] string srcUrl;
    //[TextArea]
    //[SerializeField] string encryptUrl;
    //[TextArea]
    //[SerializeField] string decryptUrl;
    [SerializeField] string aseKey = "123456789ABCDEFG";
    void Start()
    {

        //Cosmos.Utility.Unity.DownloadAssetBundleAsync(srcUrl, null, (bundleBytes) =>
        //{
        //    var keyBytes = Encoding.UTF8.GetBytes(aseKey);
        //    var encryptBundleBytes = Cosmos.Utility.Encryption.AESEncryptBinary(bundleBytes, keyBytes);
        //    File.WriteAllBytes(encryptUrl, encryptBundleBytes);

        //    var decryptBundleBytes = Cosmos.Utility.Encryption.AESDecryptBinary(encryptBundleBytes, keyBytes);
        //    File.WriteAllBytes(decryptUrl, decryptBundleBytes);
        //});
        //Cosmos.Utility.Unity.StartCoroutine(downloadAsset());
        //quarkAssetABLoader = new QuarkABLoader(Utility.Unity.StreamingAssetsPathURL);
        //quarkAssetABLoader.LoadBuildInfo();
        //quarkAssetABLoader.LoadManifest();

    }
}
