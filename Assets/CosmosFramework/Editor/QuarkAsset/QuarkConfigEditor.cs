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
        SerializedProperty sp_QuarkAssetLoadMode,sp_QuarkAssetDataset,sp_url,sp_downloadPath;
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
                        sp_url.stringValue = EditorGUILayout.TextField("Url",sp_url.stringValue);
                        sp_downloadPath.stringValue = EditorGUILayout.TextField("DownloadPath",sp_downloadPath.stringValue);
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
            sp_url= targetObject.FindProperty("url");
            sp_downloadPath= targetObject.FindProperty("downloadPath");
        }
    }
}