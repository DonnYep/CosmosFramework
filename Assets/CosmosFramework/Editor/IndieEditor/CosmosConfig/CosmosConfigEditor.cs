using UnityEditor;
using Cosmos.Resource;
using System;
using System.Linq;

namespace Cosmos.Editor
{
    [CustomEditor(typeof(CosmosConfig), false)]
    public class CosmosConfigEditor : UnityEditor.Editor
    {
        SerializedObject targetObject;
        CosmosConfig cosmosConfig;
        SerializedProperty sp_LaunchAppDomainModules;
        SerializedProperty sp_PrintModulePreparatory;

        SerializedProperty sp_AutoInitResource;
        SerializedProperty sp_ResourceLoadMode;
        SerializedProperty sp_ResourceLoaderName;
        SerializedProperty sp_ResourceLoaderIndex;
        SerializedProperty sp_ResourceDataset;
        SerializedProperty sp_ResourceBundlePathType;
        SerializedProperty sp_RelativeBundlePath;
        SerializedProperty sp_CustomeResourceBundlePath;

        SerializedProperty sp_AssetBundleEncrytion;
        SerializedProperty sp_AssetBundleEncrytionOffset;
        SerializedProperty sp_BuildInfoEncrytion;
        SerializedProperty sp_BuildInfoEncrytionKey;


        SerializedProperty sp_DebugHelperIndex;
        SerializedProperty sp_JsonHelperIndex;
        SerializedProperty sp_MessagePackHelperIndex;

        SerializedProperty sp_DebugHelperName;
        SerializedProperty sp_JsonHelperName;
        SerializedProperty sp_MessagePackHelperName;

        SerializedProperty sp_RunInBackground;

        string[] debugHelpers;
        string[] jsonHelpers;
        string[] messagePackHelpers;

        int debugHelperIndex;
        int jsonHelperIndex;
        int messagePackHelperIndex;

        bool assetBundleEncrytion;
        bool buildInfoEncrytion;

        bool launchAppDomainModules;
        int resourceLoadModeIndex;
        int resourceLoaderIndex;
        int resourceBundlePathTypeIndex;//这里使用index存储enum的写法能够避免点击到脚本时，场景发生改变以至于可以存储的问题

        string[] resourceLoaders;
        string[] resourceLoadModes;
        string[] resourceBundlePathTypes;

        bool runInBackground;

