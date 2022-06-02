using UnityEngine;
using Cosmos;
using Cosmos.Resource;
public class MyResourceLoader : MonoBehaviour
{
    void Start()
    {
        //注意：CosmosEntry.ResourceManager在初始化后，默认使用Resources模式。
        CosmosEntry.ResourceManager.LoadPrefabAsync(new AssetInfo("Prefabs/ResCube"), (go) =>
         {
             go.transform.position = new Vector3(3, 0, 0);
         }, null, true);
        CosmosEntry.ResourceManager.LoadPrefabAsync(new AssetInfo("Prefabs/ResCapsule"), (go) =>
         {
             go.transform.position = new Vector3(5, 0, 0);
         }, null, true);
        CosmosEntry.ResourceManager.LoadPrefabAsync(new AssetInfo("Prefabs/ResSphere"), go =>
         {
             go.transform.position = new Vector3(0, 0, 0);
         }, null, true);
        CosmosEntry.ResourceManager.LoadPrefabAsync<ResCube>( go =>
        {
            go.transform.position = new Vector3(-3, 0, 0);
        }, null, true);
        LoadAsync();
    }
    async void LoadAsync()
    {
        await new WaitForSeconds(5);
        var asyncGo = await CosmosEntry.ResourceManager.LoadPrefabAsync(new AssetInfo("Prefabs/ResCube"), true);
        asyncGo.transform.position = new Vector3(0, 5, 0);
        asyncGo.transform.localScale = Vector3.one * 2;
    }
    [PrefabAsset("Prefabs/ResCube")]
    public class ResCube {}
}
