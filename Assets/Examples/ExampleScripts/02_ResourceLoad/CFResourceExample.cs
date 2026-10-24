using System.Collections;
using Cosmos;
using Cosmos.Resource;
using UnityEngine;

/// <summary>
/// 资源桥（IResourceManager）示例。
/// <para>Provider 由 CosmosConfig 配置：编辑器默认使用 AssetDatabase 兜底，接入 Addressables / YooAsset / QuarkAsset 后换成对应名称即可。</para>
/// <para>下面所有写法对任意 Provider 都成立——业务代码不依赖具体资源方案。</para>
/// </summary>
public class CFResourceExample : MonoBehaviour
{
    void Start()
    {
        var manager = CosmosEntry.ResourceManager;
        if (manager == null)
        {
            Debug.LogError("resource module is missing !");
            return;
        }
        Debug.Log($"resource provider : {manager.ProviderName} , initialized : {manager.IsInitialized}");

        LoadWithEventCallback();
        StartCoroutine(LoadWithCoroutine());
        LoadWithAwait();
        LoadBatch();
        LoadWithLegacyApi();
    }

    /// <summary>
    /// 用法1：句柄 + 事件回调
    /// </summary>
    void LoadWithEventCallback()
    {
        var handle = CosmosEntry.ResourceManager.LoadAssetHandle<GameObject>("Prefabs/ResCube");
        if (handle == null)
            return;
        handle.Completed += h =>
        {
            if (h.Asset == null)
            {
                Debug.LogError($"load failed : {h.Error}");
                return;
            }
            var go = Instantiate(h.Asset);
            go.transform.position = Vector3.zero;
            // 使用完毕后释放引用
            h.Release();
        };
    }

    /// <summary>
    /// 用法2：协程等待
    /// </summary>
    IEnumerator LoadWithCoroutine()
    {
        var handle = CosmosEntry.ResourceManager.LoadAssetHandle<GameObject>("Prefabs/ResCube");
        if (handle == null)
            yield break;
        yield return handle;
        if (handle.Asset != null)
        {
            var go = Instantiate(handle.Asset);
            go.transform.position = new Vector3(2, 0, 0);
        }
        handle.Release();
    }

    /// <summary>
    /// 用法3：async/await
    /// </summary>
    async void LoadWithAwait()
    {
        var handle = CosmosEntry.ResourceManager.LoadAssetHandle<GameObject>("Prefabs/ResCube");
        if (handle == null)
            return;
        var cube = await handle.Task;
        if (cube != null)
        {
            var go = Instantiate(cube);
            go.transform.position = new Vector3(4, 0, 0);
        }
        handle.Release();
    }

    /// <summary>
    /// 用法4：同步等待（会阻塞主线程，仅建议非频繁调用）
    /// </summary>
    void LoadWithWaitForCompletion()
    {
        var handle = CosmosEntry.ResourceManager.LoadAssetHandle<GameObject>("Prefabs/ResCube");
        if (handle == null)
            return;
        var go = handle.WaitForCompletion();
        if (go != null)
            Instantiate(go, new Vector3(6, 0, 0), Quaternion.identity);
        handle.Release();
    }

    /// <summary>
    /// 用法5：批量加载（同一标识只发起一次底层请求）
    /// </summary>
    void LoadBatch()
    {
        var handle = CosmosEntry.ResourceManager.LoadAssetsHandle<GameObject>(new[]
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
            h.Release();
        };
    }

    /// <summary>
    /// 用法6：模块兼容写法（资源名 + 回调 + 配对卸载）
    /// <para>UI / Entity / Audio / DataTable 等模块内部使用同一套接口。</para>
    /// </summary>
    void LoadWithLegacyApi()
    {
        CosmosEntry.ResourceManager.LoadPrefabAsync("Prefabs/ResCube", go =>
        {
            if (go == null)
            {
                Debug.LogError("load failed : Prefabs/ResCube");
                return;
            }
            go.transform.position = new Vector3(10, 0, 0);
            // 与加载配对的释放：每调用一次 UnloadAsset 减少一个引用计数
            CosmosEntry.ResourceManager.UnloadAsset("Prefabs/ResCube");
        }, null, true);
    }

    /// <summary>
    /// 场景加载与卸载
    /// </summary>
    void LoadScene()
    {
        CosmosEntry.ResourceManager.LoadSceneAsync(new SceneAssetInfo("SceneTarget"), null, progress =>
        {
            Debug.Log($"scene loading : {progress:F2}");
        }, null, () =>
        {
            Debug.Log("scene loaded");
        });
        // CosmosEntry.ResourceManager.UnloadSceneAsync(new SceneAssetInfo("SceneTarget"), null, null, null);
    }

    /// <summary>
    /// 诊断信息
    /// </summary>
    void PrintDiagnostics()
    {
        var manager = CosmosEntry.ResourceManager;
        Debug.Log(manager.GetRuntimeInfo());
        Debug.Log(manager.GenerateDiagnosticsReport());
    }
}
