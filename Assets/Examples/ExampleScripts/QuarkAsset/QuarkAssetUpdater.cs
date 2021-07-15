using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Quark;
using Cosmos;
using System;
using System.IO;
using System.Text;
using System.Net;
using UnityEngine.UI;
public class QuarkAssetUpdater: MonoBehaviour
{
    [SerializeField] string srcUrl;
    [SerializeField] string downloadPath;
    [SerializeField] Slider slider;
    [SerializeField] Text txtProgress;
    [SerializeField] Text txtDownloadingUri;
    QuarkDownloader quarkDownloader;
    bool isDownloadDone = false;
    bool isEnterScene = false;
    private void Awake()
    {
        quarkDownloader = new QuarkDownloader();
        quarkDownloader.OnDownloadFinish += OnDownloadFinish;
        quarkDownloader.OnDownloadOverall += OnDownloadOverall;
        quarkDownloader.OnDownloadStart += OnDownloadStart;
        quarkDownloader.OnDownloadSuccess += OnDownloadSucess;
        quarkDownloader.OnDownloadFailure += OnDownloadFailure;
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
                Utility.Debug.LogInfo(name, MessageColor.YELLOW);
            });
        }
        else
        {
            var result = Utility.Net.PingURI(srcUrl);
            if (result)
            {
                Utility.Net.PingUrlFileList(srcUrl, ref fileList);
                var length = fileList.Count;
                for (int i = 0; i < length; i++)
                {
                    var uri = fileList[i].Remove(0, len);
                    downloadableUri.Add(uri);
                    Utility.Debug.LogInfo(uri, MessageColor.YELLOW);
                }
            }
        }
        if (downloadableUri.Count > 0)
        {
            quarkDownloader.InitDownloader(srcUrl, downloadPath);
            quarkDownloader.AddDownloadFiles(downloadableUri.ToArray());
        }
    }
    void LaunchDownload()
    {
            quarkDownloader.LaunchDownload();
    }
    void OnDownloadStart(string uri, string downloadPath)
    {
        if (txtDownloadingUri != null)
            txtDownloadingUri.text = uri;
    }
    void OnDownloadOverall(string uri, string downloadPath, float overall, float individual)
    {
        var overallProgress = (float)Math.Round(overall, 1);
        if (txtProgress != null)
        {
            txtProgress.text = overallProgress + "%";
        }
        if (slider != null)
        {
            slider.value = overallProgress;
        }
    }
    void OnDownloadSucess(string uri, string downloadPath, byte[] data)
    {
        Utility.Debug.LogInfo($"DownloadSuccess {uri}");
    }
    void OnDownloadFailure(string uri, string downloadPath, string errorMessage)
    {
        Utility.Debug.LogError($"DownloadFailure {uri}\n{errorMessage}");
    }
    void OnDownloadFinish(string[] successUri, string[] failureUris, TimeSpan timeSpan)
    {
        if (txtProgress != null)
        {
            txtProgress.text = "资源下载已完成，点击任意键继续！";
        }
        Utility.Debug.LogInfo($"DownloadFinish {timeSpan}", MessageColor.GREEN);
        isDownloadDone = true;
    }
    void Update()
    {
        if (isEnterScene)
            return;
        if (isDownloadDone)
        {
            if (Input.anyKey)
            {
                isEnterScene = true;

            }
        }
    }
}
