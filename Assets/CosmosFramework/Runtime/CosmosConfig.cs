using UnityEngine;
using Cosmos.Resource;

namespace Cosmos
{
    /// <summary>
    /// 框架启动配置。
    /// <para>资源方案是可插拔的：这里只需要填写 Provider 名称（如 Addressables / YooAsset / QuarkAsset / AssetDatabase）与参数，</para>
    /// <para>框架不依赖任何具体资源库，UI / Entity / Audio / Scene / DataTable 等模块通过资源桥统一访问。</para>
    /// </summary>
    [DisallowMultipleComponent]
    public class CosmosConfig : MonoBehaviour
    {
        [SerializeField] bool launchAppDomainModules = true;
        [SerializeField] bool printModulePreparatory = false;

        [Header("Resource")]
        [Tooltip("资源方案名称：Addressables / YooAsset / QuarkAsset / AssetDatabase（编辑器兜底）")]
        [SerializeField] string resourceProviderName = "AssetDatabase";
        [Tooltip("Provider 参数，形如 key=value;key2=value2")]
        [SerializeField] string resourceProviderSettings = string.Empty;
        [Tooltip("是否记录资源加载诊断（编辑器默认开启）")]
        [SerializeField] bool resourceDiagnostics = true;

        [SerializeField] int debugHelperIndex;
        [SerializeField] int jsonHelperIndex;
        [SerializeField] int messagePackHelperIndex;

        [SerializeField] string debugHelperName;
        [SerializeField] string jsonHelperName;

        [SerializeField] bool runInBackground;

        [SerializeField] bool drawDebugWindow;
        [SerializeField] bool disableDebugLog = false;

        [SerializeField] int inputHelperIndex;
        [SerializeField] string inputHelperName;

        [SerializeField] bool moduleConfigFoldout;

        /// <summary>
        /// 按配置选择并初始化资源 Provider。
        /// </summary>
        public void LoadResource()
        {
            var resourceManager = CosmosEntry.ResourceManager;
            if (resourceManager == null)
            {
                Debug.LogError("[CosmosConfig] resource module is missing, check launchAppDomainModules !");
                return;
            }
            resourceManager.DiagnosticsEnabled = resourceDiagnostics;
            resourceManager.SetProvider(resourceProviderName, resourceProviderSettings);
        }

        protected virtual void Awake()
        {
            LoadHelpers();
            CosmosEntry.PrintModulePreparatory = printModulePreparatory;
            Application.runInBackground = runInBackground;
            Utility.Debug.DisableDebugLog = disableDebugLog;
            if (launchAppDomainModules)
            {
                CosmosEntry.LaunchAppDomainModules();
                LoadResource();
                SetInputHelper();
            }
        }
        void LoadHelpers()
        {
            if (!string.IsNullOrEmpty(debugHelperName) && debugHelperName != Constants.NONE)
            {
                var debugHelper = Utility.Assembly.GetTypeInstance(debugHelperName);
                if (debugHelper != null)
                    Utility.Debug.SetHelper((Utility.Debug.IDebugHelper)debugHelper);
            }
            if (!string.IsNullOrEmpty(jsonHelperName) && jsonHelperName != Constants.NONE)
            {
                var jsonHelper = Utility.Assembly.GetTypeInstance(jsonHelperName);
                if (jsonHelper != null)
                    Utility.Json.SetHelper((Utility.Json.IJsonHelper)jsonHelper);
            }
        }
        void SetInputHelper()
        {
            if (!string.IsNullOrEmpty(inputHelperName) && inputHelperName != Constants.NONE)
            {
                var inputHelper = Utility.Assembly.GetTypeInstance<Cosmos.Input.IInputHelper>(inputHelperName);
                if (inputHelper != null)
                    CosmosEntry.InputManager.SetInputHelper(inputHelper);
            }
        }
    }
}
