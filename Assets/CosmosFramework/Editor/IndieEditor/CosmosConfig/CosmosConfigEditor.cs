using UnityEditor;
using Cosmos.Resource;
using System;
using System.Linq;
using Cosmos.Input;

namespace Cosmos.Editor
{
    [CustomEditor(typeof(CosmosConfig), false)]
    public class CosmosConfigEditor : UnityEditor.Editor
    {
        SerializedObject targetObject;
        CosmosConfig cosmosConfig;
        SerializedProperty sp_LaunchAppDomainModules;
        SerializedProperty sp_PrintModulePreparatory;

        SerializedProperty sp_ResourceLoadMode;
        SerializedProperty sp_ResourceLoaderName;
        SerializedProperty sp_ResourceLoaderIndex;
        SerializedProperty sp_ResourceDataset;
        SerializedProperty sp_ResourceBundlePathType;
        SerializedProperty sp_RelativeBundlePath;

        SerializedProperty sp_AssetBundleEncrytion;
        SerializedProperty sp_AssetBundleEncrytionOffset;
        SerializedProperty sp_ManifestEncrytion;
        SerializedProperty sp_ManifestEncrytionKey;


        SerializedProperty sp_DebugHelperIndex;
        SerializedProperty sp_JsonHelperIndex;
        SerializedProperty sp_MessagePackHelperIndex;

        SerializedProperty sp_DebugHelperName;
        SerializedProperty sp_JsonHelperName;
        SerializedProperty sp_MessagePackHelperName;

        SerializedProperty sp_RunInBackground;

        SerializedProperty sp_DrawDebugWindow;

        SerializedProperty sp_InputHelperIndex;
        SerializedProperty sp_InputHelperName;

        string[] debugHelpers;
        string[] jsonHelpers;
        string[] messagePackHelpers;

        int debugHelperIndex;
        int jsonHelperIndex;
        int messagePackHelperIndex;

        bool assetBundleEncrytion;
        bool manifestEncrytion;

        bool launchAppDomainModules;
        int resourceLoadModeIndex;
        int resourceLoaderIndex;
        int resourceBundlePathTypeIndex;//这里使用index存储enum的写法能够避免点击到脚本时，场景发生改变以至于可以存储的问题

        string[] resourceLoaders;
        string[] resourceLoadModes;
        string[] resourceBundlePathTypes;


        int inputHelperIndex;
        string[] InputHelperNames;


        bool runInBackground;
        bool drawDebugWindow;
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

            DrawResourceModuleConfig();
            EditorGUILayout.Space(16);

            DrawInputModuleConfig();

