using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Quark;
using Quark.Asset;

namespace CosmosEditor.Quark
{
    [CustomEditor(typeof(QuarkConfig), true)]
    public class QuarkConfigEditor : UnityEditor.Editor
    {
        SerializedObject targetObject;
        QuarkConfig quarkConfig;
        SerializedProperty sp_QuarkAssetLoadMode, sp_QuarkAssetDataset, sp_Url, sp_PingUrl, sp_QuarkPersistentPathType,
            sp_AESEncryptionKey, sp_UsePersistentRelativePath, sp_PersistentRelativePath, sp_CustomePersistentPath;
        bool aesKeyFoldout = false;
        public override void OnInspectorGUI()
        {
            targetObject.Update();
            sp_QuarkAssetLoadMode.enumValueIndex = (byte)(QuarkAssetLoadMode)EditorGUILayout.EnumPopup("QuarkAssetLoadMode", (QuarkAssetLoadMode)sp_QuarkAssetLoadMode.enumValueIndex);
            switch ((QuarkAssetLoadMode)sp_QuarkAssetLoadMode.enumValueIndex)
            {
                case QuarkAssetLoadMode.AssetDatabase:
                    {
                        sp_QuarkAssetDataset.objectReferenceValue = EditorGUILayout.ObjectField("QuarkAssetDataset", (QuarkAssetDataset)sp_QuarkAssetDataset.objectReferenceValue, typeof(QuarkAssetDataset), false);
                    }
                    break;
                case QuarkAssetLoadMode.BuiltAssetBundle:
                    {
                        sp_PingUrl.boolValue = EditorGUILayout.Toggle("PingUrl", sp_PingUrl.boolValue);
                        sp_Url.stringValue = EditorGUILayout.TextField("Url", sp_Url.stringValue);
                        EditorGUILayout.Space(16);
                        sp_QuarkPersistentPathType.enumValueIndex = (byte)(QuarkPersistentPathType)EditorGUILayout.EnumPopup("QuarkPersistentPathType", (QuarkPersistentPathType)sp_QuarkPersistentPathType.enumValueIndex);
                        var pathType = (QuarkPersistentPathType)sp_QuarkPersistentPathType.enumValueIndex;
                        if (pathType != QuarkPersistentPathType.CustomePersistentPath)
                        {
                            sp_UsePersistentRelativePath.boolValue = EditorGUILayout.Toggle("UsePersistentRelativePath", sp_UsePersistentRelativePath.boolValue);
                            var useRelativePath = sp_UsePersistentRelativePath.boolValue;
                            if (useRelativePath)
                            {
                                sp_PersistentRelativePath.stringValue = EditorGUILayout.TextField("PersistentRelativePath", sp_PersistentRelativePath.stringValue);
                            }
                        }
                        else
                        {
                            sp_CustomePersistentPath.stringValue = EditorGUILayout.TextField("CustomePersistentPath", sp_CustomePersistentPath.stringValue);
                        }
                        EditorGUILayout.Space(8);
                        aesKeyFoldout = EditorGUILayout.Foldout(aesKeyFoldout, "AESKey");
                        if (aesKeyFoldout)
                        {
                            EditorGUILayout.LabelField("Quark AES Key : ");
                            sp_AESEncryptionKey.stringValue = EditorGUILayout.TextField("AESEncryptionKey", sp_AESEncryptionKey.stringValue);
                        }
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
            sp_Url = targetObject.FindProperty("Url");
            sp_PingUrl = targetObject.FindProperty("PingUrl");
            sp_QuarkPersistentPathType = targetObject.FindProperty("QuarkPersistentPathType");
            sp_AESEncryptionKey = targetObject.FindProperty("AESEncryptionKey");
            sp_UsePersistentRelativePath = targetObject.FindProperty("UsePersistentRelativePath");
            sp_CustomePersistentPath = targetObject.FindProperty("CustomePersistentPath");
            sp_PersistentRelativePath = targetObject.FindProperty("PersistentRelativePath");
        }
    }
}