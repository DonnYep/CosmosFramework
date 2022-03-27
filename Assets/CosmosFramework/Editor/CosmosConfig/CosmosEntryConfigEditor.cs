using UnityEditor;
using Cosmos.Resource;
namespace Cosmos.Editor
{
    [CustomEditor(typeof(CosmosEntryConfig), true)]
    public class CosmosEntryConfigEditor : UnityEditor.Editor
    {
        SerializedObject targetObject;
        CosmosEntryConfig cosmosConfig;
        SerializedProperty sp_LoadDefaultHelper, sp_LaunchAppDomainModules, sp_PrintModulePreparatory,
           sp_ResourceLoadMode;
        public override void OnInspectorGUI()
        {
            targetObject.Update();
            sp_LoadDefaultHelper.boolValue = EditorGUILayout.Toggle("LoadDefaultHelper", sp_LoadDefaultHelper.boolValue);
            sp_LaunchAppDomainModules.boolValue = EditorGUILayout.Toggle("LaunchAppDomainModules", sp_LaunchAppDomainModules.boolValue);
            if (sp_LaunchAppDomainModules.boolValue)
            {
                sp_PrintModulePreparatory.boolValue = EditorGUILayout.Toggle("PrintModulePreparatory", sp_PrintModulePreparatory.boolValue);
            }
            sp_ResourceLoadMode.enumValueIndex = (byte)(ResourceLoadMode)EditorGUILayout.EnumPopup("ResourceLoadMode", (ResourceLoadMode)sp_ResourceLoadMode.enumValueIndex);
            targetObject.ApplyModifiedProperties();
        }
        private void OnEnable()
        {
            cosmosConfig = target as CosmosEntryConfig;
            targetObject = new SerializedObject(cosmosConfig);
            sp_LoadDefaultHelper = targetObject.FindProperty("LoadDefaultHelper");
            sp_LaunchAppDomainModules = targetObject.FindProperty("LaunchAppDomainModules");
            sp_PrintModulePreparatory = targetObject.FindProperty("PrintModulePreparatory");
            sp_ResourceLoadMode = targetObject.FindProperty("ResourceLoadMode");
        }
    }
}