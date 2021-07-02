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
using System.Threading.Tasks;
using System.Text;

public class DownloadTest : MonoBehaviour
{
    Downloader downloader;
    [SerializeField]
    string srcUrl;
    [SerializeField]
    string downloadPath;
    [SerializeField]
    Slider slider;
    [SerializeField]
    Text text;
    [SerializeField]
    Text uriText;
    int srcCount;
    private void Awake()
    {
        downloader = new Downloader();
        downloader.DownloadSuccess += OnDownloadSucess;
        downloader.DownloadFailure += OnDownloadFailure;
        downloader.DownloadStart += OnDownloadStart;
        downloader.DownloadOverall += OnDownloadOverall;
    }
    void Start()
    {
        if (string.IsNullOrEmpty(srcUrl) || string.IsNullOrEmpty(downloadPath))
            return;
        if (!Directory.Exists(downloadPath))
            return;
        var len = srcUrl.Length;
        List<string> downloadableUri = new List<string>();
        var fileList = new List<string>();
        if (Directory.Exists(srcUrl))
        {
            Utility.IO.TraverseFolderFile(srcUrl, (info) =>
            {
                var path = info.FullName;
                var name = info.FullName.Remove(0, len + 1);
                downloadableUri.Add(name);
            });
        }
        else
        {
            var result= Utility.Net.PingURI(srcUrl);
            if (result)
            {
                Utility.Net.PingUrlFileList(srcUrl, fileList);
                var length = fileList.Count;
                for (int i = 0; i < length; i++)
                {
                    var uri = fileList[i].Remove(0, len);
                    downloadableUri.Add(uri);
                    Utility.Debug.LogInfo(uri, MessageColor.YELLOW);
                }
            }
        }
        srcCount = downloadableUri.Count;
        if (srcCount > 0)
        {
            downloader.Download(srcUrl, downloadPath, downloadableUri.ToArray());
        }
    }
    void OnDownloadStart(DownloadStartEventArgs eventArgs)
    {
        if (uriText != null)
            uriText.text = eventArgs.URI;
    }
    void OnDownloadOverall(DonwloadOverallEventArgs eventArgs)
    {
        var overallProgress = (float)Math.Round(eventArgs.OverallProgress, 1);
        if (text != null)
        {
            text.text = overallProgress + "%";
        }
        if (slider != null)
        {
            slider.value = overallProgress;
        }
    }
    void OnDownloadSucess(DownloadSuccessEventArgs eventArgs)
    {
        Utility.Debug.LogInfo($"DownloadSuccess {eventArgs.URI}");
    }
    void OnDownloadFailure(DownloadFailureEventArgs eventArgs)
    {
        Utility.Debug.LogError($"DownloadFailure {eventArgs.URI}");
    }
    void Update()
    {
        downloader.TickRefresh();
    }
}
