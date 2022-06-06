using UnityEditor;
using Cosmos.Resource;
using System;
using UnityEngine;
namespace Cosmos.Editor
{
    [CustomEditor(typeof(CosmosConfig), true)]
    public class CosmosConfigEditor : UnityEditor.Editor
    {
        SerializedObject targetObject;
        CosmosConfig cosmosConfig;
        SerializedProperty sp_LaunchAppDomainModules;
        SerializedProperty sp_PrintModulePreparatory;
        SerializedProperty sp_ResourceLoadMode;

        SerializedProperty sp_DebugHelperIndex;
        SerializedProperty sp_JsonHelperIndex;
        SerializedProperty sp_MessagePackHelperIndex;

        SerializedProperty sp_DebugHelperName;
        SerializedProperty sp_JsonHelperName;
        SerializedProperty sp_MessagePackHelperName;

        string[] debugHelpers;
        string[] jsonHelpers;
        string[] messagePackHelpers;

        public override void OnInspectorGUI()
        {
            targetObject.Update();
            EditorGUILayout.LabelField("CosmosEntryConfig", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Helpers", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("DebugHelper", GUILayout.Width(128));
            sp_DebugHelperIndex.intValue = EditorGUILayout.Popup(sp_DebugHelperIndex.intValue, debugHelpers);
            sp_DebugHelperName.stringValue = debugHelpers[sp_DebugHelperIndex.intValue];
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("JsonHelper", GUILayout.Width(128));
            sp_JsonHelperIndex.intValue = EditorGUILayout.Popup(sp_JsonHelperIndex.intValue, jsonHelpers);
            sp_JsonHelperName.stringValue = jsonHelpers[sp_JsonHelperIndex.intValue];
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MessagePack", GUILayout.Width(128));
            sp_MessagePackHelperIndex.intValue = EditorGUILayout.Popup(sp_MessagePackHelperIndex.intValue, messagePackHelpers);
            sp_MessagePackHelperName.stringValue = messagePackHelpers[sp_MessagePackHelperIndex.intValue];
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            sp_LaunchAppDomainModules.boolValue = EditorGUILayout.Toggle("LaunchAppDomainModules", sp_LaunchAppDomainModules.boolValue);
            sp_PrintModulePreparatory.boolValue = EditorGUILayout.Toggle("PrintModulePreparatory", sp_PrintModulePreparatory.boolValue);
            sp_ResourceLoadMode.enumValueIndex = (byte)(ResourceLoadMode)EditorGUILayout.EnumPopup("ResourceLoadMode", (ResourceLoadMode)sp_ResourceLoadMode.enumValueIndex);
            targetObject.ApplyModifiedProperties();
        }
        private void OnEnable()
        {
            debugHelpers = Utility.Assembly.GetDerivedTypeNames<Utility.Debug.IDebugHelper>(AppDomain.CurrentDomain.GetAssemblies());
            jsonHelpers = Utility.Assembly.GetDerivedTypeNames<Utility.Json.IJsonHelper>(AppDomain.CurrentDomain.GetAssemblies());
            messagePackHelpers = Utility.Assembly.GetDerivedTypeNames<Utility.MessagePack.IMessagePackHelper>(AppDomain.CurrentDomain.GetAssemblies());
            cosmosConfig = target as CosmosConfig;
            targetObject = new SerializedObject(cosmosConfig);
            sp_LaunchAppDomainModules = targetObject.FindProperty("launchAppDomainModules");
            sp_PrintModulePreparatory = targetObject.FindProperty("printModulePreparatory");
            sp_ResourceLoadMode = targetObject.FindProperty("resourceLoadMode");

            sp_DebugHelperIndex = targetObject.FindProperty("debugHelperIndex");
            sp_JsonHelperIndex = targetObject.FindProperty("jsonHelperIndex");
            sp_MessagePackHelperIndex = targetObject.FindProperty("messagePackHelperIndex");

            sp_DebugHelperName = targetObject.FindProperty("debugHelperName");
            sp_JsonHelperName = targetObject.FindProperty("jsonHelperName");
            sp_MessagePackHelperName = targetObject.FindProperty("messagePackHelperName");

        }
    }
}