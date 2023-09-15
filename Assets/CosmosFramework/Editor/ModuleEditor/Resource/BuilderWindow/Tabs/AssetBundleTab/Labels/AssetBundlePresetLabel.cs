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
        AssetBundleBuildPreset buildPreset;

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
                buildPreset = (AssetBundleBuildPreset)EditorGUILayout.ObjectField("Build preset", buildPreset, typeof(AssetBundleBuildPreset), false);
                if (GUILayout.Button(createAddNewIcon, GUILayout.MaxWidth(ResourceBuilderWindowConstant.TEXTURE_ICON_WIDTH)))
                {
                    var previousePreset = AssetDatabase.LoadAssetAtPath<AssetBundleBuildPreset>(ResourceBuilderWindowConstant.RESOURCE_NEW_BUILD_PRESET_PATH);
                    if (previousePreset != null)
                    {
                        var canCreate = UnityEditor.EditorUtility.DisplayDialog("AssetBundleBuildPreset already exist", $"Path {ResourceBuilderWindowConstant.RESOURCE_NEW_BUILD_PRESET_PATH} exists.Whether to continue to create and overwrite this file ?", "Create", "Cancel");
                        if (canCreate)
                        {
                            buildPreset = EditorUtil.CreateScriptableObject<AssetBundleBuildPreset>(ResourceBuilderWindowConstant.RESOURCE_NEW_BUILD_PRESET_PATH, HideFlags.NotEditable);
                        }
                    }
                    else
                    {
                        buildPreset = EditorUtil.CreateScriptableObject<AssetBundleBuildPreset>(ResourceBuilderWindowConstant.RESOURCE_NEW_BUILD_PRESET_PATH, HideFlags.NotEditable);
                    }
                }
                if (GUILayout.Button(saveActiveIcon, GUILayout.MaxWidth(ResourceBuilderWindowConstant.TEXTURE_ICON_WIDTH)))
                {
                    EditorUtil.SaveScriptableObject(buildPreset);
                }
            }
            EditorGUILayout.EndHorizontal();
            if (buildPreset == null)
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
            if (buildPreset == null)
                return new ResourceBuildParams();
            var buildAssetBundleOptions = ResourceEditorUtility.Builder.GetBuildAssetBundleOptions(buildPreset.AssetBundleSettingsData.AssetBundleCompressType, buildPreset.AssetBundleSettingsData.DisableWriteTypeTree,
                buildPreset.AssetBundleSettingsData.DeterministicAssetBundle, buildPreset.AssetBundleSettingsData.ForceRebuildAssetBundle, buildPreset.AssetBundleSettingsData.IgnoreTypeTreeChanges);
            var buildParams = new ResourceBuildParams()
            {
                AssetBundleBuildPath = buildPreset.AssetBundleSettingsData.AssetBundleBuildPath,
                AssetBundleEncryption = buildPreset.AssetBundleSettingsData.AssetBundleEncryption,
                AssetBundleOffsetValue = buildPreset.AssetBundleSettingsData.AssetBundleOffsetValue,
                BuildAssetBundleOptions = buildAssetBundleOptions,
                AssetBundleNameType = buildPreset.AssetBundleSettingsData.AssetBundleNameType,
                EncryptManifest = buildPreset.AssetBundleSettingsData.EncryptManifest,
                ManifestEncryptionKey = buildPreset.AssetBundleSettingsData.ManifestEncryptionKey,
                BuildTarget = buildPreset.AssetBundleSettingsData.BuildTarget,
                ResourceBuildType = buildPreset.AssetBundleSettingsData.ResourceBuildType,
                BuildVersion = buildPreset.AssetBundleSettingsData.BuildVersion,
                InternalBuildVersion = buildPreset.AssetBundleSettingsData.InternalBuildVersion,
                CopyToStreamingAssets = buildPreset.AssetBundleSettingsData.CopyToStreamingAssets,
                UseStreamingAssetsRelativePath = buildPreset.AssetBundleSettingsData.UseStreamingAssetsRelativePath,
                StreamingAssetsRelativePath = buildPreset.AssetBundleSettingsData.StreamingAssetsRelativePath,
                AssetBundleBuildDirectory = buildPreset.AssetBundleSettingsData.AssetBundleBuildDirectory,
                ClearStreamingAssetsDestinationPath = buildPreset.AssetBundleSettingsData.ClearStreamingAssetsDestinationPath,
                ForceRemoveAllAssetBundleNames = buildPreset.AssetBundleSettingsData.ForceRemoveAllAssetBundleNames,
                BuildHandlerName = buildPreset.AssetBundleSettingsData.BuildHandlerName
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
            if (!string.IsNullOrEmpty(labelData.PresetPath))
            {
                buildPreset = AssetDatabase.LoadAssetAtPath<AssetBundleBuildPreset>(labelData.PresetPath);
                if (buildPreset != null)
                {
                    var buildHandlerMaxIndex = buildHandlers.Length - 1;
                    if (buildPreset.AssetBundleSettingsData.BuildHandlerIndex > buildHandlerMaxIndex)
                    {
                        buildPreset.AssetBundleSettingsData.BuildHandlerIndex = buildHandlerMaxIndex;
                    }
                }
            }
        }
        void SaveTabData()
        {
            labelData.PresetPath = AssetDatabase.GetAssetPath(buildPreset);
            EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName, labelData);
            EditorUtil.SaveScriptableObject(buildPreset);
        }
        void DrawPrebuildOptions()
        {
            EditorGUILayout.LabelField("Prebuild Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildPreset.AssetBundleSettingsData.ForceRemoveAllAssetBundleNames = EditorGUILayout.ToggleLeft("Force remove all assetBundle names", buildPreset.AssetBundleSettingsData.ForceRemoveAllAssetBundleNames);
                if (buildPreset.AssetBundleSettingsData.ForceRemoveAllAssetBundleNames)
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
                buildPreset.AssetBundleSettingsData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build target", buildPreset.AssetBundleSettingsData.BuildTarget);
                buildPreset.AssetBundleSettingsData.AssetBundleCompressType = (AssetBundleCompressType)EditorGUILayout.EnumPopup("Build compression type", buildPreset.AssetBundleSettingsData.AssetBundleCompressType);

                buildPreset.AssetBundleSettingsData.BuildHandlerIndex = EditorGUILayout.Popup("Build handler", buildPreset.AssetBundleSettingsData.BuildHandlerIndex, buildHandlers);
                if (buildPreset.AssetBundleSettingsData.BuildHandlerIndex < buildHandlers.Length)
                {
                    buildPreset.AssetBundleSettingsData.BuildHandlerName = buildHandlers[buildPreset.AssetBundleSettingsData.BuildHandlerIndex];
                }

                buildPreset.AssetBundleSettingsData.ResourceBuildType = (ResourceBuildType)EditorGUILayout.EnumPopup("Build type", buildPreset.AssetBundleSettingsData.ResourceBuildType);

                buildPreset.AssetBundleSettingsData.ForceRebuildAssetBundle = EditorGUILayout.ToggleLeft("Force rebuild assetBundle", buildPreset.AssetBundleSettingsData.ForceRebuildAssetBundle);
                buildPreset.AssetBundleSettingsData.DisableWriteTypeTree = EditorGUILayout.ToggleLeft("Disable write type tree", buildPreset.AssetBundleSettingsData.DisableWriteTypeTree);
                if (buildPreset.AssetBundleSettingsData.DisableWriteTypeTree)
                    buildPreset.AssetBundleSettingsData.IgnoreTypeTreeChanges = false;

                buildPreset.AssetBundleSettingsData.DeterministicAssetBundle = EditorGUILayout.ToggleLeft("Deterministic assetBundle", buildPreset.AssetBundleSettingsData.DeterministicAssetBundle);
                buildPreset.AssetBundleSettingsData.IgnoreTypeTreeChanges = EditorGUILayout.ToggleLeft("Ignore type tree changes", buildPreset.AssetBundleSettingsData.IgnoreTypeTreeChanges);
                if (buildPreset.AssetBundleSettingsData.IgnoreTypeTreeChanges)
                    buildPreset.AssetBundleSettingsData.DisableWriteTypeTree = false;

                //打包输出的资源加密，如buildInfo，assetbundle 文件名加密
                buildPreset.AssetBundleSettingsData.AssetBundleNameType = (AssetBundleNameType)EditorGUILayout.EnumPopup("Build bundle name type ", buildPreset.AssetBundleSettingsData.AssetBundleNameType);

            }
            EditorGUILayout.EndVertical();
        }
        void DrawPathOptions()
        {
            EditorGUILayout.LabelField("Path Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildPreset.AssetBundleSettingsData.BuildVersion = EditorGUILayout.TextField("Build version", buildPreset.AssetBundleSettingsData.BuildVersion);

                if (buildPreset.AssetBundleSettingsData.ResourceBuildType == ResourceBuildType.Full)
                {
                    buildPreset.AssetBundleSettingsData.InternalBuildVersion = EditorGUILayout.IntField("Internal build version", buildPreset.AssetBundleSettingsData.InternalBuildVersion);
                    if (buildPreset.AssetBundleSettingsData.InternalBuildVersion < 0)
                        buildPreset.AssetBundleSettingsData.InternalBuildVersion = 0;
                }

                EditorGUILayout.BeginHorizontal();
                {
                    buildPreset.AssetBundleSettingsData.BuildPath = EditorGUILayout.TextField("Build path", buildPreset.AssetBundleSettingsData.BuildPath.Trim());
                    if (GUILayout.Button("Browse", GUILayout.MaxWidth(128)))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("Bundle output path", buildPreset.AssetBundleSettingsData.BuildPath, string.Empty);
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            buildPreset.AssetBundleSettingsData.BuildPath = newPath.Replace("\\", "/");
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                buildPreset.AssetBundleSettingsData.AssetBundleBuildDirectory = Utility.IO.RegularPathCombine(buildPreset.AssetBundleSettingsData.BuildPath, buildPreset.AssetBundleSettingsData.BuildVersion, buildPreset.AssetBundleSettingsData.BuildTarget.ToString());
                if (!string.IsNullOrEmpty(buildPreset.AssetBundleSettingsData.BuildVersion))
                {
                    var assetBundleBuildPath = Utility.IO.RegularPathCombine(buildPreset.AssetBundleSettingsData.BuildPath, buildPreset.AssetBundleSettingsData.BuildVersion, buildPreset.AssetBundleSettingsData.BuildTarget.ToString(), $"{buildPreset.AssetBundleSettingsData.BuildVersion}");
                    if (buildPreset.AssetBundleSettingsData.ResourceBuildType == ResourceBuildType.Full)
                    {
                        assetBundleBuildPath += $"_{buildPreset.AssetBundleSettingsData.InternalBuildVersion}";
                    }
                    buildPreset.AssetBundleSettingsData.AssetBundleBuildPath = assetBundleBuildPath;
                }
                else
                    EditorGUILayout.HelpBox("BuildVersion is invalid !", MessageType.Error);
                EditorGUILayout.LabelField("Bundle build path", buildPreset.AssetBundleSettingsData.AssetBundleBuildPath);
            }
            EditorGUILayout.EndVertical();
        }
        void DrawEncryption()
        {
            EditorGUILayout.LabelField("Encryption Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildPreset.AssetBundleSettingsData.EncryptManifest = EditorGUILayout.ToggleLeft("Build info encryption", buildPreset.AssetBundleSettingsData.EncryptManifest);
                if (buildPreset.AssetBundleSettingsData.EncryptManifest)
                {
                    buildPreset.AssetBundleSettingsData.ManifestEncryptionKey = EditorGUILayout.TextField("Build info encryption key", buildPreset.AssetBundleSettingsData.ManifestEncryptionKey);
                    var aesKeyStr = buildPreset.AssetBundleSettingsData.ManifestEncryptionKey;
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
                buildPreset.AssetBundleSettingsData.AssetBundleEncryption = EditorGUILayout.ToggleLeft("AssetBundle encryption", buildPreset.AssetBundleSettingsData.AssetBundleEncryption);
                if (buildPreset.AssetBundleSettingsData.AssetBundleEncryption)
                {
                    EditorGUILayout.LabelField("AssetBundle encryption offset");
                    buildPreset.AssetBundleSettingsData.AssetBundleOffsetValue = EditorGUILayout.IntField("Encryption offset", buildPreset.AssetBundleSettingsData.AssetBundleOffsetValue);
                    if (buildPreset.AssetBundleSettingsData.AssetBundleOffsetValue < 0)
                        buildPreset.AssetBundleSettingsData.AssetBundleOffsetValue = 0;
                }
            }
            EditorGUILayout.EndVertical();
        }
        void DrawBuildDoneOption()
        {
            EditorGUILayout.LabelField("Build Done Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildPreset.AssetBundleSettingsData.CopyToStreamingAssets = EditorGUILayout.ToggleLeft("Copy to streaming assets", buildPreset.AssetBundleSettingsData.CopyToStreamingAssets);
                if (buildPreset.AssetBundleSettingsData.CopyToStreamingAssets)
                {
                    buildPreset.AssetBundleSettingsData.ClearStreamingAssetsDestinationPath = EditorGUILayout.ToggleLeft("Clear streaming assets destination path", buildPreset.AssetBundleSettingsData.ClearStreamingAssetsDestinationPath);
                    buildPreset.AssetBundleSettingsData.UseStreamingAssetsRelativePath = EditorGUILayout.ToggleLeft("Use streaming assets relative path", buildPreset.AssetBundleSettingsData.UseStreamingAssetsRelativePath);
                    string destinationPath = string.Empty;
                    if (buildPreset.AssetBundleSettingsData.UseStreamingAssetsRelativePath)
                    {
                        EditorGUILayout.LabelField("StreamingAssets  relative path");
                        buildPreset.AssetBundleSettingsData.StreamingAssetsRelativePath = EditorGUILayout.TextField("Relative path", buildPreset.AssetBundleSettingsData.StreamingAssetsRelativePath);
                        destinationPath = Utility.IO.RegularPathCombine(Application.streamingAssetsPath, buildPreset.AssetBundleSettingsData.StreamingAssetsRelativePath);
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
