using System.Text;
using UnityEditor;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class AssetBundleNoPresetLabel
    {
        AssetBundleTab parent;
        bool isAesKeyInvalid = false;
        public bool IsAesKeyInvalid
        {
            get
            {
                return isAesKeyInvalid;
            }
        }

        string[] buildHandlers;
        public const string LabelDataName = "ResourceBuilderWindow_AsseBundleTabNoSettingsLabelData.json";
        AssetBundleBuildPresetData presetData;

        public void OnEnable(AssetBundleTab parent, string[] buildHandlers)
        {
            this.parent = parent;
            this.buildHandlers = buildHandlers;
            GetTabData();
        }
        public void OnGUI(Rect rect)
        {
            GUILayout.Space(16);
            DrawPrebuildOptions();
            GUILayout.Space(16);
            DrawBuildOptions();
            GUILayout.Space(16);
            DrawPathOptions();
            GUILayout.Space(16);
            DrawEncryption();
            GUILayout.Space(16);
            DrawBuildDoneOption();
        }
        public void OnDisable()
        {
            SaveTabData();
        }
        public void Clear()
        {

        }
        public ResourceBuildParams GetBuildParams()
        {
            var buildAssetBundleOptions = ResourceEditorUtility.Builder.GetBuildAssetBundleOptions(presetData.AssetBundleCompressType, presetData.DisableWriteTypeTree,
                presetData.DeterministicAssetBundle, presetData.ForceRebuildAssetBundle, presetData.IgnoreTypeTreeChanges);
            var buildParams = new ResourceBuildParams()
            {
                AssetBundleBuildPath = presetData.AssetBundleBuildPath,
                AssetBundleEncryption = presetData.AssetBundleEncryption,
                AssetBundleOffsetValue = presetData.AssetBundleOffsetValue,
                BuildAssetBundleOptions = buildAssetBundleOptions,
                AssetBundleNameType = presetData.AssetBundleNameType,
                EncryptManifest = presetData.EncryptManifest,
                ManifestEncryptionKey = presetData.ManifestEncryptionKey,
                BuildTarget = presetData.BuildTarget,
                ResourceBuildType = presetData.ResourceBuildType,
                BuildVersion = presetData.BuildVersion,
                InternalBuildVersion = presetData.InternalBuildVersion,
                CopyToStreamingAssets = presetData.CopyToStreamingAssets,
                UseStreamingAssetsRelativePath = presetData.UseStreamingAssetsRelativePath,
                StreamingAssetsRelativePath = presetData.StreamingAssetsRelativePath,
                AssetBundleBuildDirectory = presetData.AssetBundleBuildDirectory,
                ClearStreamingAssetsDestinationPath = presetData.ClearStreamingAssetsDestinationPath,
                ForceRemoveAllAssetBundleNames = presetData.ForceRemoveAllAssetBundleNames,
                BuildHandlerName = presetData.BuildHandlerName
            };
            return buildParams;
        }
        void GetTabData()
        {
            try
            {
                presetData = EditorUtil.GetData<AssetBundleBuildPresetData>(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName);
                var buildHandlerMaxIndex = buildHandlers.Length - 1;
                if (presetData.BuildHandlerIndex > buildHandlerMaxIndex)
                {
                    presetData.BuildHandlerIndex = buildHandlerMaxIndex;
                }
            }
            catch
            {
                presetData = new AssetBundleBuildPresetData();
                EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName, presetData);
            }
        }
        void SaveTabData()
        {
            EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName, presetData);
        }
        void DrawPrebuildOptions()
        {
            EditorGUILayout.LabelField("Prebuild Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                presetData.ForceRemoveAllAssetBundleNames = EditorGUILayout.ToggleLeft("Force remove all assetBundle names", presetData.ForceRemoveAllAssetBundleNames);
                if (presetData.ForceRemoveAllAssetBundleNames)
                {
                    EditorGUILayout.HelpBox("This operation will force remove all assetBundle names in this project", MessageType.Warning);
                }
            }
            EditorGUILayout.EndVertical();
        }
        void DrawBuildOptions()
        {
            EditorGUILayout.LabelField("Build Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                presetData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build target", presetData.BuildTarget);
                presetData.AssetBundleCompressType = (AssetBundleCompressType)EditorGUILayout.EnumPopup("Build compression type", presetData.AssetBundleCompressType);

                presetData.BuildHandlerIndex = EditorGUILayout.Popup("Build handler", presetData.BuildHandlerIndex, buildHandlers);
                if (presetData.BuildHandlerIndex < buildHandlers.Length)
                {
                    presetData.BuildHandlerName = buildHandlers[presetData.BuildHandlerIndex];
                }

                presetData.ResourceBuildType = (ResourceBuildType)EditorGUILayout.EnumPopup("Build type", presetData.ResourceBuildType);

                presetData.ForceRebuildAssetBundle = EditorGUILayout.ToggleLeft("Force rebuild assetBundle", presetData.ForceRebuildAssetBundle);
                presetData.DisableWriteTypeTree = EditorGUILayout.ToggleLeft("Disable write type tree", presetData.DisableWriteTypeTree);
                if (presetData.DisableWriteTypeTree)
                    presetData.IgnoreTypeTreeChanges = false;

                presetData.DeterministicAssetBundle = EditorGUILayout.ToggleLeft("Deterministic assetBundle", presetData.DeterministicAssetBundle);
                presetData.IgnoreTypeTreeChanges = EditorGUILayout.ToggleLeft("Ignore type tree changes", presetData.IgnoreTypeTreeChanges);
                if (presetData.IgnoreTypeTreeChanges)
                    presetData.DisableWriteTypeTree = false;

                //打包输出的资源加密，如buildInfo，assetbundle 文件名加密
                presetData.AssetBundleNameType = (AssetBundleNameType)EditorGUILayout.EnumPopup("Build bundle name type ", presetData.AssetBundleNameType);

            }
            EditorGUILayout.EndVertical();
        }
        void DrawPathOptions()
        {
            EditorGUILayout.LabelField("Path Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                presetData.BuildVersion = EditorGUILayout.TextField("Build version", presetData.BuildVersion);

                if (presetData.ResourceBuildType == ResourceBuildType.Full)
                {
                    presetData.InternalBuildVersion = EditorGUILayout.IntField("Internal build version", presetData.InternalBuildVersion);
                    if (presetData.InternalBuildVersion < 0)
                        presetData.InternalBuildVersion = 0;
                }

                EditorGUILayout.BeginHorizontal();
                {
                    presetData.BuildPath = EditorGUILayout.TextField("Build path", presetData.BuildPath.Trim());
                    if (GUILayout.Button("Browse", GUILayout.MaxWidth(128)))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("Bundle output path", presetData.BuildPath, string.Empty);
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            presetData.BuildPath = newPath.Replace("\\", "/");
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                presetData.AssetBundleBuildDirectory = Utility.IO.RegularPathCombine(presetData.BuildPath, presetData.BuildVersion, presetData.BuildTarget.ToString());
                if (!string.IsNullOrEmpty(presetData.BuildVersion))
                {
                    var assetBundleBuildPath = Utility.IO.RegularPathCombine(presetData.BuildPath, presetData.BuildVersion, presetData.BuildTarget.ToString(), $"{presetData.BuildVersion}");
                    if (presetData.ResourceBuildType == ResourceBuildType.Full)
                    {
                        assetBundleBuildPath += $"_{presetData.InternalBuildVersion}";
                    }
                    presetData.AssetBundleBuildPath = assetBundleBuildPath;
                }
                else
                    EditorGUILayout.HelpBox("BuildVersion is invalid !", MessageType.Error);
                EditorGUILayout.LabelField("Bundle build path", presetData.AssetBundleBuildPath);
            }
            EditorGUILayout.EndVertical();
        }
        void DrawEncryption()
        {
            EditorGUILayout.LabelField("Encryption Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                presetData.EncryptManifest = EditorGUILayout.ToggleLeft("Build info encryption", presetData.EncryptManifest);
                if (presetData.EncryptManifest)
                {
                    presetData.ManifestEncryptionKey = EditorGUILayout.TextField("Build info encryption key", presetData.ManifestEncryptionKey);
                    var aesKeyStr = presetData.ManifestEncryptionKey;
                    var aesKeyLength = Encoding.UTF8.GetBytes(aesKeyStr).Length;
                    EditorGUILayout.LabelField($"Assets AES encryption key, key should be 16,24 or 32 bytes long, current key length is : {aesKeyLength} ");
                    if (aesKeyLength != 16 && aesKeyLength != 24 && aesKeyLength != 32 && aesKeyLength != 0)
                    {
                        EditorGUILayout.HelpBox("Encryption key should be 16,24 or 32 bytes long", MessageType.Error);
                        isAesKeyInvalid = false;
                    }
                    else
                    {
                        isAesKeyInvalid = true;
                    }

                    GUILayout.Space(16);
                }
                else
                    isAesKeyInvalid = true;
                presetData.AssetBundleEncryption = EditorGUILayout.ToggleLeft("AssetBundle encryption", presetData.AssetBundleEncryption);
                if (presetData.AssetBundleEncryption)
                {
                    EditorGUILayout.LabelField("AssetBundle encryption offset");
                    presetData.AssetBundleOffsetValue = EditorGUILayout.IntField("Encryption offset", presetData.AssetBundleOffsetValue);
                    if (presetData.AssetBundleOffsetValue < 0)
                        presetData.AssetBundleOffsetValue = 0;
                }
            }
            EditorGUILayout.EndVertical();
        }
        void DrawBuildDoneOption()
        {
            EditorGUILayout.LabelField("Build Done Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                presetData.CopyToStreamingAssets = EditorGUILayout.ToggleLeft("Copy to streaming assets", presetData.CopyToStreamingAssets);
                if (presetData.CopyToStreamingAssets)
                {
                    presetData.ClearStreamingAssetsDestinationPath = EditorGUILayout.ToggleLeft("Clear streaming assets destination path", presetData.ClearStreamingAssetsDestinationPath);
                    presetData.UseStreamingAssetsRelativePath = EditorGUILayout.ToggleLeft("Use streaming assets relative path", presetData.UseStreamingAssetsRelativePath);
                    string destinationPath = string.Empty;
                    if (presetData.UseStreamingAssetsRelativePath)
                    {
                        EditorGUILayout.LabelField("StreamingAssets  relative path");
                        presetData.StreamingAssetsRelativePath = EditorGUILayout.TextField("Relative path", presetData.StreamingAssetsRelativePath);
                        destinationPath = Utility.IO.RegularPathCombine(Application.streamingAssetsPath, presetData.StreamingAssetsRelativePath);
                    }
                    else
                    {
                        destinationPath = Application.streamingAssetsPath;
                    }

                    EditorGUILayout.LabelField("Destination path", destinationPath);
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
