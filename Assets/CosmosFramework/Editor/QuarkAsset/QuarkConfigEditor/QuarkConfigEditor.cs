﻿using UnityEditor;
using Quark.Asset;
namespace Quark.Editor
{
    [CustomEditor(typeof(QuarkConfig), true)]
    public class QuarkConfigEditor : UnityEditor.Editor
    {
        SerializedObject targetObject;
        QuarkConfig quarkConfig;
        bool encryptionToggle;
        SerializedProperty sp_QuarkAssetLoadMode, sp_QuarkAssetDataset, sp_Url, sp_PingUrl, sp_QuarkBuildPath,
             sp_EnableRelativeLoadPath, sp_RelativeLoadPath, sp_CustomeAbsolutePath, sp_EnableRelativeBuildPath,
            sp_RelativeBuildPath, sp_QuarkDownloadedPath, sp_EncryptionOffset, sp_BuildInfoAESEncryptionKey;
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
                        DrawBuildAssetBundleTab();
                        EditorGUILayout.Space(8);
                        encryptionToggle = EditorGUILayout.Foldout(encryptionToggle, "Encryption");
                        if (encryptionToggle)
                        {
                            DrawOffstEncryption();
                            DrawAESEncryption();
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
            sp_QuarkDownloadedPath = targetObject.FindProperty("QuarkDownloadedPath");
            sp_EncryptionOffset = targetObject.FindProperty("EncryptionOffset");
            sp_EnableRelativeLoadPath = targetObject.FindProperty("EnableRelativeLoadPath");
            sp_CustomeAbsolutePath = targetObject.FindProperty("CustomeAbsolutePath");
            sp_RelativeLoadPath = targetObject.FindProperty("RelativeLoadPath");
            sp_BuildInfoAESEncryptionKey = targetObject.FindProperty("BuildInfoAESEncryptionKey");
            sp_QuarkBuildPath = targetObject.FindProperty("QuarkBuildPath");
            sp_EnableRelativeBuildPath = targetObject.FindProperty("EnableRelativeBuildPath");
            sp_RelativeBuildPath = targetObject.FindProperty("RelativeBuildPath");
        }
        void DrawBuildAssetBundleTab()
        {
            EditorGUILayout.HelpBox("Asset bundle build path", MessageType.Info);

            EditorGUILayout.BeginVertical();
            sp_QuarkBuildPath.enumValueIndex = (byte)(QuarkBuildPath)EditorGUILayout.EnumPopup("QuarkBuildPath", (QuarkBuildPath)sp_QuarkBuildPath.enumValueIndex);
            var buildType = (QuarkBuildPath)sp_QuarkBuildPath.enumValueIndex;
            switch (buildType)
            {
                case QuarkBuildPath.StreamingAssets:
                    {
                        sp_EnableRelativeBuildPath.boolValue = EditorGUILayout.Toggle("EnableRelativeBuildPath", sp_EnableRelativeBuildPath.boolValue);
                        var useRelativePath = sp_EnableRelativeBuildPath.boolValue;
                        if (useRelativePath)
                        {
                            sp_RelativeBuildPath.stringValue = EditorGUILayout.TextField("RelativeBuildPath", sp_RelativeBuildPath.stringValue.Trim());
                        }
                    }
                    break;
                case QuarkBuildPath.URL:
                    {
                        sp_PingUrl.boolValue = EditorGUILayout.Toggle("PingUrl", sp_PingUrl.boolValue);
                        sp_Url.stringValue = EditorGUILayout.TextField("Url", sp_Url.stringValue.Trim());
                        EditorGUILayout.BeginVertical();
                        sp_QuarkDownloadedPath.enumValueIndex = (byte)(QuarkDownloadedPath)EditorGUILayout.EnumPopup("QuarkDownloadPath", (QuarkDownloadedPath)sp_QuarkDownloadedPath.enumValueIndex);
                        var pathType = (QuarkDownloadedPath)sp_QuarkDownloadedPath.enumValueIndex;
                        if (pathType != QuarkDownloadedPath.Custome)
                        {
                            sp_EnableRelativeLoadPath.boolValue = EditorGUILayout.Toggle("UseRelativeLoadPath", sp_EnableRelativeLoadPath.boolValue);
                            var useRelativePath = sp_EnableRelativeLoadPath.boolValue;
                            if (useRelativePath)
                            {
                                sp_RelativeLoadPath.stringValue = EditorGUILayout.TextField("RelativeLoadPath", sp_RelativeLoadPath.stringValue.Trim());
                            }
                        }
                        else
                        {
                            sp_CustomeAbsolutePath.stringValue = EditorGUILayout.TextField("CustomeAbsolutePath", sp_CustomeAbsolutePath.stringValue.Trim());
                        }
                        EditorGUILayout.EndVertical();

                    }
                    break;
            }
            EditorGUILayout.EndVertical();
        }
        void DrawOffstEncryption()
        {
            sp_EncryptionOffset.longValue = EditorGUILayout.LongField("QuarkEncryptOffset", sp_EncryptionOffset.longValue);
            var offsetVar = sp_EncryptionOffset.longValue;
            if (offsetVar < 0)
                sp_EncryptionOffset.longValue = 0;
        }
        void DrawAESEncryption()
        {
            EditorGUILayout.Space(8);
            sp_BuildInfoAESEncryptionKey.stringValue = EditorGUILayout.TextField("QuarkAesKey", sp_BuildInfoAESEncryptionKey.stringValue);
            var keyStr = sp_BuildInfoAESEncryptionKey.stringValue;
            var keyLength = System.Text.Encoding.UTF8.GetBytes(keyStr).Length;
            EditorGUILayout.LabelField($"Current key length is:{keyLength }");
            if (keyLength != 16 && keyLength != 24 && keyLength != 32 && keyLength != 0)
            {
                EditorGUILayout.HelpBox("Key should be 16,24 or 32 bytes long", MessageType.Error);
            }
        }
    }
}