using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Procedure;
using Cosmos;

public class EntityGameState : ProcedureState
{
    float progressVar;
    float currentProgress;
    bool isLoading;
    float durTime = 0;

    EntityGameLoadingSlider loadingSlider;

    public override void OnDestroy(SimpleFsm<IProcedureManager> fsm)
    {
    }
    public override void OnEnter(SimpleFsm<IProcedureManager> fsm)
    {
        isLoading = true;
        progressVar = 0;
        CosmosEntry.UIManager.DeactiveUIForm("EntityGameLauncherPanel");
        CosmosEntry.UIManager.PeekUIForm<EntityGameLoadingSlider>("EntityGameLoadingSlider", out loadingSlider);
        loadingSlider.Active = true;
        CosmosEntry.SceneManager.LoadSceneAsync(new SceneAssetInfo("EntityGame"), ProgressProvider, OnSceneLoading, LoadDoneCodition, OnSceneLoaded);
    }
    public override void OnExit(SimpleFsm<IProcedureManager> fsm)
    {
        progressVar = 0;
        currentProgress = 0;
        durTime = 0;
        ;
    }
    public override void OnInit(SimpleFsm<IProcedureManager> fsm)
    {

    }
    public override void OnUpdate(SimpleFsm<IProcedureManager> fsm)
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
