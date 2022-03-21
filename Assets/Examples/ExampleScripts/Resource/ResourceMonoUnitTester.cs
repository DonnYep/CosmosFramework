using UnityEngine;
using Cosmos;
/// <summary>
/// Mono类型资源示例;
/// 此类型会在资源生成时自动挂载在生成对象上
/// </summary>
[PrefabAsset("Prefabs/ResCapsule")]
public class ResourceMonoUnitTester : MonoBehaviour
{
    private void Start()
    {
        Utility.Debug.LogInfo("ResourceMonoUnitTester Start!");
    }
}
