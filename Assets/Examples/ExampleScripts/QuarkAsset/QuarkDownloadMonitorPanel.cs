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
public class QuarkDownloadMonitorPanel : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Text txtProgress;
    [SerializeField] Text txtDownloadingUri;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] GameObject imgBG;
    [SerializeField] QuarkLoadAssetPanel quarkLoadAssetPanel;
    bool downloadDone = false;
    bool isDisable = false;
    public void StartDownload()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
    public void HasNoLatest()
    {
        downloadDone = true;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        isDisable = true;
        imgBG.SetActive(false);
    }
    private void Awake()
    {
        QuarkManager.Instance.OnDownloadFinish += OnDownloadFinish;
        QuarkManager.Instance.OnDownloadOverall += OnDownloadOverall;
        QuarkManager.Instance.OnDownloadStart += OnDownloadStart;
        QuarkManager.Instance.OnDownloadSuccess += OnDownloadSucess;
        QuarkManager.Instance.OnDownloadFailure += OnDownloadFailure;
    }
    void Start()
    {
        if (QuarkManager.Instance.QuarkAssetLoadMode == QuarkAssetLoadMode.AssetDatabase)
        {
            HasNoLatest();
            Debug.Log(nameof(HasNoLatest));
        }
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
            txtDownloadingUri.text = "资源下载已完成";
            txtProgress.text = "点击任意键继续！";
        }
        Utility.Debug.LogInfo($"DownloadFinish {timeSpan}", MessageColor.GREEN);
        downloadDone = true;
    }
    private void Update()
    {
        if (!downloadDone)
            return;
        if (isDisable)
            return;
        if (Input.anyKey)
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            isDisable = true;
            imgBG?.SetActive(false);
            quarkLoadAssetPanel?.OnUpdateDone();
        }
    }
}
