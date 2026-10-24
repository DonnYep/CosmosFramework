using UnityEditor;
using UnityEngine;
using Cosmos.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using Cosmos.Input;

namespace Cosmos.Editor
{
    /// <summary>
    /// CosmosConfig 自定义面板。
    /// <para>资源部分只暴露 Provider 名称与参数：框架不再内置具体资源方案，方案由适配器程序集提供。</para>
    /// </summary>
    [CustomEditor(typeof(CosmosConfig), false)]
    public class CosmosConfigEditor : UnityEditor.Editor
    {
        SerializedObject targetObject;
        CosmosConfig cosmosConfig;

        SerializedProperty sp_LaunchAppDomainModules;
        SerializedProperty sp_PrintModulePreparatory;

        SerializedProperty sp_ResourceProviderName;
        SerializedProperty sp_ResourceProviderSettings;
        SerializedProperty sp_ResourceDiagnostics;

        SerializedProperty sp_DebugHelperIndex;
        SerializedProperty sp_DebugHelperName;
        SerializedProperty sp_JsonHelperIndex;
        SerializedProperty sp_JsonHelperName;

        SerializedProperty sp_RunInBackground;
        SerializedProperty sp_DrawDebugWindow;
        SerializedProperty sp_DisableDebugLog;

        SerializedProperty sp_InputHelperIndex;
        SerializedProperty sp_InputHelperName;

        SerializedProperty sp_ModuleConfigFoldout;

        string[] debugHelpers;
        string[] jsonHelpers;
        string[] inputHelperNames;

        string[] providerOptions;
        int providerIndex;

        bool launchAppDomainModules;
        bool moduleConfigFoldout;
        bool runInBackground;
        bool drawDebugWindow;
        bool disableDebugLog;

        public override void OnInspectorGUI()
        {
            targetObject.Update();

            DrawUtilityHelperConfig();
            EditorGUILayout.Space(16);

            DrawModuleLaunchConfig();
            EditorGUILayout.Space(16);

            DrawDebugConfig();
            EditorGUILayout.Space(16);

            DrawApplicationConfig();
            EditorGUILayout.Space(16);

            moduleConfigFoldout = EditorGUILayout.Foldout(moduleConfigFoldout, "ModuleConfig", EditorStyles.foldoutHeader);
            if (sp_ModuleConfigFoldout.boolValue != moduleConfigFoldout)
                sp_ModuleConfigFoldout.boolValue = moduleConfigFoldout;
            if (moduleConfigFoldout)
            {
                EditorGUILayout.Space(16);
                DrawResourceModuleConfig();
                EditorGUILayout.Space(16);
                DrawInputModuleConfig();
            }

            targetObject.ApplyModifiedProperties();
        }

        void OnEnable()
        {
            debugHelpers = EditorUtil.GetDerivedTypeHandlers<Utility.Debug.IDebugHelper>();
            jsonHelpers = EditorUtil.GetDerivedTypeHandlers<Utility.Json.IJsonHelper>();
            inputHelperNames = EditorUtil.GetDerivedTypeHandlers<IInputHelper>();
            providerOptions = CollectProviderNames();

            cosmosConfig = target as CosmosConfig;
            targetObject = new SerializedObject(cosmosConfig);

            sp_LaunchAppDomainModules = targetObject.FindProperty("launchAppDomainModules");
            sp_PrintModulePreparatory = targetObject.FindProperty("printModulePreparatory");

            sp_ResourceProviderName = targetObject.FindProperty("resourceProviderName");
            sp_ResourceProviderSettings = targetObject.FindProperty("resourceProviderSettings");
            sp_ResourceDiagnostics = targetObject.FindProperty("resourceDiagnostics");

            sp_DebugHelperIndex = targetObject.FindProperty("debugHelperIndex");
            sp_JsonHelperIndex = targetObject.FindProperty("jsonHelperIndex");
            sp_DebugHelperName = targetObject.FindProperty("debugHelperName");
            sp_JsonHelperName = targetObject.FindProperty("jsonHelperName");

            sp_RunInBackground = targetObject.FindProperty("runInBackground");
            sp_DrawDebugWindow = targetObject.FindProperty("drawDebugWindow");
            sp_DisableDebugLog = targetObject.FindProperty("disableDebugLog");

            sp_InputHelperIndex = targetObject.FindProperty("inputHelperIndex");
            sp_InputHelperName = targetObject.FindProperty("inputHelperName");

            sp_ModuleConfigFoldout = targetObject.FindProperty("moduleConfigFoldout");

            SyncFromSerialized();
        }

        void SyncFromSerialized()
        {
            launchAppDomainModules = sp_LaunchAppDomainModules.boolValue;
            moduleConfigFoldout = sp_ModuleConfigFoldout.boolValue;
            runInBackground = sp_RunInBackground.boolValue;
            drawDebugWindow = sp_DrawDebugWindow.boolValue;
            disableDebugLog = sp_DisableDebugLog.boolValue;
            if (sp_DebugHelperName != null && !string.IsNullOrEmpty(sp_DebugHelperName.stringValue))
                sp_DebugHelperName.stringValue = sp_DebugHelperName.stringValue;
            if (sp_JsonHelperName != null && !string.IsNullOrEmpty(sp_JsonHelperName.stringValue))
                sp_JsonHelperName.stringValue = sp_JsonHelperName.stringValue;
            if (sp_InputHelperName != null && !string.IsNullOrEmpty(sp_InputHelperName.stringValue))
                sp_InputHelperName.stringValue = sp_InputHelperName.stringValue;
            providerIndex = Math.Max(0, Array.IndexOf(providerOptions, sp_ResourceProviderName.stringValue));
            targetObject.ApplyModifiedProperties();
        }