            targetObject.ApplyModifiedProperties();
        }
        private void OnEnable()
        {
            var debugSrc = Utility.Assembly.GetDerivedTypeNames<Utility.Debug.IDebugHelper>();
            debugHelpers = new string[debugSrc.Length + 1];
            debugHelpers[0] = Constants.NONE;
            Array.Copy(debugSrc, 0, debugHelpers, 1, debugSrc.Length);

            var jsonSrc = Utility.Assembly.GetDerivedTypeNames<Utility.Json.IJsonHelper>();
            jsonHelpers = new string[jsonSrc.Length + 1];
            jsonHelpers[0] = Constants.NONE;
            Array.Copy(jsonSrc, 0, jsonHelpers, 1, jsonSrc.Length);

            var msgPackSrc = Utility.Assembly.GetDerivedTypeNames<Utility.MessagePack.IMessagePackHelper>();
            messagePackHelpers = new string[msgPackSrc.Length + 1];
            messagePackHelpers[0] = Constants.NONE;
            Array.Copy(msgPackSrc, 0, messagePackHelpers, 1, msgPackSrc.Length);

            var srcLoaders = Utility.Assembly.GetDerivedTypeNames<IResourceLoadHelper>();
            var filteredLoader = resourceLoaders = srcLoaders.Where(l =>
             {
                 return l != typeof(AssetDatabaseLoader).FullName &&
                   l != typeof(ResourcesLoader).FullName &&
                   l != typeof(AssetBundleLoader).FullName;
             }).ToArray();
            resourceLoaders = new string[filteredLoader.Length + 1];
            resourceLoaders[0] = Constants.NONE;
            Array.Copy(srcLoaders, 0, resourceLoaders, 1, filteredLoader.Length);

            var inputLoaders = Utility.Assembly.GetDerivedTypeNames<IInputHelper>();
            InputHelperNames = new string[inputLoaders.Length + 1];
            InputHelperNames[0] = Constants.NONE;
            Array.Copy(inputLoaders, 0, InputHelperNames, 1, inputLoaders.Length);

            cosmosConfig = target as CosmosConfig;
            targetObject = new SerializedObject(cosmosConfig);

            sp_LaunchAppDomainModules = targetObject.FindProperty("launchAppDomainModules");
            sp_PrintModulePreparatory = targetObject.FindProperty("printModulePreparatory");
            sp_ResourceLoadMode = targetObject.FindProperty("resourceLoadMode");
            sp_ResourceLoaderName = targetObject.FindProperty("resourceLoaderName");
            sp_ResourceLoaderIndex = targetObject.FindProperty("resourceLoaderIndex");

            sp_DebugHelperIndex = targetObject.FindProperty("debugHelperIndex");
            sp_JsonHelperIndex = targetObject.FindProperty("jsonHelperIndex");
            sp_MessagePackHelperIndex = targetObject.FindProperty("messagePackHelperIndex");

            sp_DebugHelperName = targetObject.FindProperty("debugHelperName");
            sp_JsonHelperName = targetObject.FindProperty("jsonHelperName");
            sp_MessagePackHelperName = targetObject.FindProperty("messagePackHelperName");
            sp_RunInBackground = targetObject.FindProperty("runInBackground");
            sp_ResourceDataset = targetObject.FindProperty("resourceDataset");
            sp_ResourceBundlePathType = targetObject.FindProperty("resourceBundlePathType");
            sp_RelativeBundlePath = targetObject.FindProperty("relativeBundlePath");

            sp_AssetBundleEncrytion = targetObject.FindProperty("assetBundleEncrytion");
            sp_AssetBundleEncrytionOffset = targetObject.FindProperty("assetBundleEncrytionOffset");
            sp_ManifestEncrytion = targetObject.FindProperty("manifestEncrytion");
            sp_ManifestEncrytionKey = targetObject.FindProperty("manifestEncrytionKey");

            sp_DrawDebugWindow = targetObject.FindProperty("drawDebugWindow");

            sp_InputHelperIndex = targetObject.FindProperty("inputHelperIndex");
            sp_InputHelperName = targetObject.FindProperty("inputHelperName");

            resourceLoadModes = Enum.GetNames(typeof(ResourceLoadMode));
            resourceBundlePathTypes = Enum.GetNames(typeof(ResourceBundlePathType));
            RefreshConfig();
        }
        void RefreshConfig()
        {
            debugHelperIndex = sp_DebugHelperIndex.intValue;
            jsonHelperIndex = sp_JsonHelperIndex.intValue;
            messagePackHelperIndex = sp_MessagePackHelperIndex.intValue;
            launchAppDomainModules = sp_LaunchAppDomainModules.boolValue;
            resourceLoadModeIndex = sp_ResourceLoadMode.enumValueIndex;
            resourceLoaderIndex = sp_ResourceLoaderIndex.intValue;
            sp_DebugHelperName.stringValue = debugHelpers[sp_DebugHelperIndex.intValue];
            sp_JsonHelperName.stringValue = jsonHelpers[sp_JsonHelperIndex.intValue];
            sp_MessagePackHelperName.stringValue = messagePackHelpers[sp_MessagePackHelperIndex.intValue];
            sp_ResourceLoaderName.stringValue = sp_ResourceLoaderName.stringValue;
            runInBackground = sp_RunInBackground.boolValue;
            resourceBundlePathTypeIndex = sp_ResourceBundlePathType.enumValueIndex;

            assetBundleEncrytion = sp_AssetBundleEncrytion.boolValue;
            manifestEncrytion = sp_ManifestEncrytion.boolValue;

            inputHelperIndex = sp_InputHelperIndex.intValue;
            sp_InputHelperName.stringValue = InputHelperNames[inputHelperIndex];

            targetObject.ApplyModifiedProperties();
        }
        void DrawUtilityHelperConfig()
        {
            EditorGUILayout.LabelField("Helpers", EditorStyles.boldLabel);
            debugHelperIndex = EditorGUILayout.Popup("DebugHelper", debugHelperIndex, debugHelpers);
            if (debugHelperIndex != sp_DebugHelperIndex.intValue)
            {
                sp_DebugHelperIndex.intValue = debugHelperIndex;
                sp_DebugHelperName.stringValue = debugHelpers[sp_DebugHelperIndex.intValue];
            }

            jsonHelperIndex = EditorGUILayout.Popup("JsonHelper", jsonHelperIndex, jsonHelpers);
            if (jsonHelperIndex != sp_JsonHelperIndex.intValue)
            {
                sp_JsonHelperIndex.intValue = jsonHelperIndex;
                sp_JsonHelperName.stringValue = jsonHelpers[sp_JsonHelperIndex.intValue];
            }

            messagePackHelperIndex = EditorGUILayout.Popup("MessagePackHelper", messagePackHelperIndex, messagePackHelpers);
            if (messagePackHelperIndex != sp_MessagePackHelperIndex.intValue)
            {
                sp_MessagePackHelperIndex.intValue = messagePackHelperIndex;
                sp_MessagePackHelperName.stringValue = messagePackHelpers[sp_MessagePackHelperIndex.intValue];
            }
        }
        void DrawModuleLaunchConfig()
        {
            EditorGUILayout.LabelField("Module", EditorStyles.boldLabel);
            launchAppDomainModules = EditorGUILayout.ToggleLeft("LaunchAppDomainModules", launchAppDomainModules);
            if (launchAppDomainModules != sp_LaunchAppDomainModules.boolValue)
            {
                sp_LaunchAppDomainModules.boolValue = launchAppDomainModules;
            }
            if (sp_LaunchAppDomainModules.boolValue)
            {
                sp_PrintModulePreparatory.boolValue = EditorGUILayout.ToggleLeft("PrintModulePreparatory", sp_PrintModulePreparatory.boolValue);
            }
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
            drawDebugWindow = EditorGUILayout.ToggleLeft("DrawDebugWindow", drawDebugWindow);
            if (drawDebugWindow != sp_DrawDebugWindow.boolValue)
                sp_DrawDebugWindow.boolValue = drawDebugWindow;
        }
        void DrawResourceModuleConfig()
        {
            EditorGUILayout.LabelField("ResourceModule", EditorStyles.boldLabel);
            resourceLoadModeIndex = EditorGUILayout.Popup("ResourceLoadMode", resourceLoadModeIndex, resourceLoadModes);
            if (resourceLoadModeIndex != sp_ResourceLoadMode.enumValueIndex)
            {
                sp_ResourceLoadMode.enumValueIndex = resourceLoadModeIndex;
            }
            var resourceLoadMode = (ResourceLoadMode)resourceLoadModeIndex;
            switch (resourceLoadMode)
            {
                case ResourceLoadMode.NONE:
                    EditorGUILayout.LabelField(Constants.NONE);
                    break;
                case ResourceLoadMode.Resource:
                    EditorGUILayout.LabelField(typeof(UnityEngine.Resources).FullName);
                    break;
                case ResourceLoadMode.AssetBundle:
                    {
                        resourceBundlePathTypeIndex = EditorGUILayout.Popup("AssetBundlePathType", resourceBundlePathTypeIndex, resourceBundlePathTypes);
                        if (resourceBundlePathTypeIndex != sp_ResourceBundlePathType.enumValueIndex)
                        {
                            sp_ResourceBundlePathType.enumValueIndex = resourceBundlePathTypeIndex;
                        }
                        sp_RelativeBundlePath.stringValue = EditorGUILayout.TextField("RelativeBundlePath", sp_RelativeBundlePath.stringValue);
                        assetBundleEncrytion = EditorGUILayout.ToggleLeft("AssetBundleEncrytion", assetBundleEncrytion);
                        if (assetBundleEncrytion != sp_AssetBundleEncrytion.boolValue)
                        {
                            sp_AssetBundleEncrytion.boolValue = assetBundleEncrytion;
                        }
                        if (assetBundleEncrytion)
                        {
                            sp_AssetBundleEncrytionOffset.intValue = EditorGUILayout.IntField("Offset", sp_AssetBundleEncrytionOffset.intValue);
                        }

                        manifestEncrytion = EditorGUILayout.ToggleLeft("ManifestEncrytion", manifestEncrytion);
                        if (manifestEncrytion != sp_ManifestEncrytion.boolValue)
                        {
                            sp_ManifestEncrytion.boolValue = manifestEncrytion;
                        }
                        if (manifestEncrytion)
                        {
                            sp_ManifestEncrytionKey.stringValue = EditorGUILayout.TextField("Key", sp_ManifestEncrytionKey.stringValue);
                        }
                    }
                    break;
                case ResourceLoadMode.AssetDatabase:
                    {
                        sp_ResourceDataset.objectReferenceValue = (ResourceDataset)EditorGUILayout.
                            ObjectField("ResourceDataset", sp_ResourceDataset.objectReferenceValue, typeof(ResourceDataset), false);
                    }
                    break;
                case ResourceLoadMode.CustomLoader:
                    {
                        resourceLoaderIndex = EditorGUILayout.Popup("ResourceLoadHelper", resourceLoaderIndex, resourceLoaders);
                        if (resourceLoaderIndex != sp_ResourceLoaderIndex.intValue)
                        {
                            sp_ResourceLoaderIndex.intValue = resourceLoaderIndex;
                            sp_ResourceLoaderName.stringValue = resourceLoaders[sp_ResourceLoaderIndex.intValue];
                        }
                    }
                    break;
            }
        }
        void DrawInputModuleConfig()
        {
            EditorGUILayout.LabelField("InputModule", EditorStyles.boldLabel);
            inputHelperIndex = EditorGUILayout.Popup("InputHelper", inputHelperIndex, InputHelperNames);
            if (inputHelperIndex != sp_InputHelperIndex.intValue)
            {
                sp_InputHelperIndex.intValue = inputHelperIndex;
                sp_InputHelperName.stringValue = InputHelperNames[inputHelperIndex];
            }

        }
    }
}