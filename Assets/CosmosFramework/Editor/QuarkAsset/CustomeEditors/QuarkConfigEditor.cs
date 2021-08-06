using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cosmos.Quark;
namespace Cosmos.CosmosEditor
{
    [CustomEditor(typeof(QuarkConfig), true)]
    public class QuarkConfigEditor : Editor
    {
        SerializedObject targetObject;
        QuarkConfig quarkConfig;
        SerializedProperty sp_QuarkAssetLoadMode,sp_QuarkAssetDataset,sp_Url,sp_DownloadPath, sp_PingUrl;
        public override void OnInspectorGUI()
        {
            targetObject.Update();
            sp_QuarkAssetLoadMode.enumValueIndex = (byte)(QuarkAssetLoadMode)EditorGUILayout.EnumPopup("QuarkAssetLoadMode", (QuarkAssetLoadMode)sp_QuarkAssetLoadMode.enumValueIndex);
            switch ((QuarkAssetLoadMode)sp_QuarkAssetLoadMode.enumValueIndex)
            {
                case QuarkAssetLoadMode.AssetDatabase:
                    {
                        sp_QuarkAssetDataset.objectReferenceValue = EditorGUILayout.ObjectField("QuarkAssetDataset",(QuarkAssetDataset)sp_QuarkAssetDataset.objectReferenceValue,typeof(QuarkAssetDataset),false);
                    }
                    break;
                case QuarkAssetLoadMode.BuiltAssetBundle:
                    {
                        sp_PingUrl.boolValue = EditorGUILayout.Toggle("PingUrl", sp_PingUrl.boolValue);
                        sp_Url.stringValue = EditorGUILayout.TextField("Url",sp_Url.stringValue);
                        sp_DownloadPath.stringValue = EditorGUILayout.TextField("DownloadPath",sp_DownloadPath.stringValue);
                    }
                    break;
            }
            targetObject.ApplyModifiedProperties();
        }
        private void OnEnable()
        {
            quarkConfig = target as QuarkConfig;
            targetObject = new SerializedObject(quarkConfig);
            sp_QuarkAssetLoadMode = targetObject.FindProperty("QuarkAssetLoadMode");
            sp_QuarkAssetDataset = targetObject.FindProperty("QuarkAssetDataset");
            sp_Url= targetObject.FindProperty("Url");
            sp_DownloadPath= targetObject.FindProperty("DownloadPath");
            sp_PingUrl= targetObject.FindProperty("PingUrl");
        }
    }
}