        static string[] CollectProviderNames()
        {
            var names = new List<string>() { "AssetDatabase" };
            try
            {
                var derived = Utility.Assembly.GetDerivedTypeNames<IResourceProvider>();
                for (int i = 0; i < derived.Length; i++)
                {
                    var typeName = derived[i];
                    if (string.IsNullOrEmpty(typeName))
                        continue;
                    var shortName = typeName.Substring(typeName.LastIndexOf('.') + 1);
                    if (shortName.EndsWith("Provider", StringComparison.Ordinal))
                        shortName = shortName.Substring(0, shortName.Length - "Provider".Length);
                    if (!names.Contains(shortName))
                        names.Add(shortName);
                }
            }
            catch (Exception)
            {
                // 某些适配器程序集在缺少依赖时可能反射失败，忽略即可
            }
            names.AddRange(ResourceProviderRegistry.Names.Where(n => !names.Contains(n)));
            return names.ToArray();
        }

        void DrawUtilityHelperConfig()
        {
            EditorGUILayout.LabelField("Helpers", EditorStyles.boldLabel);
            var debugIndex = EditorGUILayout.Popup("DebugHelper", sp_DebugHelperIndex.intValue, debugHelpers);
            if (debugIndex != sp_DebugHelperIndex.intValue)
            {
                sp_DebugHelperIndex.intValue = debugIndex;
                sp_DebugHelperName.stringValue = debugHelpers[debugIndex];
            }
            var jsonIndex = EditorGUILayout.Popup("JsonHelper", sp_JsonHelperIndex.intValue, jsonHelpers);
            if (jsonIndex != sp_JsonHelperIndex.intValue)
            {
                sp_JsonHelperIndex.intValue = jsonIndex;
                sp_JsonHelperName.stringValue = jsonHelpers[jsonIndex];
            }
        }

        void DrawModuleLaunchConfig()
        {
            EditorGUILayout.LabelField("Module", EditorStyles.boldLabel);
            launchAppDomainModules = EditorGUILayout.ToggleLeft("LaunchAppDomainModules", launchAppDomainModules);
            if (launchAppDomainModules != sp_LaunchAppDomainModules.boolValue)
                sp_LaunchAppDomainModules.boolValue = launchAppDomainModules;
            if (sp_LaunchAppDomainModules.boolValue)
                sp_PrintModulePreparatory.boolValue = EditorGUILayout.ToggleLeft("PrintModulePreparatory", sp_PrintModulePreparatory.boolValue);
        }

        void DrawApplicationConfig()
        {
            EditorGUILayout.LabelField("Application", EditorStyles.boldLabel);
            runInBackground = EditorGUILayout.ToggleLeft("RunInBackground", runInBackground);
            if (runInBackground != sp_RunInBackground.boolValue)
                sp_RunInBackground.boolValue = runInBackground;
        }

        void DrawDebugConfig()
        {
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            disableDebugLog = EditorGUILayout.ToggleLeft("DisableDebugLog", disableDebugLog);
            if (disableDebugLog != sp_DisableDebugLog.boolValue)
                sp_DisableDebugLog.boolValue = disableDebugLog;
            drawDebugWindow = EditorGUILayout.ToggleLeft("DrawDebugWindow", drawDebugWindow);
            if (drawDebugWindow != sp_DrawDebugWindow.boolValue)
                sp_DrawDebugWindow.boolValue = drawDebugWindow;
        }

        void DrawResourceModuleConfig()
        {
            EditorGUILayout.LabelField("ResourceModule（可插拔 Provider）", EditorStyles.boldLabel);
            var index = EditorGUILayout.Popup("Provider", providerIndex, providerOptions);
            if (index != providerIndex)
            {
                providerIndex = index;
                sp_ResourceProviderName.stringValue = providerOptions[index];
            }
            sp_ResourceProviderName.stringValue = EditorGUILayout.TextField("Provider Name", sp_ResourceProviderName.stringValue);
            EditorGUILayout.LabelField("Settings（key=value;key2=value2）");
            sp_ResourceProviderSettings.stringValue = EditorGUILayout.TextArea(sp_ResourceProviderSettings.stringValue, GUILayout.Height(40));
            sp_ResourceDiagnostics.boolValue = EditorGUILayout.ToggleLeft("Enable Diagnostics", sp_ResourceDiagnostics.boolValue);

            EditorGUILayout.HelpBox(
                "资源方案由适配器程序集提供：\n" +
                "· Addressables → Cosmos.Resource.Addressables\n" +
                "· YooAsset → Cosmos.Resource.YooAsset\n" +
                "· QuarkAsset → Cosmos.Resource.QuarkAsset\n" +
                "· 未接入任何方案时，编辑器内使用 AssetDatabase 兜底。",
                MessageType.Info);
        }

        void DrawInputModuleConfig()
        {
            EditorGUILayout.LabelField("InputModule", EditorStyles.boldLabel);
            var index = EditorGUILayout.Popup("InputHelper", sp_InputHelperIndex.intValue, inputHelperNames);
            if (index != sp_InputHelperIndex.intValue)
            {
                sp_InputHelperIndex.intValue = index;
                sp_InputHelperName.stringValue = inputHelperNames[index];
            }
        }
    }
}
