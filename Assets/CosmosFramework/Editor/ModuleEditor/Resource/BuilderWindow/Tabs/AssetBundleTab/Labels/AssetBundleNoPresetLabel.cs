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
        AssetBundleBuildPresetData settingsData;

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
            var buildAssetBundleOptions = ResourceEditorUtility.Builder.GetBuildAssetBundleOptions(settingsData.AssetBundleCompressType, settingsData.DisableWriteTypeTree,
                settingsData.DeterministicAssetBundle, settingsData.ForceRebuildAssetBundle, settingsData.IgnoreTypeTreeChanges);
            var buildParams = new ResourceBuildParams()
            {
                AssetBundleBuildPath = settingsData.AssetBundleBuildPath,
                AssetBundleEncryption = settingsData.AssetBundleEncryption,
                AssetBundleOffsetValue = settingsData.AssetBundleOffsetValue,
                BuildAssetBundleOptions = buildAssetBundleOptions,
                AssetBundleNameType = settingsData.AssetBundleNameType,
                EncryptManifest = settingsData.EncryptManifest,
                ManifestEncryptionKey = settingsData.ManifestEncryptionKey,
                BuildTarget = settingsData.BuildTarget,
                ResourceBuildType = settingsData.ResourceBuildType,
                BuildVersion = settingsData.BuildVersion,
                InternalBuildVersion = settingsData.InternalBuildVersion,
                CopyToStreamingAssets = settingsData.CopyToStreamingAssets,
                UseStreamingAssetsRelativePath = settingsData.UseStreamingAssetsRelativePath,
                StreamingAssetsRelativePath = settingsData.StreamingAssetsRelativePath,
                AssetBundleBuildDirectory = settingsData.AssetBundleBuildDirectory,
                ClearStreamingAssetsDestinationPath = settingsData.ClearStreamingAssetsDestinationPath,
                ForceRemoveAllAssetBundleNames = settingsData.ForceRemoveAllAssetBundleNames,
                BuildHandlerName = settingsData.BuildHandlerName
            };
            return buildParams;
        }
        void GetTabData()
        {
            try
            {
                settingsData = EditorUtil.GetData<AssetBundleBuildPresetData>(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName);
                var buildHandlerMaxIndex = buildHandlers.Length - 1;
                if (settingsData.BuildHandlerIndex > buildHandlerMaxIndex)
                {
                    settingsData.BuildHandlerIndex = buildHandlerMaxIndex;
                }
            }
            catch
            {
                settingsData = new AssetBundleBuildPresetData();
                EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName, settingsData);
            }
        }
        void SaveTabData()
        {
            EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName, settingsData);
        }
        void DrawPrebuildOptions()
        {
            EditorGUILayout.LabelField("Prebuild Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                settingsData.ForceRemoveAllAssetBundleNames = EditorGUILayout.ToggleLeft("Force remove all assetBundle names", settingsData.ForceRemoveAllAssetBundleNames);
                if (settingsData.ForceRemoveAllAssetBundleNames)
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
                settingsData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build target", settingsData.BuildTarget);
                settingsData.AssetBundleCompressType = (AssetBundleCompressType)EditorGUILayout.EnumPopup("Build compression type", settingsData.AssetBundleCompressType);

                settingsData.BuildHandlerIndex = EditorGUILayout.Popup("Build handler", settingsData.BuildHandlerIndex, buildHandlers);
                if (settingsData.BuildHandlerIndex < buildHandlers.Length)
                {
                    settingsData.BuildHandlerName = buildHandlers[settingsData.BuildHandlerIndex];
                }

                settingsData.ResourceBuildType = (ResourceBuildType)EditorGUILayout.EnumPopup("Build type", settingsData.ResourceBuildType);

                settingsData.ForceRebuildAssetBundle = EditorGUILayout.ToggleLeft("Force rebuild assetBundle", settingsData.ForceRebuildAssetBundle);
                settingsData.DisableWriteTypeTree = EditorGUILayout.ToggleLeft("Disable write type tree", settingsData.DisableWriteTypeTree);
                if (settingsData.DisableWriteTypeTree)
                    settingsData.IgnoreTypeTreeChanges = false;

                settingsData.DeterministicAssetBundle = EditorGUILayout.ToggleLeft("Deterministic assetBundle", settingsData.DeterministicAssetBundle);
                settingsData.IgnoreTypeTreeChanges = EditorGUILayout.ToggleLeft("Ignore type tree changes", settingsData.IgnoreTypeTreeChanges);
                if (settingsData.IgnoreTypeTreeChanges)
                    settingsData.DisableWriteTypeTree = false;

                //打包输出的资源加密，如buildInfo，assetbundle 文件名加密
                settingsData.AssetBundleNameType = (AssetBundleNameType)EditorGUILayout.EnumPopup("Build bundle name type ", settingsData.AssetBundleNameType);

            }
            EditorGUILayout.EndVertical();
        }
        void DrawPathOptions()
        {
            EditorGUILayout.LabelField("Path Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                settingsData.BuildVersion = EditorGUILayout.TextField("Build version", settingsData.BuildVersion);

                if (settingsData.ResourceBuildType == ResourceBuildType.Full)
                {
                    settingsData.InternalBuildVersion = EditorGUILayout.IntField("Internal build version", settingsData.InternalBuildVersion);
                    if (settingsData.InternalBuildVersion < 0)
                        settingsData.InternalBuildVersion = 0;
                }

                EditorGUILayout.BeginHorizontal();
                {
                    settingsData.BuildPath = EditorGUILayout.TextField("Build path", settingsData.BuildPath.Trim());
                    if (GUILayout.Button("Browse", GUILayout.MaxWidth(128)))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("Bundle output path", settingsData.BuildPath, string.Empty);
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            settingsData.BuildPath = newPath.Replace("\\", "/");
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                settingsData.AssetBundleBuildDirectory = Utility.IO.RegularPathCombine(settingsData.BuildPath, settingsData.BuildVersion, settingsData.BuildTarget.ToString());
                if (!string.IsNullOrEmpty(settingsData.BuildVersion))
                {
                    var assetBundleBuildPath = Utility.IO.RegularPathCombine(settingsData.BuildPath, settingsData.BuildVersion, settingsData.BuildTarget.ToString(), $"{settingsData.BuildVersion}");
                    if (settingsData.ResourceBuildType == ResourceBuildType.Full)
                    {
                        assetBundleBuildPath += $"_{settingsData.InternalBuildVersion}";
                    }
                    settingsData.AssetBundleBuildPath = assetBundleBuildPath;
                }
                else
                    EditorGUILayout.HelpBox("BuildVersion is invalid !", MessageType.Error);
                EditorGUILayout.LabelField("Bundle build path", settingsData.AssetBundleBuildPath);
            }
            EditorGUILayout.EndVertical();
        }
        void DrawEncryption()
        {
            EditorGUILayout.LabelField("Encryption Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                settingsData.EncryptManifest = EditorGUILayout.ToggleLeft("Build info encryption", settingsData.EncryptManifest);
                if (settingsData.EncryptManifest)
                {
                    settingsData.ManifestEncryptionKey = EditorGUILayout.TextField("Build info encryption key", settingsData.ManifestEncryptionKey);
                    var aesKeyStr = settingsData.ManifestEncryptionKey;
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
                settingsData.AssetBundleEncryption = EditorGUILayout.ToggleLeft("AssetBundle encryption", settingsData.AssetBundleEncryption);
                if (settingsData.AssetBundleEncryption)
                {
                    EditorGUILayout.LabelField("AssetBundle encryption offset");
                    settingsData.AssetBundleOffsetValue = EditorGUILayout.IntField("Encryption offset", settingsData.AssetBundleOffsetValue);
                    if (settingsData.AssetBundleOffsetValue < 0)
                        settingsData.AssetBundleOffsetValue = 0;
                }
            }
            EditorGUILayout.EndVertical();
        }
        void DrawBuildDoneOption()
        {
            EditorGUILayout.LabelField("Build Done Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                settingsData.CopyToStreamingAssets = EditorGUILayout.ToggleLeft("Copy to streaming assets", settingsData.CopyToStreamingAssets);
                if (settingsData.CopyToStreamingAssets)
                {
                    settingsData.ClearStreamingAssetsDestinationPath = EditorGUILayout.ToggleLeft("Clear streaming assets destination path", settingsData.ClearStreamingAssetsDestinationPath);
                    settingsData.UseStreamingAssetsRelativePath = EditorGUILayout.ToggleLeft("Use streaming assets relative path", settingsData.UseStreamingAssetsRelativePath);
                    string destinationPath = string.Empty;
                    if (settingsData.UseStreamingAssetsRelativePath)
                    {
                        EditorGUILayout.LabelField("StreamingAssets  relative path");
                        settingsData.StreamingAssetsRelativePath = EditorGUILayout.TextField("Relative path", settingsData.StreamingAssetsRelativePath);
                        destinationPath = Utility.IO.RegularPathCombine(Application.streamingAssetsPath, settingsData.StreamingAssetsRelativePath);
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
