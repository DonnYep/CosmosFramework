using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos.Quark;
using Cosmos;
using System;

public class QuarkLoadAssetPanel : MonoBehaviour
{
    [SerializeField] Button btnLoad;
    [SerializeField] Button btnUnload;
    [SerializeField] InputField iptAssetName;
    [SerializeField] CanvasGroup canvasGroup;
    GameObject objectRoot;
    public void OnUpdateDone()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    void Start()
    {
        btnLoad?.onClick.AddListener(OnLoadClick);
        btnUnload?.onClick.AddListener(OnUnloadClick);
        QuarkManager.Instance.QuarkAssetLoadMode = QuarkAssetLoadMode.BuiltAssetBundle;
        QuarkManager.Instance.OnDetectedSuccess += OnDetectedSuccess;
        objectRoot = new GameObject("ObjectRoot");
    }
    void OnLoadClick()
    {
        var assetName = iptAssetName?.text;
        if (!string.IsNullOrEmpty(assetName))
        {
            Utility.Debug.LogInfo("加载：" + assetName);
            QuarkManager.Instance.LoadAssetAsync<GameObject>(assetName, (go) => { go.transform.SetParent(objectRoot.transform); }, true);
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
