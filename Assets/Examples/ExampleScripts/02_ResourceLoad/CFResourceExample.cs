using Cosmos;
using Cosmos.Resource;
using UnityEngine;

/// <summary>
/// 新资源加载API示例（CFResources handle式加载）。
/// <para>支持四种用法：事件回调、协程等待、async/await、同步等待。</para>
/// </summary>
public class CFResourceExample : MonoBehaviour
{
    void Start()
    {
        // 1、初始化资源系统（使用编辑器模拟模式时，先在 Window/Cosmos/Module/Resource/ResourceBuild 中导出清单）
        var initHandle = CFResources.InitializeAsync(new ResourceInitParameters()
        {
            PackageName = "DefaultPackage",
#if UNITY_EDITOR
            PlayMode = CFResourcePlayMode.AssetDatabase,
#else
            PlayMode = CFResourcePlayMode.AssetBundle,
#endif
            ManifestPath = "CosmosResources/DefaultPackage/PackageManifest.json",
            BundleDirectory = "CosmosResources/DefaultPackage",
            ManifestEncryptionKey = "CosmosBundlesKey",
        });
        initHandle.AddCompletedCallback(op =>
        {
            if (op.Status == OperationStatus.Succeeded)
            {
                Debug.Log($"CFResources initialized , version : {initHandle.Version}");
                LoadWithEventCallback();
            }
            else
            {
                Debug.LogError($"CFResources initialize failed : {op.Error}");
            }
        });
    }

    /// <summary>
    /// 用法1：事件回调
    /// </summary>
    void LoadWithEventCallback()
    {
        var handle = CFResources.LoadAssetAsync<GameObject>("Prefabs/ResCube");
        handle.Completed += h =>
        {
            var go = Instantiate(h.Asset);
            go.transform.position = Vector3.zero;
            // 使用完毕后释放
            h.Release();
        };
    }

    /// <summary>
    /// 用法2：协程等待
    /// </summary>
    System.Collections.IEnumerator LoadWithCoroutine()
    {
        var handle = CFResources.LoadAssetAsync<GameObject>("Prefabs/ResCube");
        yield return handle;
        var go = Instantiate(handle.Asset);
        go.transform.position = new Vector3(2, 0, 0);
        handle.Release();
    }

    /// <summary>
    /// 用法3：async/await
    /// </summary>
    async void LoadWithAwait()
    {
        var cube = await CFResources.LoadAssetAsync<GameObject>("Prefabs/ResCube").Task;
        if (cube != null)
        {
            var go = Instantiate(cube);
            go.transform.position = new Vector3(4, 0, 0);
        }
        // 也可直接await初始化句柄
        var initHandle = CFResources.InitializeAsync(new ResourceInitParameters()
        {
            PackageName = "DefaultPackage",
            PlayMode = CFResourcePlayMode.AssetDatabase,
            ManifestPath = "CosmosResources/DefaultPackage/PackageManifest.json",
            BundleDirectory = "CosmosResources/DefaultPackage",
        });
        await initHandle.Task;
    }

    /// <summary>
    /// 用法4：同步等待（注意会阻塞主线程，仅建议用于非频繁调用场景）
    /// </summary>
    void LoadWithWaitForCompletion()
    {
        var handle = CFResources.LoadAssetAsync<GameObject>("Prefabs/ResCube");
        var go = handle.WaitForCompletion();
        if (go != null)
        {
            Instantiate(go, new Vector3(6, 0, 0), Quaternion.identity);
        }
        handle.Release();
    }

    /// <summary>
    /// 用法5：批量加载（Addressables 的 LoadAssetsAsync 语义）
    /// <para>一批请求里同一资产复用 Provider、同一包体只加载一次，全部完成后只回调一次。</para>
    /// </summary>
    void LoadBatch()
    {
        var handle = CFResources.LoadAssetsAsync<GameObject>(new[]
        {
            "Prefabs/ResCube",
            "Prefabs/ResSphere",
        });
        handle.Completed += h =>
        {
            Debug.Log($"batch done : {h.SucceededCount}/{h.Count} , failed : {h.FailedCount}");
            if (h.Assets != null)
            {
                for (int i = 0; i < h.Assets.Length; i++)
                {
                    if (h.Assets[i] != null)
                        Instantiate(h.Assets[i], new Vector3(8 + i, 0, 0), Quaternion.identity);
                }
            }
            // 批量句柄统一释放
            h.Release();
        };
    }

    /// <summary>
    /// 用法6：分包（每个包裹独立清单、独立目录、可整体卸载）
    /// </summary>
    void LoadSubPackage()
    {
        var uiInit = CFResources.LoadPackageAsync(new ResourceInitParameters()
        {
            PackageName = "UI",
            PlayMode = CFResourcePlayMode.AssetBundle,
            ManifestPath = "CosmosResources/UI/PackageManifest.json",
            BundleDirectory = "CosmosResources/UI",
        });
        uiInit.AddCompletedCallback(op =>
        {
            if (op.Status != OperationStatus.Succeeded)
            {
                Debug.LogError($"UI package initialize failed : {op.Error}");
                return;
            }
            var handle = CFResources.LoadAssetAsync<GameObject>("UI", "Prefabs/UIRoot");
            handle.Completed += h =>
            {
                Instantiate(h.Asset);
                h.Release();
                // 整个分包一次性卸载（包内资源与包体一起释放）
                // CFResources.UnloadPackage("UI");
            };
        });
    }

    /// <summary>
    /// 场景加载与卸载
    /// </summary>
    void LoadScene()
    {
        var sceneHandle = CFResources.LoadSceneAsync("Assets/Scenes/MyScene.unity", additive: true);
        sceneHandle.Completed += h =>
        {
            Debug.Log($"Scene loaded : {h.SceneName}");
        };
        // 卸载
        // var unloadOp = sceneHandle.UnloadAsync();
    }
}
