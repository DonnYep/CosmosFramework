using UnityEngine;
using Cosmos.Download;
using Cosmos;
using UnityEngine.UI;
using System;

public class MyDownload : MonoBehaviour
{
    [Header("文件所在的Url根目录")]
    [SerializeField] string srcUrl;
    [Header("文件下载到的本地文件夹")]
    [SerializeField] string downloadPath;
    [SerializeField] Slider slider;
    [SerializeField] Text text;
    [SerializeField] Text uriText;
    long downloadTaskId;
    Cosmos.Download.IDownloadManager downloadManager;
    void Start()
    {
        downloadManager = CosmosEntry.DownloadManager;
        if (downloadManager == null)
        {
            Utility.Debug.LogError("Download module is missing : check BuiltinModuleRegistry !");
            return;
        }
        downloadManager.OnDownloadSuccess += OnDownloadSucess;
        downloadManager.OnDownloadFailure += OnDownloadFailure;
        downloadManager.OnDownloadStart += OnDownloadStart;
        downloadManager.OnDownloadOverallProgress += OnDownloadOverall;
        downloadManager.OnAllDownloadTaskCompleted += OnDownloadFinish;
        CosmosEntry.WebRequestManager.OnGetHtmlFilesFailureCallback += OnGetHtmlFilesFailureCallback;
        CosmosEntry.WebRequestManager.OnGetHtmlFilesSuccessCallback += OnGetHtmlFilesSuccessCallback;
        StartDownload();
    }
    void OnDestroy()
    {
        if (downloadManager == null)
            return;
        downloadManager.OnDownloadSuccess -= OnDownloadSucess;
        downloadManager.OnDownloadFailure -= OnDownloadFailure;
        downloadManager.OnDownloadStart -= OnDownloadStart;
        downloadManager.OnDownloadOverallProgress -= OnDownloadOverall;
        downloadManager.OnAllDownloadTaskCompleted -= OnDownloadFinish;
    }
    void StartDownload()
    {
        if (string.IsNullOrEmpty(srcUrl) || string.IsNullOrEmpty(downloadPath))
            return;
        Utility.IO.EmptyFolder(downloadPath);
        CosmosEntry.WebRequestManager.AddUrlFileRequestTask(srcUrl);
    }
    void OnDownloadStart(DownloadStartEventArgs eventArgs)
    {
        if (uriText != null)
            uriText.text = eventArgs.DownloadInfo.DownloadUrl;
    }
    void OnDownloadOverall(DonwloadUpdateEventArgs eventArgs)
    {
        //整体进度按字节加权（DownloadInfo.ExpectedLength 未知时才退化为按任务完成数估算）
        var overallProgress = eventArgs.TotalExpectedBytes > 0
            ? eventArgs.OverallProgress
            : (eventArgs.CurrentDownloadTaskIndex / (float)eventArgs.DownloadTaskCount);
        var percent = (float)Math.Round(overallProgress * 100f, 1);
        if (text != null)
            text.text = percent + "%";
        if (slider != null)
            slider.value = overallProgress;
    }    void OnDownloadSucess(DownloadSuccessEventArgs eventArgs)
    {
        Utility.Debug.LogInfo($"DownloadSuccess {eventArgs.DownloadInfo.DownloadUrl}");
    }
    void OnDownloadFailure(DownloadFailureEventArgs eventArgs)
    {
        Utility.Debug.LogError($"DownloadFailure {eventArgs.DownloadInfo.DownloadUrl}\n{eventArgs.ErrorMessage}");
    }
    void OnDownloadFinish(DownloadTasksCompletedEventArgs eventArgs)
    {
        if (text != null)
            text.text = "100%   Done";
        Utility.Debug.LogInfo($"DownloadFinish {eventArgs.TimeSpan} , success {eventArgs.SuccessedInfos.Length} , failed {eventArgs.FailedInfos.Length}", DebugColor.green);
    }
    private void OnGetHtmlFilesSuccessCallback(Cosmos.WebRequest.WebRequestGetHtmlFilesSuccessEventArgs eventArgs)
    {
        var fileInfos = eventArgs.UrlFileInfos;
        foreach (var fileInfo in fileInfos)
        {
            var fileDownloadPath = Utility.Text.Combine(downloadPath, fileInfo.RelativeUrl, fileInfo.FileName);
            Utility.Debug.LogInfo(fileDownloadPath);
            //重复点击时会自动从已下载的临时文件长度处续传
            downloadTaskId = downloadManager.AddDownload(fileInfo.URL, fileDownloadPath, DownloadMode.ResumeIfPossible);
        }
        downloadManager.LaunchDownload();
    }
    private void OnGetHtmlFilesFailureCallback(Cosmos.WebRequest.WebRequestGetHtmlFilesFailureEventArgs eventArgs)
    {
        var errors = eventArgs.ErrorMessages;
        foreach (var error in errors)
        {
            Utility.Debug.LogError(error);
        }
    }
}
