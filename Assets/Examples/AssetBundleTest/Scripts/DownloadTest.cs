using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Download;
using Cosmos;
using System.IO;
using UnityEngine.UI;
public class DownloadTest : MonoBehaviour
{
    MultiFileDownloader downloader;
    [SerializeField]
    string srcUri;
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
        if (string.IsNullOrEmpty(srcUri) || string.IsNullOrEmpty(downloadPath))
            return;
        if (!Directory.Exists(srcUri) || !Directory.Exists(downloadPath))
            return;
        downloader.DownloadPath = downloadPath;
        var len = srcUri.Length;
        Utility.IO.TraverseFolderFile(srcUri, (info) =>
        {
            var path = info.FullName;
            var name = info.FullName.Remove(0,len+1);
            uriNameDict.TryAdd(path, name);
        });
        srcCount = uriNameDict.Count;
        downloader.Download(uriNameDict);
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
    }
    void OnDownloadFailure(DownloadFailureEventArgs eventArgs)
    {
        Utility.Debug.LogInfo($"DownloadFailure {eventArgs.URI}");
    }
}
