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
    [SerializeField]
    bool toggle;
    float time=0;
    void Start()
    {

        FutureTaskMonitor.Instance.Initiate();
        FutureTask task = FutureTask.Create(() => 
        {
            return toggle==true;
        });
        task.Completed += () => { Cosmos.Utility.Debug.LogInfo(task.Description); };
        task.Polling += () => Cosmos.Utility.Debug.LogInfo("Polling");
        task.Description = "延迟测试";
        Cosmos.Utility.Debug.LogInfo("开始测试延时");
        //if (!string.IsNullOrEmpty(localUrl)&&!string.IsNullOrEmpty(remoteUrl))
        //{
        //    if (Directory.Exists(remoteUrl)&&Directory.Exists(localUrl))
        //    {
        //        QuarkManager.Instance.RemoteUrl = remoteUrl;
        //        QuarkManager.Instance.LocalUrl= localUrl;
        //        QuarkManager.Instance.CheckLatestManifestAsync((result) => 
        //        {
        //            if (result)
        //            {
        //                QuarkManager.Instance.DownloadAssetBundlesAsync((overallProgress)=>
        //                {
        //                    Cosmos.Utility.Debug.LogInfo($"download overallProgress progress{overallProgress*100} %");
        //                },progress=> 
        //                {
        //                    Cosmos.Utility.Debug.LogInfo($"download individual progress{progress* 100} %",MessageColor.YELLOW);
        //                });
        //            }
        //        });
        //    }
        //}

    }
    private void Update()
    {
        time += Time.deltaTime;
        if (time >= 3)
            toggle = true;
    }
}
