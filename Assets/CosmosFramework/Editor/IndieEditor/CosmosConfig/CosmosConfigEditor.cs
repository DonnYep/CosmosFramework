using UnityEditor;
using Cosmos.Resource;
using System;
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

        bool launchAppDomainModules;
        int resourceLoadModeIndex;
        int resourceLoaderIndex;

        string[] resourceLoaders;
        string[] resourceLoadModes;

        bool runInBackground;
        public override void OnInspectorGUI()
        {
            targetObject.Update();
            EditorGUILayout.LabelField("CosmosConfig", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");
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
            EditorGUILayout.EndVertical();

            launchAppDomainModules = EditorGUILayout.Toggle("LaunchAppDomainModules", launchAppDomainModules);
            if (launchAppDomainModules != sp_LaunchAppDomainModules.boolValue)
            {
                sp_LaunchAppDomainModules.boolValue = launchAppDomainModules;
            }
            if (sp_LaunchAppDomainModules.boolValue)
            {
                sp_PrintModulePreparatory.boolValue = EditorGUILayout.Toggle("PrintModulePreparatory", sp_PrintModulePreparatory.boolValue);
            }

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("ResourceLoadConfig", EditorStyles.boldLabel);
            resourceLoadModeIndex = EditorGUILayout.Popup("ResourceLoadMode", resourceLoadModeIndex, resourceLoadModes);
            if (resourceLoadModeIndex != sp_ResourceLoadMode.enumValueIndex)
            {
                sp_ResourceLoadMode.enumValueIndex = resourceLoadModeIndex;
            }

            resourceLoaderIndex = EditorGUILayout.Popup("ResourceLoadHelper", resourceLoaderIndex, resourceLoaders);
            if (resourceLoaderIndex != sp_ResourceLoaderIndex.intValue)
            {
                sp_ResourceLoaderIndex.intValue = resourceLoaderIndex;
                sp_ResourceLoaderName.stringValue = resourceLoaders[sp_ResourceLoaderIndex.intValue];
            }
            EditorGUILayout.EndVertical();

            runInBackground = EditorGUILayout.Toggle("RunInBackground", runInBackground);
            if (runInBackground != sp_RunInBackground.boolValue)
                sp_RunInBackground.boolValue = runInBackground;

            targetObject.ApplyModifiedProperties();
        }
        private void OnEnable()
        {
            debugHelpers = Utility.Assembly.GetDerivedTypeNames<Utility.Debug.IDebugHelper>(AppDomain.CurrentDomain.GetAssemblies());
            jsonHelpers = Utility.Assembly.GetDerivedTypeNames<Utility.Json.IJsonHelper>(AppDomain.CurrentDomain.GetAssemblies());
            messagePackHelpers = Utility.Assembly.GetDerivedTypeNames<Utility.MessagePack.IMessagePackHelper>(AppDomain.CurrentDomain.GetAssemblies());
            resourceLoaders = Utility.Assembly.GetDerivedTypeNames<IResourceLoadHelper>(AppDomain.CurrentDomain.GetAssemblies());
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
            resourceLoadModes = Enum.GetNames(typeof(ResourceLoadMode));
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
            sp_ResourceLoaderName.stringValue = resourceLoaders[sp_ResourceLoaderIndex.intValue];
            runInBackground = sp_RunInBackground.boolValue;
            targetObject.ApplyModifiedProperties();
        }
    }
}