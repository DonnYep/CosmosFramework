﻿using System.Text;
using UnityEditor;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class AssetBundleProfileLabel
    {
        AssetBundleTab parent;
        bool isAesKeyInvalid = false;
        public bool HasProfile
        {
            get { return buildProfile != null; }
        }
        public bool IsAesKeyInvalid
        {
            get
            {
                return isAesKeyInvalid;
            }
        }
        string[] buildHandlers;
        AssetBundleBuildProfile buildProfile;

        AssetBundleProfileLabelData labelData;
        public const string LabelDataName = "ResourceBuilderWindow_AsseBundleTabProfileLabelData.json";

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
                buildProfile = (AssetBundleBuildProfile)EditorGUILayout.ObjectField("Build profile", buildProfile, typeof(AssetBundleBuildProfile), false);
                if (GUILayout.Button(createAddNewIcon, GUILayout.MaxWidth(ResourceBuilderWindowConstant.TEXTURE_ICON_WIDTH)))
                {
                    var previousePreset = AssetDatabase.LoadAssetAtPath<AssetBundleBuildProfile>(ResourceBuilderWindowConstant.RESOURCE_NEW_BUILD_PRESET_PATH);
                    if (previousePreset != null)
                    {
                        var canCreate = UnityEditor.EditorUtility.DisplayDialog("AssetBundleBuildProfile already exist", $"Path {ResourceBuilderWindowConstant.RESOURCE_NEW_BUILD_PRESET_PATH} exists.Whether to continue to create and overwrite this file ?", "Create", "Cancel");
                        if (canCreate)
                        {
                            buildProfile = EditorUtil.CreateScriptableObject<AssetBundleBuildProfile>(ResourceBuilderWindowConstant.RESOURCE_NEW_BUILD_PRESET_PATH, HideFlags.NotEditable);
                        }
                    }
                    else
                    {
                        buildProfile = EditorUtil.CreateScriptableObject<AssetBundleBuildProfile>(ResourceBuilderWindowConstant.RESOURCE_NEW_BUILD_PRESET_PATH, HideFlags.NotEditable);
                    }
                }
                if (GUILayout.Button(saveActiveIcon, GUILayout.MaxWidth(ResourceBuilderWindowConstant.TEXTURE_ICON_WIDTH)))
                {
                    EditorUtil.SaveScriptableObject(buildProfile);
                }
            }
            EditorGUILayout.EndHorizontal();
            if (buildProfile == null)
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
            if (buildProfile == null)
                return new ResourceBuildParams();
            var buildAssetBundleOptions = ResourceEditorUtility.Builder.GetBuildAssetBundleOptions(buildProfile.AssetBundleBuildProfileData.AssetBundleCompressType, buildProfile.AssetBundleBuildProfileData.DisableWriteTypeTree,
                buildProfile.AssetBundleBuildProfileData.DeterministicAssetBundle, buildProfile.AssetBundleBuildProfileData.ForceRebuildAssetBundle, buildProfile.AssetBundleBuildProfileData.IgnoreTypeTreeChanges);
            var buildParams = new ResourceBuildParams()
            {
                AssetBundleBuildPath = buildProfile.AssetBundleBuildProfileData.AssetBundleBuildPath,
                AssetBundleEncryption = buildProfile.AssetBundleBuildProfileData.AssetBundleEncryption,
                AssetBundleOffsetValue = buildProfile.AssetBundleBuildProfileData.AssetBundleOffsetValue,
                BuildAssetBundleOptions = buildAssetBundleOptions,
                AssetBundleNameType = buildProfile.AssetBundleBuildProfileData.AssetBundleNameType,
                EncryptManifest = buildProfile.AssetBundleBuildProfileData.EncryptManifest,
                ManifestEncryptionKey = buildProfile.AssetBundleBuildProfileData.ManifestEncryptionKey,
                BuildTarget = buildProfile.AssetBundleBuildProfileData.BuildTarget,
                ResourceBuildType = buildProfile.AssetBundleBuildProfileData.ResourceBuildType,
                BuildVersion = buildProfile.AssetBundleBuildProfileData.BuildVersion,
                InternalBuildVersion = buildProfile.AssetBundleBuildProfileData.InternalBuildVersion,
                CopyToStreamingAssets = buildProfile.AssetBundleBuildProfileData.CopyToStreamingAssets,
                UseStreamingAssetsRelativePath = buildProfile.AssetBundleBuildProfileData.UseStreamingAssetsRelativePath,
                StreamingAssetsRelativePath = buildProfile.AssetBundleBuildProfileData.StreamingAssetsRelativePath,
                AssetBundleBuildDirectory = buildProfile.AssetBundleBuildProfileData.AssetBundleBuildDirectory,
                ClearStreamingAssetsDestinationPath = buildProfile.AssetBundleBuildProfileData.ClearStreamingAssetsDestinationPath,
                ForceRemoveAllAssetBundleNames = buildProfile.AssetBundleBuildProfileData.ForceRemoveAllAssetBundleNames,
                BuildHandlerName = buildProfile.AssetBundleBuildProfileData.BuildHandlerName
            };
            return buildParams;
        }
        void GetTabData()
        {
            try
            {
                labelData = EditorUtil.GetData<AssetBundleProfileLabelData>(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName);
            }
            catch
            {
                labelData = new AssetBundleProfileLabelData();
                EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName, labelData);
            }
            if (!string.IsNullOrEmpty(labelData.ProfilePath))
            {
                buildProfile = AssetDatabase.LoadAssetAtPath<AssetBundleBuildProfile>(labelData.ProfilePath);
                if (buildProfile != null)
                {
                    var buildHandlerMaxIndex = buildHandlers.Length - 1;
                    if (buildProfile.AssetBundleBuildProfileData.BuildHandlerIndex > buildHandlerMaxIndex)
                    {
                        buildProfile.AssetBundleBuildProfileData.BuildHandlerIndex = buildHandlerMaxIndex;
                    }
                }
            }
        }
        void SaveTabData()
        {
            labelData.ProfilePath = AssetDatabase.GetAssetPath(buildProfile);
            EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName, labelData);
            EditorUtil.SaveScriptableObject(buildProfile);
        }
        void DrawPrebuildOptions()
        {
            EditorGUILayout.LabelField("Prebuild Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildProfile.AssetBundleBuildProfileData.ForceRemoveAllAssetBundleNames = EditorGUILayout.ToggleLeft("Force remove all assetBundle names", buildProfile.AssetBundleBuildProfileData.ForceRemoveAllAssetBundleNames);
                if (buildProfile.AssetBundleBuildProfileData.ForceRemoveAllAssetBundleNames)
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
                buildProfile.AssetBundleBuildProfileData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build target", buildProfile.AssetBundleBuildProfileData.BuildTarget);
                buildProfile.AssetBundleBuildProfileData.AssetBundleCompressType = (AssetBundleCompressType)EditorGUILayout.EnumPopup("Build compression type", buildProfile.AssetBundleBuildProfileData.AssetBundleCompressType);

                buildProfile.AssetBundleBuildProfileData.BuildHandlerIndex = EditorGUILayout.Popup("Build handler", buildProfile.AssetBundleBuildProfileData.BuildHandlerIndex, buildHandlers);
                if (buildProfile.AssetBundleBuildProfileData.BuildHandlerIndex < buildHandlers.Length)
                {
                    buildProfile.AssetBundleBuildProfileData.BuildHandlerName = buildHandlers[buildProfile.AssetBundleBuildProfileData.BuildHandlerIndex];
                }

                buildProfile.AssetBundleBuildProfileData.ResourceBuildType = (ResourceBuildType)EditorGUILayout.EnumPopup("Build type", buildProfile.AssetBundleBuildProfileData.ResourceBuildType);

                buildProfile.AssetBundleBuildProfileData.ForceRebuildAssetBundle = EditorGUILayout.ToggleLeft("Force rebuild assetBundle", buildProfile.AssetBundleBuildProfileData.ForceRebuildAssetBundle);
                buildProfile.AssetBundleBuildProfileData.DisableWriteTypeTree = EditorGUILayout.ToggleLeft("Disable write type tree", buildProfile.AssetBundleBuildProfileData.DisableWriteTypeTree);
                if (buildProfile.AssetBundleBuildProfileData.DisableWriteTypeTree)
                    buildProfile.AssetBundleBuildProfileData.IgnoreTypeTreeChanges = false;

                buildProfile.AssetBundleBuildProfileData.DeterministicAssetBundle = EditorGUILayout.ToggleLeft("Deterministic assetBundle", buildProfile.AssetBundleBuildProfileData.DeterministicAssetBundle);
                buildProfile.AssetBundleBuildProfileData.IgnoreTypeTreeChanges = EditorGUILayout.ToggleLeft("Ignore type tree changes", buildProfile.AssetBundleBuildProfileData.IgnoreTypeTreeChanges);
                if (buildProfile.AssetBundleBuildProfileData.IgnoreTypeTreeChanges)
                    buildProfile.AssetBundleBuildProfileData.DisableWriteTypeTree = false;

                //打包输出的资源加密，如buildInfo，assetbundle 文件名加密
                buildProfile.AssetBundleBuildProfileData.AssetBundleNameType = (AssetBundleNameType)EditorGUILayout.EnumPopup("Build bundle name type ", buildProfile.AssetBundleBuildProfileData.AssetBundleNameType);

            }
            EditorGUILayout.EndVertical();
        }
        void DrawPathOptions()
        {
            EditorGUILayout.LabelField("Path Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildProfile.AssetBundleBuildProfileData.BuildVersion = EditorGUILayout.TextField("Build version", buildProfile.AssetBundleBuildProfileData.BuildVersion);

                if (buildProfile.AssetBundleBuildProfileData.ResourceBuildType == ResourceBuildType.Full)
                {
                    buildProfile.AssetBundleBuildProfileData.InternalBuildVersion = EditorGUILayout.IntField("Internal build version", buildProfile.AssetBundleBuildProfileData.InternalBuildVersion);
                    if (buildProfile.AssetBundleBuildProfileData.InternalBuildVersion < 0)
                        buildProfile.AssetBundleBuildProfileData.InternalBuildVersion = 0;
                }

                EditorGUILayout.BeginHorizontal();
                {
                    buildProfile.AssetBundleBuildProfileData.BuildPath = EditorGUILayout.TextField("Build path", buildProfile.AssetBundleBuildProfileData.BuildPath.Trim());
                    if (GUILayout.Button("Browse", GUILayout.MaxWidth(128)))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("Bundle output path", buildProfile.AssetBundleBuildProfileData.BuildPath, string.Empty);
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            buildProfile.AssetBundleBuildProfileData.BuildPath = newPath.Replace("\\", "/");
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                buildProfile.AssetBundleBuildProfileData.AssetBundleBuildDirectory = Utility.IO.RegularPathCombine(buildProfile.AssetBundleBuildProfileData.BuildPath, buildProfile.AssetBundleBuildProfileData.BuildVersion, buildProfile.AssetBundleBuildProfileData.BuildTarget.ToString());
                if (!string.IsNullOrEmpty(buildProfile.AssetBundleBuildProfileData.BuildVersion))
                {
                    var assetBundleBuildPath = Utility.IO.RegularPathCombine(buildProfile.AssetBundleBuildProfileData.BuildPath, buildProfile.AssetBundleBuildProfileData.BuildVersion, buildProfile.AssetBundleBuildProfileData.BuildTarget.ToString(), $"{buildProfile.AssetBundleBuildProfileData.BuildVersion}");
                    if (buildProfile.AssetBundleBuildProfileData.ResourceBuildType == ResourceBuildType.Full)
                    {
                        assetBundleBuildPath += $"_{buildProfile.AssetBundleBuildProfileData.InternalBuildVersion}";
                    }
                    buildProfile.AssetBundleBuildProfileData.AssetBundleBuildPath = assetBundleBuildPath;
                }
                else
                    EditorGUILayout.HelpBox("BuildVersion is invalid !", MessageType.Error);
                EditorGUILayout.LabelField("Bundle build path", buildProfile.AssetBundleBuildProfileData.AssetBundleBuildPath);
            }
            EditorGUILayout.EndVertical();
        }
        void DrawEncryption()
        {
            EditorGUILayout.LabelField("Encryption Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildProfile.AssetBundleBuildProfileData.EncryptManifest = EditorGUILayout.ToggleLeft("Build info encryption", buildProfile.AssetBundleBuildProfileData.EncryptManifest);
                if (buildProfile.AssetBundleBuildProfileData.EncryptManifest)
                {
                    buildProfile.AssetBundleBuildProfileData.ManifestEncryptionKey = EditorGUILayout.TextField("Build info encryption key", buildProfile.AssetBundleBuildProfileData.ManifestEncryptionKey);
                    var aesKeyStr = buildProfile.AssetBundleBuildProfileData.ManifestEncryptionKey;
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
                buildProfile.AssetBundleBuildProfileData.AssetBundleEncryption = EditorGUILayout.ToggleLeft("AssetBundle encryption", buildProfile.AssetBundleBuildProfileData.AssetBundleEncryption);
                if (buildProfile.AssetBundleBuildProfileData.AssetBundleEncryption)
                {
                    EditorGUILayout.LabelField("AssetBundle encryption offset");
                    buildProfile.AssetBundleBuildProfileData.AssetBundleOffsetValue = EditorGUILayout.IntField("Encryption offset", buildProfile.AssetBundleBuildProfileData.AssetBundleOffsetValue);
                    if (buildProfile.AssetBundleBuildProfileData.AssetBundleOffsetValue < 0)
                        buildProfile.AssetBundleBuildProfileData.AssetBundleOffsetValue = 0;
                }
            }
            EditorGUILayout.EndVertical();
        }
        void DrawBuildDoneOption()
        {
            EditorGUILayout.LabelField("Build Done Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                buildProfile.AssetBundleBuildProfileData.CopyToStreamingAssets = EditorGUILayout.ToggleLeft("Copy to streaming assets", buildProfile.AssetBundleBuildProfileData.CopyToStreamingAssets);
                if (buildProfile.AssetBundleBuildProfileData.CopyToStreamingAssets)
                {
                    buildProfile.AssetBundleBuildProfileData.ClearStreamingAssetsDestinationPath = EditorGUILayout.ToggleLeft("Clear streaming assets destination path", buildProfile.AssetBundleBuildProfileData.ClearStreamingAssetsDestinationPath);
                    buildProfile.AssetBundleBuildProfileData.UseStreamingAssetsRelativePath = EditorGUILayout.ToggleLeft("Use streaming assets relative path", buildProfile.AssetBundleBuildProfileData.UseStreamingAssetsRelativePath);
                    string destinationPath = string.Empty;
                    if (buildProfile.AssetBundleBuildProfileData.UseStreamingAssetsRelativePath)
                    {
                        EditorGUILayout.LabelField("StreamingAssets  relative path");
                        buildProfile.AssetBundleBuildProfileData.StreamingAssetsRelativePath = EditorGUILayout.TextField("Relative path", buildProfile.AssetBundleBuildProfileData.StreamingAssetsRelativePath);
                        destinationPath = Utility.IO.RegularPathCombine(Application.streamingAssetsPath, buildProfile.AssetBundleBuildProfileData.StreamingAssetsRelativePath);
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