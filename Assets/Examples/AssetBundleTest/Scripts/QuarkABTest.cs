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
    [TextArea]
    [SerializeField] string remoteUrl;
    [TextArea]
    [SerializeField] string localUrl;
    QuarkABLoader quarkAssetABLoader;
    void Start()
    {
        
        if (!string.IsNullOrEmpty(localUrl)&&!string.IsNullOrEmpty(remoteUrl))
        {
            if (Directory.Exists(remoteUrl)&&Directory.Exists(localUrl))
            {
                QuarkManager.Instance.RemoteUrl = remoteUrl;
                QuarkManager.Instance.LocalUrl= localUrl;
                QuarkManager.Instance.CheckLatestManifestAsync((result) => 
                {
                    if (result)
                    {
                        QuarkManager.Instance.DownloadAssetBundlesAsync((overallProgress)=>
                        {
                            Cosmos.Utility.Debug.LogInfo($"download overallProgress progress{overallProgress*100} %");
                        },progress=> 
                        {
                            Cosmos.Utility.Debug.LogInfo($"download individual progress{progress* 100} %",MessageColor.YELLOW);
                        });
                    }
                });
            }
        }

    }
}
