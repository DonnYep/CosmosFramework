using UnityEngine;
using Cosmos.Procedure;
using Cosmos;
using Cosmos.Extensions;

public class EntityGameState : ProcedureNode
{
    float progressVar;
    float currentProgress;
    bool isLoading;
    float durTime = 0;

    EntityGameLoadingSlider loadingSlider;

    public override void OnDestroy(ProcedureProcessor processo)
    {
    }
    public override void OnEnter(ProcedureProcessor processo)
    {
        isLoading = true;
        progressVar = 0;
        CosmosEntry.UIManager.DeactiveUIForm("EntityGameLauncherPanel");
        CosmosEntry.UIManager.PeekUIForm<EntityGameLoadingSlider>("EntityGameLoadingSlider", out loadingSlider);
        loadingSlider.Active = true;
        CosmosEntry.ResourceManager.LoadSceneAsync(new SceneAssetInfo("EntityGame"), ProgressProvider, OnSceneLoading, LoadDoneCodition, OnSceneLoaded);
    }
    public override void OnExit(ProcedureProcessor processo)
    {
        progressVar = 0;
        currentProgress = 0;
        durTime = 0;
        ;
    }
    public override void OnInit(ProcedureProcessor processo)
    {

    }
    public override void OnUpdate(ProcedureProcessor processo)
    {
        if (isLoading)
        {
            loadingSlider?.UpdateProgress(currentProgress);
        }
    }
    async void OnSceneLoaded()
    {
        isLoading = false;
        loadingSlider.Active = false;
        var cameraRoot = GameObject.Find("CameraRoot");
        var player = await CosmosEntry.ResourceManager.LoadPrefabAsync("EntityPlayer", true);
        cameraRoot.AddComponent<EntityCamera>().SetCameraTarget(player.transform);
    }
    float ProgressProvider()
    {
        progressVar += Time.deltaTime;
        return progressVar / 1;
    }
    void OnSceneLoading(float value)
    {
        currentProgress = value;
    }
    bool LoadDoneCodition()
    {
        durTime += Time.deltaTime;
        if (durTime >= 1)
            return true;
        return false;
    }
}
