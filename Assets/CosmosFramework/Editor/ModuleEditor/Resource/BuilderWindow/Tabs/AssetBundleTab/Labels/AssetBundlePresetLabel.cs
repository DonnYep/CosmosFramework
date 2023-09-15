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
            var buildAssetBundleOptions = ResourceEditorUtility.Builder.GetBuildAssetBundleOptions(buildPreset.AssetBundleBuildPresetData.AssetBundleCompressType, buildPreset.AssetBundleBuildPresetData.DisableWriteTypeTree,
                buildPreset.AssetBundleBuildPresetData.DeterministicAssetBundle, buildPreset.AssetBundleBuildPresetData.ForceRebuildAssetBundle, buildPreset.AssetBundleBuildPresetData.IgnoreTypeTreeChanges);
            var buildParams = new ResourceBuildParams()
            {
                AssetBundleBuildPath = buildPreset.AssetBundleBuildPresetData.AssetBundleBuildPath,
                AssetBundleEncryption = buildPreset.AssetBundleBuildPresetData.AssetBundleEncryption,
                AssetBundleOffsetValue = buildPreset.AssetBundleBuildPresetData.AssetBundleOffsetValue,
                BuildAssetBundleOptions = buildAssetBundleOptions,
                AssetBundleNameType = buildPreset.AssetBundleBuildPresetData.AssetBundleNameType,
                EncryptManifest = buildPreset.AssetBundleBuildPresetData.EncryptManifest,
                ManifestEncryptionKey = buildPreset.AssetBundleBuildPresetData.ManifestEncryptionKey,
                BuildTarget = buildPreset.AssetBundleBuildPresetData.BuildTarget,
                ResourceBuildType = buildPreset.AssetBundleBuildPresetData.ResourceBuildType,
                BuildVersion = buildPreset.AssetBundleBuildPresetData.BuildVersion,
                InternalBuildVersion = buildPreset.AssetBundleBuildPresetData.InternalBuildVersion,
                CopyToStreamingAssets = buildPreset.AssetBundleBuildPresetData.CopyToStreamingAssets,
                UseStreamingAssetsRelativePath = buildPreset.AssetBundleBuildPresetData.UseStreamingAssetsRelativePath,
                StreamingAssetsRelativePath = buildPreset.AssetBundleBuildPresetData.StreamingAssetsRelativePath,
                AssetBundleBuildDirectory = buildPreset.AssetBundleBuildPresetData.AssetBundleBuildDirectory,
                ClearStreamingAssetsDestinationPath = buildPreset.AssetBundleBuildPresetData.ClearStreamingAssetsDestinationPath,
                ForceRemoveAllAssetBundleNames = buildPreset.AssetBundleBuildPresetData.ForceRemoveAllAssetBundleNames,
                BuildHandlerName = buildPreset.AssetBundleBuildPresetData.BuildHandlerName
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
                    if (buildPreset.AssetBundleBuildPresetData.BuildHandlerIndex > buildHandlerMaxIndex)
                    {
                        buildPreset.AssetBundleBuildPresetData.BuildHandlerIndex = buildHandlerMaxIndex;
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
                buildPreset.AssetBundleBuildPresetData.ForceRemoveAllAssetBundleNames = EditorGUILayout.ToggleLeft("Force remove all assetBundle names", buildPreset.AssetBundleBuildPresetData.ForceRemoveAllAssetBundleNames);
                if (buildPreset.AssetBundleBuildPresetData.ForceRemoveAllAssetBundleNames)
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
                buildPreset.AssetBundleBuildPresetData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build target", buildPreset.AssetBundleBuildPresetData.BuildTarget);
                buildPreset.AssetBundleBuildPresetData.AssetBundleCompressType = (AssetBundleCompressType)EditorGUILayout.EnumPopup("Build compression type", buildPreset.AssetBundleBuildPresetData.AssetBundleCompressType);

                buildPreset.AssetBundleBuildPresetData.BuildHandlerIndex = EditorGUILayout.Popup("Build handler", buildPreset.AssetBundleBuildPresetData.BuildHandlerIndex, buildHandlers);
                if (buildPreset.AssetBundleBuildPresetData.BuildHandlerIndex < buildHandlers.Length)
                {
                    buildPreset.AssetBundleBuildPresetData.BuildHandlerName = buildHandlers[buildPreset.AssetBundleBuildPresetData.BuildHandlerIndex];
                }

                buildPreset.AssetBundleBuildPresetData.ResourceBuildType = (ResourceBuildType)EditorGUILayout.EnumPopup("Build type", buildPreset.AssetBundleBuildPresetData.ResourceBuildType);

                buildPreset.AssetBundleBuildPresetData.ForceRebuildAssetBundle = EditorGUILayout.ToggleLeft("Force rebuild assetBundle", buildPreset.AssetBundleBuildPresetData.ForceRebuildAssetBundle);
                buildPreset.AssetBundleBuildPresetData.DisableWriteTypeTree = EditorGUILayout.ToggleLeft("Disable write type tree", buildPreset.AssetBundleBuildPresetData.DisableWriteTypeTree);
                if (buildPreset.AssetBundleBuildPresetData.DisableWriteTypeTree)
                    buildPreset.AssetBundleBuildPresetData.IgnoreTypeTreeChanges = false;

                buildPreset.AssetBundleBuildPresetData.DeterministicAssetBundle = EditorGUILayout.ToggleLeft("Deterministic assetBundle", buildPreset.AssetBundleBuildPresetData.DeterministicAssetBundle);
                buildPreset.AssetBundleBuildPresetData.IgnoreTypeTreeChanges = EditorGUILayout.ToggleLeft("Ignore type tree changes", buildPreset.AssetBundleBuildPresetData.IgnoreTypeTreeChanges);
                if (buildPreset.AssetBundleBuildPresetData.IgnoreTypeTreeChanges)
                    buildPreset.AssetBundleBuildPresetData.DisableWriteTypeTree = false;

                //打包输出的资源加密，如buildInfo，assetbundle 文件名加密
                buildPreset.AssetBundleBuildPresetData.AssetBundleNameType = (AssetBundleNameType)EditorGUILayout.EnumPopup("Build bundle name type ", buildPreset.AssetBundleBuildPresetData.AssetBundleNameType);

            }
            EditorGUILayout.EndVertical();
        }
        void DrawPathOptions()
        {
            EditorGUILayout.LabelField("Path Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildPreset.AssetBundleBuildPresetData.BuildVersion = EditorGUILayout.TextField("Build version", buildPreset.AssetBundleBuildPresetData.BuildVersion);

                if (buildPreset.AssetBundleBuildPresetData.ResourceBuildType == ResourceBuildType.Full)
                {
                    buildPreset.AssetBundleBuildPresetData.InternalBuildVersion = EditorGUILayout.IntField("Internal build version", buildPreset.AssetBundleBuildPresetData.InternalBuildVersion);
                    if (buildPreset.AssetBundleBuildPresetData.InternalBuildVersion < 0)
                        buildPreset.AssetBundleBuildPresetData.InternalBuildVersion = 0;
                }

                EditorGUILayout.BeginHorizontal();
                {
                    buildPreset.AssetBundleBuildPresetData.BuildPath = EditorGUILayout.TextField("Build path", buildPreset.AssetBundleBuildPresetData.BuildPath.Trim());
                    if (GUILayout.Button("Browse", GUILayout.MaxWidth(128)))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("Bundle output path", buildPreset.AssetBundleBuildPresetData.BuildPath, string.Empty);
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            buildPreset.AssetBundleBuildPresetData.BuildPath = newPath.Replace("\\", "/");
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                buildPreset.AssetBundleBuildPresetData.AssetBundleBuildDirectory = Utility.IO.RegularPathCombine(buildPreset.AssetBundleBuildPresetData.BuildPath, buildPreset.AssetBundleBuildPresetData.BuildVersion, buildPreset.AssetBundleBuildPresetData.BuildTarget.ToString());
                if (!string.IsNullOrEmpty(buildPreset.AssetBundleBuildPresetData.BuildVersion))
                {
                    var assetBundleBuildPath = Utility.IO.RegularPathCombine(buildPreset.AssetBundleBuildPresetData.BuildPath, buildPreset.AssetBundleBuildPresetData.BuildVersion, buildPreset.AssetBundleBuildPresetData.BuildTarget.ToString(), $"{buildPreset.AssetBundleBuildPresetData.BuildVersion}");
                    if (buildPreset.AssetBundleBuildPresetData.ResourceBuildType == ResourceBuildType.Full)
                    {
                        assetBundleBuildPath += $"_{buildPreset.AssetBundleBuildPresetData.InternalBuildVersion}";
                    }
                    buildPreset.AssetBundleBuildPresetData.AssetBundleBuildPath = assetBundleBuildPath;
                }
                else
                    EditorGUILayout.HelpBox("BuildVersion is invalid !", MessageType.Error);
                EditorGUILayout.LabelField("Bundle build path", buildPreset.AssetBundleBuildPresetData.AssetBundleBuildPath);
            }
            EditorGUILayout.EndVertical();
        }
        void DrawEncryption()
        {
            EditorGUILayout.LabelField("Encryption Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildPreset.AssetBundleBuildPresetData.EncryptManifest = EditorGUILayout.ToggleLeft("Build info encryption", buildPreset.AssetBundleBuildPresetData.EncryptManifest);
                if (buildPreset.AssetBundleBuildPresetData.EncryptManifest)
                {
                    buildPreset.AssetBundleBuildPresetData.ManifestEncryptionKey = EditorGUILayout.TextField("Build info encryption key", buildPreset.AssetBundleBuildPresetData.ManifestEncryptionKey);
                    var aesKeyStr = buildPreset.AssetBundleBuildPresetData.ManifestEncryptionKey;
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
                buildPreset.AssetBundleBuildPresetData.AssetBundleEncryption = EditorGUILayout.ToggleLeft("AssetBundle encryption", buildPreset.AssetBundleBuildPresetData.AssetBundleEncryption);
                if (buildPreset.AssetBundleBuildPresetData.AssetBundleEncryption)
                {
                    EditorGUILayout.LabelField("AssetBundle encryption offset");
                    buildPreset.AssetBundleBuildPresetData.AssetBundleOffsetValue = EditorGUILayout.IntField("Encryption offset", buildPreset.AssetBundleBuildPresetData.AssetBundleOffsetValue);
                    if (buildPreset.AssetBundleBuildPresetData.AssetBundleOffsetValue < 0)
                        buildPreset.AssetBundleBuildPresetData.AssetBundleOffsetValue = 0;
                }
            }
            EditorGUILayout.EndVertical();
        }
        void DrawBuildDoneOption()
        {
            EditorGUILayout.LabelField("Build Done Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildPreset.AssetBundleBuildPresetData.CopyToStreamingAssets = EditorGUILayout.ToggleLeft("Copy to streaming assets", buildPreset.AssetBundleBuildPresetData.CopyToStreamingAssets);
                if (buildPreset.AssetBundleBuildPresetData.CopyToStreamingAssets)
                {
                    buildPreset.AssetBundleBuildPresetData.ClearStreamingAssetsDestinationPath = EditorGUILayout.ToggleLeft("Clear streaming assets destination path", buildPreset.AssetBundleBuildPresetData.ClearStreamingAssetsDestinationPath);
                    buildPreset.AssetBundleBuildPresetData.UseStreamingAssetsRelativePath = EditorGUILayout.ToggleLeft("Use streaming assets relative path", buildPreset.AssetBundleBuildPresetData.UseStreamingAssetsRelativePath);
                    string destinationPath = string.Empty;
                    if (buildPreset.AssetBundleBuildPresetData.UseStreamingAssetsRelativePath)
                    {
                        EditorGUILayout.LabelField("StreamingAssets  relative path");
                        buildPreset.AssetBundleBuildPresetData.StreamingAssetsRelativePath = EditorGUILayout.TextField("Relative path", buildPreset.AssetBundleBuildPresetData.StreamingAssetsRelativePath);
                        destinationPath = Utility.IO.RegularPathCombine(Application.streamingAssetsPath, buildPreset.AssetBundleBuildPresetData.StreamingAssetsRelativePath);
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
