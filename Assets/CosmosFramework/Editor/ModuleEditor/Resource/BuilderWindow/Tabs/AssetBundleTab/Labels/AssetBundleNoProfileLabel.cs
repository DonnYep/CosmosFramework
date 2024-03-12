using Cosmos.Resource;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class AssetBundleNoProfileLabel
    {
        AssetBundleTab parent;
        bool isAesKeyInvalid = false;

        string[] buildHandlers;
        public const string LabelDataName = "ResourceBuilderWindow_AsseBundleTabNoProfileLabelData.json";
        AssetBundleBuildProfileData profileData;
        public bool IsAesKeyInvalid
        {
            get { return isAesKeyInvalid; }
        }
        public void OnEnable(AssetBundleTab parent, string[] buildHandlers)
        {
            this.parent = parent;
            this.buildHandlers = buildHandlers;
            GetLabelData();
        }
        public void OnGUI()
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
            SaveLabelData();
        }
        public void Reset()
        {
            profileData = new AssetBundleBuildProfileData();
        }
        public ResourceBuildParams GetBuildParams()
        {
            var buildAssetBundleOptions = ResourceEditorUtility.Builder.GetBuildAssetBundleOptions(profileData.AssetBundleCompressType,
                profileData.DisableWriteTypeTree,
                profileData.DeterministicAssetBundle,
                profileData.ForceRebuildAssetBundle,
                profileData.IgnoreTypeTreeChanges);
            var buildParams = new ResourceBuildParams()
            {
                UseProjectRelativeBuildPath = profileData.UseProjectRelativeBuildPath,
                ProjectRelativeBuildPath = profileData.ProjectRelativeBuildPath,
                AssetBundleAbsoluteBuildPath = profileData.AssetBundleAbsoluteBuildPath,
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
                BuildDetailOutputPath = profileData.BuildDetailOutputPath,
                ClearStreamingAssetsDestinationPath = profileData.ClearStreamingAssetsDestinationPath,
                ForceRemoveAllAssetBundleNames = profileData.ForceRemoveAllAssetBundleNames,
                BuildHandlerName = profileData.BuildHandlerName,
                AssetBundleExtension = profileData.UseAssetBundleExtension == true ? profileData.AssetBundleExtension : string.Empty
            };
            return buildParams;
        }
        void GetLabelData()
        {
            profileData = EditorUtil.SafeGetData<AssetBundleBuildProfileData>(ResourceEditorConstants.EDITOR_CACHE_RELATIVE_PATH, LabelDataName);
            var buildHandlerMaxIndex = buildHandlers.Length - 1;
            if (profileData.BuildHandlerIndex > buildHandlerMaxIndex)
            {
                profileData.BuildHandlerIndex = buildHandlerMaxIndex;
            }
        }
        void SaveLabelData()
        {
            EditorUtil.SaveData(ResourceEditorConstants.EDITOR_CACHE_RELATIVE_PATH, LabelDataName, profileData);
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
                profileData.UseProjectRelativeBuildPath = EditorGUILayout.ToggleLeft("Use project relative path", profileData.UseProjectRelativeBuildPath);
                var useProjectRelativeBuildPath = profileData.UseProjectRelativeBuildPath;
                if (useProjectRelativeBuildPath)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        profileData.ProjectRelativeBuildPath = EditorGUILayout.TextField("Project relative path", profileData.ProjectRelativeBuildPath.Trim());
                        if (GUILayout.Button("Browse", GUILayout.MaxWidth(ResourceEditorConstants.BUTTON_WIDTH)))
                        {
                            var relativeBuildPath = EditorUtil.BrowseProjectReativeFolder(profileData.ProjectRelativeBuildPath);
                            if (!string.IsNullOrEmpty(relativeBuildPath))
                            {
                                profileData.ProjectRelativeBuildPath = relativeBuildPath;
                            }
                            else
                            {
                                profileData.ProjectRelativeBuildPath = ResourceEditorConstants.DEFAULT_PROJECT_RELATIVE_BUILD_PATH;
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        profileData.BuildPath = EditorGUILayout.TextField("Build path", profileData.BuildPath.Trim());
                        if (GUILayout.Button("Browse", GUILayout.MaxWidth(ResourceEditorConstants.BUTTON_WIDTH)))
                        {
                            var newPath = EditorUtility.OpenFolderPanel("Bundle output path", profileData.BuildPath, string.Empty);
                            if (!string.IsNullOrEmpty(newPath))
                            {
                                profileData.BuildPath = newPath.Replace("\\", "/");
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (!string.IsNullOrEmpty(profileData.BuildVersion))
                {
                    var abAbsBuildPath = string.Empty;
                    var buildDetailOutputPath = string.Empty;

                    if (useProjectRelativeBuildPath)
                    {
                        abAbsBuildPath = Utility.IO.CombineURL(
                             EditorUtil.ProjectPath,
                             profileData.ProjectRelativeBuildPath,
                             profileData.BuildTarget.ToString(),
                             profileData.BuildVersion);

                        buildDetailOutputPath = Utility.IO.CombineURL(
                            EditorUtil.ProjectPath,
                            profileData.ProjectRelativeBuildPath,
                            profileData.BuildTarget.ToString());
                    }
                    else
                    {
                        abAbsBuildPath = Utility.IO.CombineURL(
                            profileData.BuildPath,
                            profileData.BuildTarget.ToString(),
                            profileData.BuildVersion);

                        buildDetailOutputPath = Utility.IO.CombineURL(
                            profileData.BuildPath,
                            profileData.BuildTarget.ToString());
                    }

                    profileData.BuildDetailOutputPath = buildDetailOutputPath;

                    if (profileData.ResourceBuildType == ResourceBuildType.Full)
                    {
                        abAbsBuildPath += $"_{profileData.InternalBuildVersion}";
                    }
                    profileData.AssetBundleAbsoluteBuildPath = abAbsBuildPath;
                }
                else
                    EditorGUILayout.HelpBox("BuildVersion is invalid !", MessageType.Error);
                EditorGUILayout.LabelField("Bundle build path", profileData.AssetBundleAbsoluteBuildPath);

                profileData.UseAssetBundleExtension = EditorGUILayout.ToggleLeft("Use assetBundle extension", profileData.UseAssetBundleExtension);
                if (profileData.UseAssetBundleExtension)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        profileData.AssetBundleExtension = EditorGUILayout.TextField("AssetBundle extension", profileData.AssetBundleExtension?.Trim());
                        if (GUILayout.Button("Reset extension", GUILayout.MaxWidth(ResourceEditorConstants.BUTTON_WIDTH)))
                        {
                            profileData.AssetBundleExtension = ResourceEditorConstants.DEFAULT_AB_EXTENSION;
                            parent.RepaintWindow();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
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
                    EditorGUILayout.BeginHorizontal();
                    {
                        profileData.ManifestEncryptionKey = EditorGUILayout.TextField("Build info encryption key", profileData.ManifestEncryptionKey);
                        if (GUILayout.Button("Generate aes key", GUILayout.MaxWidth(ResourceEditorConstants.BUTTON_WIDTH)))
                        {
                            profileData.ManifestEncryptionKey = Utility.Text.GenerateRandomString(16);
                            parent.RepaintWindow();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    var aesKeyStr = profileData.ManifestEncryptionKey;
                    var aesKeyLength = Encoding.UTF8.GetBytes(aesKeyStr).Length;
                    EditorGUILayout.LabelField($"Assets AES encryption key, key should be 16,24 or 32 bytes long, current key length is : {aesKeyLength} ");
                    isAesKeyInvalid = ResourceUtility.CheckManifestKeyValidable(aesKeyStr);
                    if (!isAesKeyInvalid)
                    {
                        EditorGUILayout.HelpBox("Encryption key should be 16,24 or 32 bytes long", MessageType.Error);
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
                        destinationPath = Utility.IO.CombineURL(Application.streamingAssetsPath, profileData.StreamingAssetsRelativePath);
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
