using UnityEngine;
using UnityEngine.UI;
using Quark;
using Cosmos;

public class QuarkLoadAssetPanel : MonoBehaviour
{
    [SerializeField] Button btnLoadAsset;
    [SerializeField] Button btnUnloadAsset;
    [SerializeField] Button btnLoadScene;
    [SerializeField] Button btnUnLoadScene;
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
        btnLoadAsset?.onClick.AddListener(OnLoadAssetClick);
        btnUnloadAsset?.onClick.AddListener(OnUnloadAssetClick);
        btnLoadScene?.onClick.AddListener(OnLoadSceneClick);
        btnUnLoadScene?.onClick.AddListener(OnUnloadSceneClick);
        objectRoot = new GameObject("ObjectRoot");
        if (QuarkResources.QuarkAssetLoadMode == QuarkAssetLoadMode.AssetDatabase)
            OnUpdateDone();
    }

    

    void OnLoadAssetClick()
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
    void OnUnloadAssetClick()
    {
        var assetName = iptAssetName.text;
        if (!string.IsNullOrEmpty(assetName))
        {
            Utility.Debug.LogInfo("功能未实现");
        }
    }
    void OnLoadSceneClick()
    {
        QuarkResources.LoadSceneAsync("TestAddtiveScene", null,null,true);
    }
    void OnUnloadSceneClick()
    {
        QuarkResources.UnLoadSceneAsync("TestAddtiveScene",null,null);
    }
    void OnDetectedSuccess(long size)
    {
        if (size <= 0)
        {
            OnUpdateDone();
        }
    }
}
