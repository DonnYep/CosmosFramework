using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
/// <summary>
/// 此示例展示了通过挂载特性达到资源生成的效果;
/// 使用时挂载好特性，并且实现IReference接口即可；
/// 此类型对象一般通过实体（Entity）持有，引用对象需要能够被回收
/// </summary>
public class ResourceTester : MonoBehaviour
{
    void Start()
    {
       var go= Facade.LoadResPrefabInstance<ResourceUnitCube>(true);
        go.transform.position = new Vector3(3, 0, 0);
        Facade.LoadResPrefabInstanceAsync<ResourceUnitSphere>(LoadDone);
        var monoGo= Facade.LoadResPrefab<ResourceMonoUnitTester>(true);
        monoGo.transform.position = new Vector3(5, 0, 0);
    }
    void LoadDone(ResourceUnitSphere resUnit, GameObject go)
    {
        resUnit.OnInitialization();
        go.transform.position = new Vector3(1, 0, 0);
        resUnit.ResPrefab = go;
    }
}
[PrefabUnit("ResPrefab/ResPrefab_Cube")]
public class ResourceUnitCube : IBehaviour, IReference
{
    public GameObject ResPrefab { get; set; }

    public void Clear()
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
[PrefabUnit("ResPrefab/ResPrefab_Sphere")]
public class ResourceUnitSphere : IBehaviour, IReference
{
    public GameObject ResPrefab { get; set; }

    public void Clear()
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
