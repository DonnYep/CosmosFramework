using UnityEngine.UI;
using Cosmos;
using Cosmos.Scene;
using UnityEngine;
/// <summary>
/// 按钮按下载入的脚本
/// </summary>
public class LoadScenePanel : MonoBehaviour
{
    [SerializeField] GameObject root;
    readonly string targetLevel = "SceneTarget";
    Text txtProgress;
    Slider sldProgress;
    Button btnLoad;
    float currentProgress;
    CanvasGroup loadingSlider;
    bool isLoading;
    void Awake()
    {
        btnLoad = gameObject.GetComponentInChildren<Button>("BtnLoad");
        txtProgress = gameObject.GetComponentInChildren<Text>("TxtProgress");
        sldProgress = gameObject.GetComponentInChildren<Slider>("SldProgress");
        loadingSlider = gameObject.GetComponentInChildren<CanvasGroup>("LoadingSlider");
        btnLoad.onClick.AddListener(LoadClick);
        DontDestroyOnLoad(root);
    }
    void LoadClick()
    {
        loadingSlider.alpha = 1;
        btnLoad.gameObject.SetActive(false);
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneInfo(targetLevel), OnSceneLoading,OnSceneLoadDone);
        isLoading = true;
    }
    void Update()
    {
        if (!isLoading)
            return;
        var percent = currentProgress * 100;
        sldProgress.value = percent;
        txtProgress.text = (int)sldProgress.value + "%";
    }
    void OnSceneLoading(float value)
    {
        currentProgress = value;
    }
    void OnSceneLoadDone()
    {
        isLoading = false;
        loadingSlider.alpha = 0;
        Utility.Debug.LogInfo("Scene load Done");
    }
}
