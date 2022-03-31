﻿using UnityEngine;
using UnityEngine.UI;
using Quark;
using Cosmos;

public class QuarkLoadAssetPanel : MonoBehaviour
{
    [SerializeField] Button btnLoad;
    [SerializeField] Button btnUnload;
    [SerializeField] InputField iptAssetName;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Vector3 startPos = new Vector3(-2, 0, 0);
    [SerializeField] int rowElementCount = 4;
    [SerializeField] int dist = 2;
    int currentRow;
    int currentColumn = 0;
    GameObject objectRoot;
    public void OnUpdateDone()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    private void Awake()
    {
        QuarkResources.OnDetectedSuccess += OnDetectedSuccess;
    }
    void Start()
    {
        btnLoad?.onClick.AddListener(OnLoadClick);
        btnUnload?.onClick.AddListener(OnUnloadClick);
        objectRoot = new GameObject("ObjectRoot");
        if (QuarkResources.QuarkAssetLoadMode == QuarkAssetLoadMode.AssetDatabase)
            OnUpdateDone();
    }
    void OnLoadClick()
    {
        var assetName = iptAssetName?.text;
        if (!string.IsNullOrEmpty(assetName))
        {
            Utility.Debug.LogInfo("从AB中请求资源：" + assetName);
            QuarkResources.LoadPrefabAsync(assetName, (go) =>
            {
                go.transform.SetParent(objectRoot.transform);
                currentRow++;
                go.transform.position = startPos - new Vector3(dist * currentRow, 0, currentColumn * 2);
                if (currentRow >= rowElementCount)
                {
                    currentColumn++;
                    currentRow = 0;
                }
            }, true);
        }
    }
    void OnUnloadClick()
    {
        var assetName = iptAssetName.text;
        if (!string.IsNullOrEmpty(assetName))
        {
            Utility.Debug.LogInfo("功能未实现");
        }
    }
    void OnDetectedSuccess(long size)
    {
        if (size <= 0)
        {
            OnUpdateDone();
        }
    }
}