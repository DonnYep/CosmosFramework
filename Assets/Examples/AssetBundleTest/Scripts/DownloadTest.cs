using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Download;
using Cosmos;
using System.IO;
using UnityEngine.UI;
using System;
using System.Net;
using System.Text.RegularExpressions;

public class DownloadTest : MonoBehaviour
{
    MultiFileDownloader downloader;
    [SerializeField]
    string srcUrl;
    [SerializeField]
    string resName;
    [SerializeField]
    string downloadPath;
    Dictionary<string, string> uriNameDict = new Dictionary<string, string>();
    [SerializeField]
    Slider slider;
    [SerializeField]
    Text text;
    [SerializeField]
    Text uriText;
    int srcCount;
    int targetIndex = 0;
    Action tickRefresh;
    bool isDone = false;
    [SerializeField]
    private void Awake()
    {
        downloader = new MultiFileDownloader();
        downloader.DownloadSuccess += OnDownloadSucess;
        downloader.DownloadFailure += OnDownloadFailure;
        downloader.DownloadStart += OnDownloadStart;
        downloader.DownloadOverall += OnDownloadOverall; ;
    }

    void Start()
    {
        if (string.IsNullOrEmpty(srcUrl) || string.IsNullOrEmpty(downloadPath))
            return;
        if (!Directory.Exists(downloadPath))
            return;
        downloader.DownloadPath = downloadPath;
        var len = srcUrl.Length;
        Utility.IO.TraverseFolderFile(srcUrl, (info) =>
        {
            var path = info.FullName;
            var name = info.FullName.Remove(0, len + 1);
            uriNameDict.TryAdd(path, name);
        });
        //var res = Utility.IO.WebPathCombine(srcUrl, resName);
        //Utility.Debug.LogInfo(res, MessageColor.YELLOW);
        //uriNameDict.Add(res, resName);

        srcCount = uriNameDict.Count;

        downloader.Download(uriNameDict);
        TickRefreshAttribute.GetRefreshAction(downloader, out tickRefresh);

        //单位块的百分比长度；



        //PrintURIs();
        //var lst= Utility.Net.GetUrlRootFiles(srcUrl);
        //Utility.Assert.Traverse(lst, (str) => Utility.Debug.LogInfo(str));
    }
    void OnDownloadStart(DownloadStartEventArgs eventArgs)
    {
        if (uriText != null)
            uriText.text = eventArgs.URI;
    }
    void OnDownloadOverall(DonwloadOverallEventArgs  eventArgs)
    {
        var overallProgress = (float)Math.Round(eventArgs.OverallProgress, 1);
        if (text != null)
        {
            text.text = overallProgress+ "%";
        }
        if (slider != null)
        {
            slider.value = overallProgress;
        }
    }
    void OnDownloadSucess(DownloadSuccessEventArgs eventArgs)
    {
        Utility.Debug.LogInfo($"DownloadSuccess {eventArgs.URI}");
        targetIndex++;
        var progress = (int)(100 * ((float)targetIndex / (float)srcCount));
        if (progress == 100) isDone = true;
    }
    void OnDownloadFailure(DownloadFailureEventArgs eventArgs)
    {
        Utility.Debug.LogError($"DownloadFailure {eventArgs.URI}");
    }
    void Update()
    {
        //if (!isDone)
            tickRefresh?.Invoke();
    }
}
