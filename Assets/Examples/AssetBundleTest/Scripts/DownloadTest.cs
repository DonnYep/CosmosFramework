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
    int targetIndex = 1;
    Action tickRefresh;
    bool isDone = false;
    private void Awake()
    {
        downloader = new MultiFileDownloader();
        downloader.DownloadUpdate += OnDownloadUpdate;
        downloader.DownloadSuccess += OnDownloadSucess;
        downloader.DownloadFailure += OnDownloadFailure;
        downloader.DownloadStart += OnDownloadStart;
    }
    void Start()
    {
        if (string.IsNullOrEmpty(srcUrl) || string.IsNullOrEmpty(downloadPath))
            return;
        if (!Directory.Exists(downloadPath))
            return;
        downloader.DownloadPath = downloadPath;
        var len = srcUrl.Length;
        //Utility.IO.TraverseFolderFile(srcUri, (info) =>
        //{
        //    var path = info.FullName;
        //    var name = info.FullName.Remove(0, len + 1);
        //    uriNameDict.TryAdd(path, name);
        //});
        //var res = Utility.IO.WebPathCombine(srcUrl, resName);
        //Utility.Debug.LogInfo(res, MessageColor.YELLOW);
        //uriNameDict.Add(res, resName);
        //srcCount = uriNameDict.Count;
        //downloader.Download(uriNameDict);
        //TickRefreshAttribute.GetRefreshAction(downloader, out tickRefresh);
        //PrintURIs();
        var lst= Utility.Net.GetUrlRootDirectoryList(srcUrl);
        Utility.Assert.Traverse(lst, (str) => Utility.Debug.LogInfo(str));
    }
    void OnDownloadStart(DownloadStartEventArgs eventArgs)
    {
        if (uriText != null)
            uriText.text = eventArgs.URI;
    }
    void OnDownloadUpdate(DownloadUpdateEventArgs eventArgs)
    {
        var progress = (int)(100 * ((float)targetIndex / (float)srcCount));
        if (text != null)
        {
            text.text = progress + "%";
        }
        if (slider != null)
        {
            slider.value = progress;
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
        Utility.Debug.LogInfo($"DownloadFailure {eventArgs.URI}");
    }
    void Update()
    {
        if (!isDone)
            tickRefresh?.Invoke();
    }
    void PrintURIs()
    {
        string url = srcUrl;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string html = reader.ReadToEnd();
                Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
                MatchCollection matches = regex.Matches(html);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        if (match.Success)
                        {
                            Debug.Log(match.Groups["name"]);
                        }
                    }
                }
            }
        }
    }
}
