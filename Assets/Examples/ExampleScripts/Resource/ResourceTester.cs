using UnityEngine;
using Cosmos;
using Cosmos.Resource;
/// <summary>
/// 此示例展示了通过挂载特性达到资源生成的效果;
/// 使用时挂载好特性，并且实现IReference接口即可；
/// 此类型对象一般通过实体（Entity）持有，引用对象需要能够被回收
/// </summary>
public class ResourceTester : MonoBehaviour
{
  void Start()
    {
        CosmosEntry.ResourceManager.LoadPrefabAsync<ResourceUnitCube>((go) =>
       {
           go.transform.position = new Vector3(3, 0, 0);
       }, null, true);
        CosmosEntry.ResourceManager.LoadPrefabAsync<ResourceMonoUnitTester>((go) =>
        {
            go.transform.position = new Vector3(5, 0, 0);
        }, null, true);
        CosmosEntry.ResourceManager.LoadPrefabAsync<ResourceUnitSphere>(go =>
        {
            go.transform.position = new Vector3(0, 0, 0);
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
}
[PrefabAsset("Prefabs/ResCube")]
public class ResourceUnitCube : IBehaviour, IReference
{
    public void Release()
    {
        Utility.Debug.LogInfo("ResoureceUnitCube IReference Clear ! ", MessageColor.INDIGO);
    }
    public void OnInitialization()
    {
        Utility.Debug.LogInfo("ResoureceUnitCube OnInitialization! ", MessageColor.INDIGO);
    }
    public void OnTermination()
    {
        Utility.Debug.LogInfo("ResoureceUnitCube OnTermination! ", MessageColor.INDIGO);
    }
}
[PrefabAsset("Prefabs/ResSphere")]
public class ResourceUnitSphere : IBehaviour, IReference
{
    public void Release()
    {
        Utility.Debug.LogInfo(" ResoureceUnitSphere IReference Clear ! ", MessageColor.INDIGO);
    }
    public void OnInitialization()
    {
        Utility.Debug.LogInfo("ResoureceUnitSphere OnInitialization! ", MessageColor.INDIGO);
    }
    public void OnTermination()
    {
        Utility.Debug.LogInfo("ResoureceUnitSphere OnTermination! ", MessageColor.INDIGO);
    }
}
