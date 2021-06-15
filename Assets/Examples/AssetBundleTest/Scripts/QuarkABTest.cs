
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Quark;
using Cosmos;
using System;
using System.IO;
using System.Text;
using System.Net;

public class QuarkABTest : MonoBehaviour
{
    [TextArea]
    [SerializeField] string remoteUrl;
    [TextArea]
    [SerializeField] string localUrl;
    QuarkABLoader quarkAssetABLoader;
    [SerializeField]
    bool toggle;
    [SerializeField]
    float waitTime = 6;
    [SerializeField]
    float time = 0;
    void Start()
    {
        FutureTask.Detection(() =>
       {
           return toggle == true;
       }, (t) => { Utility.Debug.LogInfo("Polling"); }, (t) =>
          {
              Utility.Debug.LogInfo(t.Description + " ---异步状态结束");
          }, "延迟测试");
        Utility.Debug.LogInfo("开始测试延时");
    }
    private void Update()
    {
        time += Time.deltaTime;
        if (time >= waitTime)
        {
            toggle = true;
            time = waitTime;
        }
    }
}
