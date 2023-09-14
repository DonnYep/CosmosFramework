using System.Text;
using UnityEditor;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class AssetBundlePresetLabel
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
        AssetBundleBuildPreset buildSettings;

        AssetBundlePresetLabelData labelData;
        public const string LabelDataName = "ResourceBuilderWindow_AsseBundleTabSettingsLabelData.json";

        Texture2D createAddNewIcon;
        Texture2D saveActiveIcon;

        public void OnEnable(AssetBundleTab parent, string[] buildHandlers)
        {
            this.parent = parent;
            this.buildHandlers = buildHandlers;
            GetTabData();
            createAddNewIcon = ResourceEditorUtility.GetCreateAddNewIcon();
            saveActiveIcon = ResourceEditorUtility.GetSaveActiveIcon();
        }
        public void OnGUI(Rect rect)
        {
            GUILayout.Space(16);
            EditorGUILayout.BeginHorizontal();
            {
                buildSettings = (AssetBundleBuildPreset)EditorGUILayout.ObjectField("Build preset", buildSettings, typeof(AssetBundleBuildPreset), false);
                if (GUILayout.Button(createAddNewIcon, GUILayout.MaxWidth(ResourceBuilderWindowConstant.TEXTURE_ICON_WIDTH)))
                {
                    buildSettings = EditorUtil.CreateScriptableObject<AssetBundleBuildPreset>("Assets/Editor/NewAssetBundleBuildPreset.asset", HideFlags.NotEditable);
                }
                if (GUILayout.Button(saveActiveIcon, GUILayout.MaxWidth(ResourceBuilderWindowConstant.TEXTURE_ICON_WIDTH)))
                {
                    EditorUtil.SaveScriptableObject(buildSettings);
                }
            }
            EditorGUILayout.EndHorizontal();
            if (buildSettings == null)
            {
                return;
            }
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
            if (buildSettings == null)
                return new ResourceBuildParams();
            var buildAssetBundleOptions = ResourceEditorUtility.Builder.GetBuildAssetBundleOptions(buildSettings.AssetBundleSettingsData.AssetBundleCompressType, buildSettings.AssetBundleSettingsData.DisableWriteTypeTree,
                buildSettings.AssetBundleSettingsData.DeterministicAssetBundle, buildSettings.AssetBundleSettingsData.ForceRebuildAssetBundle, buildSettings.AssetBundleSettingsData.IgnoreTypeTreeChanges);
            var buildParams = new ResourceBuildParams()
            {
                AssetBundleBuildPath = buildSettings.AssetBundleSettingsData.AssetBundleBuildPath,
                AssetBundleEncryption = buildSettings.AssetBundleSettingsData.AssetBundleEncryption,
                AssetBundleOffsetValue = buildSettings.AssetBundleSettingsData.AssetBundleOffsetValue,
                BuildAssetBundleOptions = buildAssetBundleOptions,
                AssetBundleNameType = buildSettings.AssetBundleSettingsData.AssetBundleNameType,
                EncryptManifest = buildSettings.AssetBundleSettingsData.EncryptManifest,
                ManifestEncryptionKey = buildSettings.AssetBundleSettingsData.ManifestEncryptionKey,
                BuildTarget = buildSettings.AssetBundleSettingsData.BuildTarget,
                ResourceBuildType = buildSettings.AssetBundleSettingsData.ResourceBuildType,
                BuildVersion = buildSettings.AssetBundleSettingsData.BuildVersion,
                InternalBuildVersion = buildSettings.AssetBundleSettingsData.InternalBuildVersion,
                CopyToStreamingAssets = buildSettings.AssetBundleSettingsData.CopyToStreamingAssets,
                UseStreamingAssetsRelativePath = buildSettings.AssetBundleSettingsData.UseStreamingAssetsRelativePath,
                StreamingAssetsRelativePath = buildSettings.AssetBundleSettingsData.StreamingAssetsRelativePath,
                AssetBundleBuildDirectory = buildSettings.AssetBundleSettingsData.AssetBundleBuildDirectory,
                ClearStreamingAssetsDestinationPath = buildSettings.AssetBundleSettingsData.ClearStreamingAssetsDestinationPath,
                ForceRemoveAllAssetBundleNames = buildSettings.AssetBundleSettingsData.ForceRemoveAllAssetBundleNames,
                BuildHandlerName = buildSettings.AssetBundleSettingsData.BuildHandlerName
            };
            return buildParams;
        }
        void GetTabData()
        {
            try
            {
                labelData = EditorUtil.GetData<AssetBundlePresetLabelData>(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName);
            }
            catch
            {
                labelData = new AssetBundlePresetLabelData();
                EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName, labelData);
            }
            if (!string.IsNullOrEmpty(labelData.SettingsPresetPath))
            {
                buildSettings = AssetDatabase.LoadAssetAtPath<AssetBundleBuildPreset>(labelData.SettingsPresetPath);
                if (buildSettings != null)
                {
                    var buildHandlerMaxIndex = buildHandlers.Length - 1;
                    if (buildSettings.AssetBundleSettingsData.BuildHandlerIndex > buildHandlerMaxIndex)
                    {
                        buildSettings.AssetBundleSettingsData.BuildHandlerIndex = buildHandlerMaxIndex;
                    }
                }
            }
        }
        void SaveTabData()
        {
            labelData.SettingsPresetPath = AssetDatabase.GetAssetPath(buildSettings);
            EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName, labelData);
            EditorUtil.SaveScriptableObject(buildSettings);
        }
        void DrawPrebuildOptions()
        {
            EditorGUILayout.LabelField("Prebuild Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildSettings.AssetBundleSettingsData.ForceRemoveAllAssetBundleNames = EditorGUILayout.ToggleLeft("Force remove all assetBundle names", buildSettings.AssetBundleSettingsData.ForceRemoveAllAssetBundleNames);
                if (buildSettings.AssetBundleSettingsData.ForceRemoveAllAssetBundleNames)
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
                buildSettings.AssetBundleSettingsData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build target", buildSettings.AssetBundleSettingsData.BuildTarget);
                buildSettings.AssetBundleSettingsData.AssetBundleCompressType = (AssetBundleCompressType)EditorGUILayout.EnumPopup("Build compression type", buildSettings.AssetBundleSettingsData.AssetBundleCompressType);

                buildSettings.AssetBundleSettingsData.BuildHandlerIndex = EditorGUILayout.Popup("Build handler", buildSettings.AssetBundleSettingsData.BuildHandlerIndex, buildHandlers);
                if (buildSettings.AssetBundleSettingsData.BuildHandlerIndex < buildHandlers.Length)
                {
                    buildSettings.AssetBundleSettingsData.BuildHandlerName = buildHandlers[buildSettings.AssetBundleSettingsData.BuildHandlerIndex];
                }

                buildSettings.AssetBundleSettingsData.ResourceBuildType = (ResourceBuildType)EditorGUILayout.EnumPopup("Build type", buildSettings.AssetBundleSettingsData.ResourceBuildType);

                buildSettings.AssetBundleSettingsData.ForceRebuildAssetBundle = EditorGUILayout.ToggleLeft("Force rebuild assetBundle", buildSettings.AssetBundleSettingsData.ForceRebuildAssetBundle);
                buildSettings.AssetBundleSettingsData.DisableWriteTypeTree = EditorGUILayout.ToggleLeft("Disable write type tree", buildSettings.AssetBundleSettingsData.DisableWriteTypeTree);
                if (buildSettings.AssetBundleSettingsData.DisableWriteTypeTree)
                    buildSettings.AssetBundleSettingsData.IgnoreTypeTreeChanges = false;

                buildSettings.AssetBundleSettingsData.DeterministicAssetBundle = EditorGUILayout.ToggleLeft("Deterministic assetBundle", buildSettings.AssetBundleSettingsData.DeterministicAssetBundle);
                buildSettings.AssetBundleSettingsData.IgnoreTypeTreeChanges = EditorGUILayout.ToggleLeft("Ignore type tree changes", buildSettings.AssetBundleSettingsData.IgnoreTypeTreeChanges);
                if (buildSettings.AssetBundleSettingsData.IgnoreTypeTreeChanges)
                    buildSettings.AssetBundleSettingsData.DisableWriteTypeTree = false;

                //打包输出的资源加密，如buildInfo，assetbundle 文件名加密
                buildSettings.AssetBundleSettingsData.AssetBundleNameType = (AssetBundleNameType)EditorGUILayout.EnumPopup("Build bundle name type ", buildSettings.AssetBundleSettingsData.AssetBundleNameType);

            }
            EditorGUILayout.EndVertical();
        }
        void DrawPathOptions()
        {
            EditorGUILayout.LabelField("Path Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildSettings.AssetBundleSettingsData.BuildVersion = EditorGUILayout.TextField("Build version", buildSettings.AssetBundleSettingsData.BuildVersion);

                if (buildSettings.AssetBundleSettingsData.ResourceBuildType == ResourceBuildType.Full)
                {
                    buildSettings.AssetBundleSettingsData.InternalBuildVersion = EditorGUILayout.IntField("Internal build version", buildSettings.AssetBundleSettingsData.InternalBuildVersion);
                    if (buildSettings.AssetBundleSettingsData.InternalBuildVersion < 0)
                        buildSettings.AssetBundleSettingsData.InternalBuildVersion = 0;
                }

                EditorGUILayout.BeginHorizontal();
                {
                    buildSettings.AssetBundleSettingsData.BuildPath = EditorGUILayout.TextField("Build path", buildSettings.AssetBundleSettingsData.BuildPath.Trim());
                    if (GUILayout.Button("Browse", GUILayout.MaxWidth(128)))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("Bundle output path", buildSettings.AssetBundleSettingsData.BuildPath, string.Empty);
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            buildSettings.AssetBundleSettingsData.BuildPath = newPath.Replace("\\", "/");
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                buildSettings.AssetBundleSettingsData.AssetBundleBuildDirectory = Utility.IO.RegularPathCombine(buildSettings.AssetBundleSettingsData.BuildPath, buildSettings.AssetBundleSettingsData.BuildVersion, buildSettings.AssetBundleSettingsData.BuildTarget.ToString());
                if (!string.IsNullOrEmpty(buildSettings.AssetBundleSettingsData.BuildVersion))
                {
                    var assetBundleBuildPath = Utility.IO.RegularPathCombine(buildSettings.AssetBundleSettingsData.BuildPath, buildSettings.AssetBundleSettingsData.BuildVersion, buildSettings.AssetBundleSettingsData.BuildTarget.ToString(), $"{buildSettings.AssetBundleSettingsData.BuildVersion}");
                    if (buildSettings.AssetBundleSettingsData.ResourceBuildType == ResourceBuildType.Full)
                    {
                        assetBundleBuildPath += $"_{buildSettings.AssetBundleSettingsData.InternalBuildVersion}";
                    }
                    buildSettings.AssetBundleSettingsData.AssetBundleBuildPath = assetBundleBuildPath;
                }
                else
                    EditorGUILayout.HelpBox("BuildVersion is invalid !", MessageType.Error);
                EditorGUILayout.LabelField("Bundle build path", buildSettings.AssetBundleSettingsData.AssetBundleBuildPath);
            }
            EditorGUILayout.EndVertical();
        }
        void DrawEncryption()
        {
            EditorGUILayout.LabelField("Encryption Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildSettings.AssetBundleSettingsData.EncryptManifest = EditorGUILayout.ToggleLeft("Build info encryption", buildSettings.AssetBundleSettingsData.EncryptManifest);
                if (buildSettings.AssetBundleSettingsData.EncryptManifest)
                {
                    buildSettings.AssetBundleSettingsData.ManifestEncryptionKey = EditorGUILayout.TextField("Build info encryption key", buildSettings.AssetBundleSettingsData.ManifestEncryptionKey);
                    var aesKeyStr = buildSettings.AssetBundleSettingsData.ManifestEncryptionKey;
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
                buildSettings.AssetBundleSettingsData.AssetBundleEncryption = EditorGUILayout.ToggleLeft("AssetBundle encryption", buildSettings.AssetBundleSettingsData.AssetBundleEncryption);
                if (buildSettings.AssetBundleSettingsData.AssetBundleEncryption)
                {
                    EditorGUILayout.LabelField("AssetBundle encryption offset");
                    buildSettings.AssetBundleSettingsData.AssetBundleOffsetValue = EditorGUILayout.IntField("Encryption offset", buildSettings.AssetBundleSettingsData.AssetBundleOffsetValue);
                    if (buildSettings.AssetBundleSettingsData.AssetBundleOffsetValue < 0)
                        buildSettings.AssetBundleSettingsData.AssetBundleOffsetValue = 0;
                }
            }
            EditorGUILayout.EndVertical();
        }
        void DrawBuildDoneOption()
        {
            EditorGUILayout.LabelField("Build Done Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildSettings.AssetBundleSettingsData.CopyToStreamingAssets = EditorGUILayout.ToggleLeft("Copy to streaming assets", buildSettings.AssetBundleSettingsData.CopyToStreamingAssets);
                if (buildSettings.AssetBundleSettingsData.CopyToStreamingAssets)
                {
                    buildSettings.AssetBundleSettingsData.ClearStreamingAssetsDestinationPath = EditorGUILayout.ToggleLeft("Clear streaming assets destination path", buildSettings.AssetBundleSettingsData.ClearStreamingAssetsDestinationPath);
                    buildSettings.AssetBundleSettingsData.UseStreamingAssetsRelativePath = EditorGUILayout.ToggleLeft("Use streaming assets relative path", buildSettings.AssetBundleSettingsData.UseStreamingAssetsRelativePath);
                    string destinationPath = string.Empty;
                    if (buildSettings.AssetBundleSettingsData.UseStreamingAssetsRelativePath)
                    {
                        EditorGUILayout.LabelField("StreamingAssets  relative path");
                        buildSettings.AssetBundleSettingsData.StreamingAssetsRelativePath = EditorGUILayout.TextField("Relative path", buildSettings.AssetBundleSettingsData.StreamingAssetsRelativePath);
                        destinationPath = Utility.IO.RegularPathCombine(Application.streamingAssetsPath, buildSettings.AssetBundleSettingsData.StreamingAssetsRelativePath);
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
