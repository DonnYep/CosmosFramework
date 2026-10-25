using System;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Input;
using Cosmos.Resource;

namespace Cosmos
{
    /// <summary>
    /// 框架启动配置。
    /// <para>启动流程只做三件事：设置静态配置 → 启动模块（内置零反射 + 外部程序集按需发现）→ 初始化资源与输入帮助体。</para>
    /// <para>资源方案是可插拔的：这里只需要填写 Provider 名称（如 Addressables / YooAsset / QuarkAsset / AssetDatabase）与参数，</para>
    /// <para>框架不依赖任何具体资源库，UI / Entity / Audio / Scene / DataTable 等模块通过资源桥统一访问。</para>
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-10000)]
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
            if (!GameManager.TryGetModule<IResourceManager>(out var resourceManager))
            {
                Debug.LogError($"[CosmosConfig] resource module is missing; keep launchAppDomainModules = true (or pass ResourceManager to CosmosEntry.LaunchModules).");
                return;
            }
            resourceManager.DiagnosticsEnabled = resourceDiagnostics;
            resourceManager.SetProvider(resourceProviderName, resourceProviderSettings);
        }

        protected virtual void Awake()
        {
            //静态配置先落地：模块初始化阶段就可能用到 Debug / Json 帮助体
            ApplyStaticConfig();
            LoadHelpers();
            if (launchAppDomainModules)
            {
                CosmosEntry.LaunchAppDomainModules();
                LoadResource();
                SetInputHelper();
            }
        }
        /// <summary>
        /// 应用与模块无关的静态配置
        /// </summary>
        void ApplyStaticConfig()
        {
            CosmosEntry.PrintModulePreparatory = printModulePreparatory;
            Application.runInBackground = runInBackground;
            Utility.Debug.DisableDebugLog = disableDebugLog;
        }
        /// <summary>
        /// 按配置装配 Debug / Json 帮助体。
        /// <para>只在这里解析一次类型名，并且解析失败会明确告警（旧实现是静默忽略）。</para>
        /// </summary>
        void LoadHelpers()
        {
            if (TryResolveHelper<Utility.Debug.IDebugHelper>(debugHelperName, out var debugHelper))
            {
                Utility.Debug.SetHelper(debugHelper);
            }
            if (TryResolveHelper<Utility.Json.IJsonHelper>(jsonHelperName, out var jsonHelper))
            {
                Utility.Json.SetHelper(jsonHelper);
            }
        }
        void SetInputHelper()
        {
            if (!GameManager.TryGetModule<IInputManager>(out var inputManager))
            {
                Debug.LogError("[CosmosConfig] input module is missing, input helper is not applied !");
                return;
            }
            if (TryResolveHelper<Cosmos.Input.IInputHelper>(inputHelperName, out var inputHelper))
            {
                inputManager.SetInputHelper(inputHelper);
            }
        }
        /// <summary>
        /// 解析帮助体类型并实例化
        /// </summary>
        /// <typeparam name="T">帮助体接口</typeparam>
        /// <param name="typeName">类型完全限定名</param>
        /// <param name="helper">帮助体实例</param>
        /// <returns>是否解析成功</returns>
        static bool TryResolveHelper<T>(string typeName, out T helper)
            where T : class
        {
            helper = null;
            if (string.IsNullOrEmpty(typeName) || typeName == Constants.NONE)
                return false;
            var type = Utility.Assembly.GetType(typeName);
            if (type == null)
            {
                Debug.LogWarning($"[CosmosConfig] helper type is not found : {typeName} (expected interface {typeof(T).Name})");
                return false;
            }
            if (!typeof(T).IsAssignableFrom(type) || type.IsAbstract || type.GetConstructor(Type.EmptyTypes) == null)
            {
                Debug.LogWarning($"[CosmosConfig] helper type is invalid : {typeName} (must implement {typeof(T).Name} and have a parameterless constructor)");
                return false;
            }
            helper = (T)Activator.CreateInstance(type);
            return true;
        }
    }
}
