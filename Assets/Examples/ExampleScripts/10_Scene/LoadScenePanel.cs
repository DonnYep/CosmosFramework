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
    float durTime = 0;
    float progressVar;
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
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneInfo(targetLevel), ProgressProvider, OnSceneLoading, LoadDoneCodition, OnSceneLoadDone);
        isLoading = true;
    }
    void Update()
    {
        if (!isLoading)
            return;
        var percent = currentProgress * 100;
        sldProgress.value = Mathf.Lerp(sldProgress.value, percent, Time.deltaTime * 5);
        txtProgress.text = Mathf.RoundToInt(sldProgress.value) + "%";
    }
    /// <summary>
    /// 自定义百分比加载方法；
    /// </summary>
    /// <returns>0-1的百分比</returns>
    float ProgressProvider()
    {
        //此方法的意义在于， 在加载场景的过程中，将伴随加载的数据、资源的进度与场景加载的进度合并，最终显示为整体的加载进度。
        //加载的数据、资源所显示的百分比进度，将与场景的百分比进度进行加权平均，最终获得真实的整体加载进度。

        //这里模拟一个大文件加载需要五秒钟，并返回0-1的百分比进度
        progressVar += Time.deltaTime;
        return progressVar / 5;
    }
    /// <summary>
    /// 进度回调，显示当前加载的整体进度；
    /// </summary>
    /// <param name="value">加载的进度</param>
    void OnSceneLoading(float value)
    {
        currentProgress = value;
    }
    /// <summary>
    /// 完成条件
    /// </summary>
    /// <returns>是否完成</returns>
    bool LoadDoneCodition()
    {
        //等待一秒；
        durTime += Time.deltaTime;
        if (durTime >= 1)
            return true;
        return false;
    }
    /// <summary>
    /// 场景完成回调
    /// </summary>
    void OnSceneLoadDone()
    {
        isLoading = false;
        loadingSlider.alpha = 0;
        Utility.Debug.LogInfo("Scene load Done");
    }
}