        public override void OnInspectorGUI()
        {
            targetObject.Update();
            EditorGUILayout.BeginVertical();
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
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(16);
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

            EditorGUILayout.Space(16);

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("ResourceLoadConfig", EditorStyles.boldLabel);
                sp_AutoInitResource.boolValue = EditorGUILayout.ToggleLeft("AutoInitResource", sp_AutoInitResource.boolValue);
                if (sp_AutoInitResource.boolValue)
                {
                    resourceLoadModeIndex = EditorGUILayout.Popup("ResourceLoadMode", resourceLoadModeIndex, resourceLoadModes);
                    if (resourceLoadModeIndex != sp_ResourceLoadMode.enumValueIndex)
                    {
                        sp_ResourceLoadMode.enumValueIndex = resourceLoadModeIndex;
                    }
                    var resourceLoadMode = (ResourceLoadMode)resourceLoadModeIndex;
                    switch (resourceLoadMode)
                    {
                        case ResourceLoadMode.Resource:
                            break;
                        case ResourceLoadMode.AssetBundle:
                            {
                                resourceBundlePathTypeIndex = EditorGUILayout.Popup("ResourceBundlePathType", resourceBundlePathTypeIndex, resourceBundlePathTypes);
                                if (resourceBundlePathTypeIndex != sp_ResourceBundlePathType.enumValueIndex)
                                {
                                    sp_ResourceBundlePathType.enumValueIndex = resourceBundlePathTypeIndex;
                                }
                                var bundlePathType = (ResourceBundlePathType)resourceBundlePathTypeIndex;
                                if (bundlePathType == ResourceBundlePathType.CustomePath)
                                {
                                    sp_CustomeResourceBundlePath.stringValue = EditorGUILayout.TextField("CustomePath", sp_CustomeResourceBundlePath.stringValue);
                                }
                                else
                                {
                                    sp_RelativeBundlePath.stringValue = EditorGUILayout.TextField("RelativeBundlePath", sp_RelativeBundlePath.stringValue);
                                }
                                assetBundleEncrytion = EditorGUILayout.ToggleLeft("AssetBundleEncrytion", assetBundleEncrytion);
                                if (assetBundleEncrytion != sp_AssetBundleEncrytion.boolValue)
                                {
                                    sp_AssetBundleEncrytion.boolValue = assetBundleEncrytion;
                                }
                                if (assetBundleEncrytion)
                                {
                                    sp_AssetBundleEncrytionOffset.intValue = EditorGUILayout.IntField("AssetBundleEncrytionOffset", sp_AssetBundleEncrytionOffset.intValue);
                                }

                                buildInfoEncrytion = EditorGUILayout.ToggleLeft("BuildInfoEncrytion", buildInfoEncrytion);
                                if (buildInfoEncrytion != sp_BuildInfoEncrytion.boolValue)
                                {
                                    sp_BuildInfoEncrytion.boolValue = buildInfoEncrytion;
                                }
                                if (buildInfoEncrytion)
                                {
                                    sp_BuildInfoEncrytionKey.stringValue = EditorGUILayout.TextField("BuildInfoEncrytionKey", sp_BuildInfoEncrytionKey.stringValue);
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
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(16);
            EditorGUILayout.LabelField("Application", EditorStyles.boldLabel);
            runInBackground = EditorGUILayout.ToggleLeft("RunInBackground", runInBackground);
            if (runInBackground != sp_RunInBackground.boolValue)
                sp_RunInBackground.boolValue = runInBackground;

            targetObject.ApplyModifiedProperties();
        }
        private void OnEnable()
        {
            var debugSrc = Utility.Assembly.GetDerivedTypeNames<Utility.Debug.IDebugHelper>();
            debugHelpers = new string[debugSrc.Length + 1];
            debugHelpers[0] = CosmosConfig.NONE;
            Array.Copy(debugSrc, 0, debugHelpers, 1, debugSrc.Length);

            var jsonSrc = Utility.Assembly.GetDerivedTypeNames<Utility.Json.IJsonHelper>();
            jsonHelpers = new string[jsonSrc.Length + 1];
            jsonHelpers[0] = CosmosConfig.NONE;
            Array.Copy(jsonSrc, 0, jsonHelpers, 1, jsonSrc.Length);

            var msgPackSrc = Utility.Assembly.GetDerivedTypeNames<Utility.MessagePack.IMessagePackHelper>();
            messagePackHelpers = new string[msgPackSrc.Length + 1];
            messagePackHelpers[0] = CosmosConfig.NONE;
            Array.Copy(msgPackSrc, 0, messagePackHelpers, 1, msgPackSrc.Length);

            var srcLoaders = Utility.Assembly.GetDerivedTypeNames<IResourceLoadHelper>();
            var filteredLoader = resourceLoaders = srcLoaders.Where(l =>
             {
                 return l != typeof(AssetDatabaseLoader).FullName &&
                   l != typeof(ResourcesLoader).FullName &&
                   l != typeof(AssetBundleLoader).FullName;
             }).ToArray();
            resourceLoaders = new string[filteredLoader.Length + 1];
            resourceLoaders[0] = CosmosConfig.NONE;
            Array.Copy(srcLoaders, 0, resourceLoaders, 1, filteredLoader.Length);

            cosmosConfig = target as CosmosConfig;
            targetObject = new SerializedObject(cosmosConfig);

            sp_LaunchAppDomainModules = targetObject.FindProperty("launchAppDomainModules");
            sp_PrintModulePreparatory = targetObject.FindProperty("printModulePreparatory");
            sp_AutoInitResource = targetObject.FindProperty("autoInitResource");
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
            sp_CustomeResourceBundlePath = targetObject.FindProperty("customeResourceBundlePath");

            sp_AssetBundleEncrytion = targetObject.FindProperty("assetBundleEncrytion");
            sp_AssetBundleEncrytionOffset = targetObject.FindProperty("assetBundleEncrytionOffset");
            sp_BuildInfoEncrytion = targetObject.FindProperty("buildInfoEncrytion");
            sp_BuildInfoEncrytionKey = targetObject.FindProperty("buildInfoEncrytionKey");

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
            buildInfoEncrytion = sp_BuildInfoEncrytion.boolValue;

            targetObject.ApplyModifiedProperties();
        }
    }
}