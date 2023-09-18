using System.Text;
using UnityEditor;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class AssetBundleNoProfileLabel
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
        public const string LabelDataName = "ResourceBuilderWindow_AsseBundleTabNoProfileLabelData.json";
        AssetBundleBuildProfileData profileData;

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
        public void Reset()
        {
            profileData = new AssetBundleBuildProfileData();
        }
        public ResourceBuildParams GetBuildParams()
        {
            var buildAssetBundleOptions = ResourceEditorUtility.Builder.GetBuildAssetBundleOptions(profileData.AssetBundleCompressType, profileData.DisableWriteTypeTree,
                profileData.DeterministicAssetBundle, profileData.ForceRebuildAssetBundle, profileData.IgnoreTypeTreeChanges);
            var buildParams = new ResourceBuildParams()
            {
                AssetBundleBuildPath = profileData.AssetBundleBuildPath,
                AssetBundleEncryption = profileData.AssetBundleEncryption,
                AssetBundleOffsetValue = profileData.AssetBundleOffsetValue,
                BuildAssetBundleOptions = buildAssetBundleOptions,
                AssetBundleNameType = profileData.AssetBundleNameType,
                EncryptManifest = profileData.EncryptManifest,
                ManifestEncryptionKey = profileData.ManifestEncryptionKey,
                BuildTarget = profileData.BuildTarget,
                ResourceBuildType = profileData.ResourceBuildType,
                BuildVersion = profileData.BuildVersion,
                InternalBuildVersion = profileData.InternalBuildVersion,
                CopyToStreamingAssets = profileData.CopyToStreamingAssets,
                UseStreamingAssetsRelativePath = profileData.UseStreamingAssetsRelativePath,
                StreamingAssetsRelativePath = profileData.StreamingAssetsRelativePath,
                AssetBundleBuildDirectory = profileData.AssetBundleBuildDirectory,
                ClearStreamingAssetsDestinationPath = profileData.ClearStreamingAssetsDestinationPath,
                ForceRemoveAllAssetBundleNames = profileData.ForceRemoveAllAssetBundleNames,
                BuildHandlerName = profileData.BuildHandlerName
            };
            return buildParams;
        }
        void GetTabData()
        {
            try
            {
                profileData = EditorUtil.GetData<AssetBundleBuildProfileData>(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName);
                var buildHandlerMaxIndex = buildHandlers.Length - 1;
                if (profileData.BuildHandlerIndex > buildHandlerMaxIndex)
                {
                    profileData.BuildHandlerIndex = buildHandlerMaxIndex;
                }
            }
            catch
            {
                profileData = new AssetBundleBuildProfileData();
                EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName, profileData);
            }
        }
        void SaveTabData()
        {
            EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, LabelDataName, profileData);
        }
        void DrawPrebuildOptions()
        {
            EditorGUILayout.LabelField("Prebuild Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                profileData.ForceRemoveAllAssetBundleNames = EditorGUILayout.ToggleLeft("Force remove all assetBundle names", profileData.ForceRemoveAllAssetBundleNames);
                if (profileData.ForceRemoveAllAssetBundleNames)
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
                profileData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build target", profileData.BuildTarget);
                profileData.AssetBundleCompressType = (AssetBundleCompressType)EditorGUILayout.EnumPopup("Build compression type", profileData.AssetBundleCompressType);

                profileData.BuildHandlerIndex = EditorGUILayout.Popup("Build handler", profileData.BuildHandlerIndex, buildHandlers);
                if (profileData.BuildHandlerIndex < buildHandlers.Length)
                {
                    profileData.BuildHandlerName = buildHandlers[profileData.BuildHandlerIndex];
                }

                profileData.ResourceBuildType = (ResourceBuildType)EditorGUILayout.EnumPopup("Build type", profileData.ResourceBuildType);

                profileData.ForceRebuildAssetBundle = EditorGUILayout.ToggleLeft("Force rebuild assetBundle", profileData.ForceRebuildAssetBundle);
                profileData.DisableWriteTypeTree = EditorGUILayout.ToggleLeft("Disable write type tree", profileData.DisableWriteTypeTree);
                if (profileData.DisableWriteTypeTree)
                    profileData.IgnoreTypeTreeChanges = false;

                profileData.DeterministicAssetBundle = EditorGUILayout.ToggleLeft("Deterministic assetBundle", profileData.DeterministicAssetBundle);
                profileData.IgnoreTypeTreeChanges = EditorGUILayout.ToggleLeft("Ignore type tree changes", profileData.IgnoreTypeTreeChanges);
                if (profileData.IgnoreTypeTreeChanges)
                    profileData.DisableWriteTypeTree = false;

                //打包输出的资源加密，如buildInfo，assetbundle 文件名加密
                profileData.AssetBundleNameType = (AssetBundleNameType)EditorGUILayout.EnumPopup("Build bundle name type ", profileData.AssetBundleNameType);

            }
            EditorGUILayout.EndVertical();
        }
        void DrawPathOptions()
        {
            EditorGUILayout.LabelField("Path Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                profileData.BuildVersion = EditorGUILayout.TextField("Build version", profileData.BuildVersion);

                if (profileData.ResourceBuildType == ResourceBuildType.Full)
                {
                    profileData.InternalBuildVersion = EditorGUILayout.IntField("Internal build version", profileData.InternalBuildVersion);
                    if (profileData.InternalBuildVersion < 0)
                        profileData.InternalBuildVersion = 0;
                }

                EditorGUILayout.BeginHorizontal();
                {
                    profileData.BuildPath = EditorGUILayout.TextField("Build path", profileData.BuildPath.Trim());
                    if (GUILayout.Button("Browse", GUILayout.MaxWidth(128)))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("Bundle output path", profileData.BuildPath, string.Empty);
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            profileData.BuildPath = newPath.Replace("\\", "/");
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                profileData.AssetBundleBuildDirectory = Utility.IO.RegularPathCombine(profileData.BuildPath, profileData.BuildVersion, profileData.BuildTarget.ToString());
                if (!string.IsNullOrEmpty(profileData.BuildVersion))
                {
                    var assetBundleBuildPath = Utility.IO.RegularPathCombine(profileData.BuildPath, profileData.BuildVersion, profileData.BuildTarget.ToString(), $"{profileData.BuildVersion}");
                    if (profileData.ResourceBuildType == ResourceBuildType.Full)
                    {
                        assetBundleBuildPath += $"_{profileData.InternalBuildVersion}";
                    }
                    profileData.AssetBundleBuildPath = assetBundleBuildPath;
                }
                else
                    EditorGUILayout.HelpBox("BuildVersion is invalid !", MessageType.Error);
                EditorGUILayout.LabelField("Bundle build path", profileData.AssetBundleBuildPath);
            }
            EditorGUILayout.EndVertical();
        }
        void DrawEncryption()
        {
            EditorGUILayout.LabelField("Encryption Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                profileData.EncryptManifest = EditorGUILayout.ToggleLeft("Build info encryption", profileData.EncryptManifest);
                if (profileData.EncryptManifest)
                {
                    profileData.ManifestEncryptionKey = EditorGUILayout.TextField("Build info encryption key", profileData.ManifestEncryptionKey);
                    var aesKeyStr = profileData.ManifestEncryptionKey;
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
                profileData.AssetBundleEncryption = EditorGUILayout.ToggleLeft("AssetBundle encryption", profileData.AssetBundleEncryption);
                if (profileData.AssetBundleEncryption)
                {
                    EditorGUILayout.LabelField("AssetBundle encryption offset");
                    profileData.AssetBundleOffsetValue = EditorGUILayout.IntField("Encryption offset", profileData.AssetBundleOffsetValue);
                    if (profileData.AssetBundleOffsetValue < 0)
                        profileData.AssetBundleOffsetValue = 0;
                }
            }
            EditorGUILayout.EndVertical();
        }
        void DrawBuildDoneOption()
        {
            EditorGUILayout.LabelField("Build Done Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                profileData.CopyToStreamingAssets = EditorGUILayout.ToggleLeft("Copy to streaming assets", profileData.CopyToStreamingAssets);
                if (profileData.CopyToStreamingAssets)
                {
                    profileData.ClearStreamingAssetsDestinationPath = EditorGUILayout.ToggleLeft("Clear streaming assets destination path", profileData.ClearStreamingAssetsDestinationPath);
                    profileData.UseStreamingAssetsRelativePath = EditorGUILayout.ToggleLeft("Use streaming assets relative path", profileData.UseStreamingAssetsRelativePath);
                    string destinationPath = string.Empty;
                    if (profileData.UseStreamingAssetsRelativePath)
                    {
                        EditorGUILayout.LabelField("StreamingAssets  relative path");
                        profileData.StreamingAssetsRelativePath = EditorGUILayout.TextField("Relative path", profileData.StreamingAssetsRelativePath);
                        destinationPath = Utility.IO.RegularPathCombine(Application.streamingAssetsPath, profileData.StreamingAssetsRelativePath);
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
