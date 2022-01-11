using UnityEditor;
using Quark;
using Quark.Asset;

namespace CosmosEditor.Quark
{
    [CustomEditor(typeof(QuarkConfiguration), true)]
    public class QuarkConfigurationEditor : UnityEditor.Editor
    {
        SerializedObject targetObject;
        QuarkConfiguration quarkConfig;
        SerializedProperty sp_QuarkAssetLoadMode, sp_QuarkAssetDataset, sp_Url, sp_PingUrl, sp_QuarkLoadPath,
            sp_AESEncryptionKey, sp_EnableRelativeLoadPath, sp_RelativeLoadPath, sp_CustomeLoadPath, sp_QuarkBuildPath,
            sp_EnableRelativeBuildPath, sp_RelativeBuildPath;
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
                        DrawBuildAssetBundleTab();
                    }
                    break;
            }
            DrawAES();
            targetObject.ApplyModifiedProperties();
        }
        private void OnEnable()
        {
            quarkConfig = target as QuarkConfiguration;
            targetObject = new SerializedObject(quarkConfig);
            sp_QuarkAssetLoadMode = targetObject.FindProperty("QuarkAssetLoadMode");
            sp_QuarkAssetDataset = targetObject.FindProperty("QuarkAssetDataset");
            sp_Url = targetObject.FindProperty("Url");
            sp_PingUrl = targetObject.FindProperty("PingUrl");
            sp_QuarkLoadPath = targetObject.FindProperty("QuarkLoadPath");
            sp_AESEncryptionKey = targetObject.FindProperty("AESEncryptionKey");
            sp_EnableRelativeLoadPath = targetObject.FindProperty("EnableRelativeLoadPath");
            sp_CustomeLoadPath = targetObject.FindProperty("CustomeLoadPath");
            sp_RelativeLoadPath = targetObject.FindProperty("RelativeLoadPath");

            sp_QuarkBuildPath = targetObject.FindProperty("QuarkBuildPath");
            sp_EnableRelativeBuildPath = targetObject.FindProperty("EnableRelativeBuildPath");
            sp_RelativeBuildPath = targetObject.FindProperty("RelativeBuildPath");
        }
        void DrawBuildAssetBundleTab()
        {
            EditorGUILayout.Space(16);
            EditorGUILayout.HelpBox("Asset bundle build path",MessageType.Info);

            EditorGUILayout.BeginVertical("box");
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
                        EditorGUILayout.Space(16);
                    }
                    break;
            }
            EditorGUILayout.EndVertical();

            if (buildType == QuarkBuildPath.StreamingAssets)
                return;

            EditorGUILayout.Space(16);

            EditorGUILayout.HelpBox("Asset bundle load path", MessageType.Info);

            EditorGUILayout.BeginVertical("box");
            sp_QuarkLoadPath.enumValueIndex = (byte)(QuarkLoadPath)EditorGUILayout.EnumPopup("QuarkLoadPath", (QuarkLoadPath)sp_QuarkLoadPath.enumValueIndex);
            var pathType = (QuarkLoadPath)sp_QuarkLoadPath.enumValueIndex;
            if (pathType != QuarkLoadPath.Custome)
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
                sp_CustomeLoadPath.stringValue = EditorGUILayout.TextField("CustomeLoadPath", sp_CustomeLoadPath.stringValue.Trim());
            }
            EditorGUILayout.EndVertical();
        }
        void DrawAES()
        {
            EditorGUILayout.Space(8);
            aesKeyFoldout = EditorGUILayout.Foldout(aesKeyFoldout, "AESKey");
            if (aesKeyFoldout)
            {
                EditorGUILayout.LabelField("Quark AES Key : ");
                sp_AESEncryptionKey.stringValue = EditorGUILayout.TextField("AESEncryptionKey", sp_AESEncryptionKey.stringValue.Trim());
            }
        }
    }
}