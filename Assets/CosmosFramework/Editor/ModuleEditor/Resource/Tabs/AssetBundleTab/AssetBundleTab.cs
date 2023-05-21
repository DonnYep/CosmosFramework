using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using Cosmos.Unity.EditorCoroutines.Editor;
using System.Collections;
using Cosmos.Resource;

namespace Cosmos.Editor.Resource
{
    public class AssetBundleTab : ResourceWindowTabBase
    {
        public Func<EditorCoroutine> BuildDataset;
        public const string AssetBundleTabDataName = "ResourceEditor_AsseBundleTabData.json";
        AssetBundleTabData tabData;
        Vector2 scrollPosition;
        bool isAesKeyInvalid = false;
        string[] buildHandlers;
        public override void OnEnable()
        {
            GetTabData();
            GetBuildHandlers();
        }
        public override void OnDisable()
        {
            SaveTabData();
        }
        public override void OnGUI(Rect rect)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawBuildOptions();
            GUILayout.Space(16);
            DrawPathOptions();
            GUILayout.Space(16);
            DrawEncryption();
            GUILayout.Space(16);
            DrawBuildDoneOption();
            GUILayout.Space(16);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Build assetBundle"))
                {
                    if (ResourceWindowDataProxy.ResourceDataset == null)
                    {
                        EditorUtil.Debug.LogError("ResourceDataset is invalid !");
                        return;
                    }
                    if (!isAesKeyInvalid)
                    {
                        EditorUtil.Debug.LogError("Encryption key should be 16,24 or 32 bytes long");
                        return;
                    }
                    var buildParams = new ResourceBuildParams()
                    {
                        AssetBundleBuildPath = tabData.AssetBundleBuildPath,
                        AssetBundleEncryption = tabData.AssetBundleEncryption,
                        AssetBundleOffsetValue = tabData.AssetBundleOffsetValue,
                        BuildAssetBundleOptions = GetBuildAssetBundleOptions(),
                        AssetBundleNameType = tabData.AssetBundleNameType,
                        EncryptManifest = tabData.EncryptManifest,
                        ManifestEncryptionKey = tabData.ManifestEncryptionKey,
                        BuildTarget = tabData.BuildTarget,
                        BuildVersion = $"{tabData.BuildVersion}_{tabData.InternalBuildVersion}",
                        CopyToStreamingAssets = tabData.CopyToStreamingAssets,
                        UseStreamingAssetsRelativePath = tabData.UseStreamingAssetsRelativePath,
                        StreamingAssetsRelativePath = tabData.StreamingAssetsRelativePath
                    };
                    EditorUtil.Coroutine.StartCoroutine(BuildAssetBundle(buildParams, ResourceWindowDataProxy.ResourceDataset));
                }
                if (GUILayout.Button("Reset options"))
                {
                    tabData = new AssetBundleTabData();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
        void DrawBuildOptions()
        {
            EditorGUILayout.LabelField("Build Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                tabData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build target", tabData.BuildTarget);
                tabData.AssetBundleCompressType = (AssetBundleCompressType)EditorGUILayout.EnumPopup("Build compression type", tabData.AssetBundleCompressType);

                tabData.BuildHandlerIndex = EditorGUILayout.Popup("Build handler", tabData.BuildHandlerIndex, buildHandlers);
                if (tabData.BuildHandlerIndex < buildHandlers.Length)
                {
                    tabData.BuildHandlerName = buildHandlers[tabData.BuildHandlerIndex];
                }

                tabData.ForceRebuildAssetBundle = EditorGUILayout.ToggleLeft("Force rebuild assetBundle", tabData.ForceRebuildAssetBundle);
                tabData.DisableWriteTypeTree = EditorGUILayout.ToggleLeft("Disable write type tree", tabData.DisableWriteTypeTree);
                if (tabData.DisableWriteTypeTree)
                    tabData.IgnoreTypeTreeChanges = false;

                tabData.DeterministicAssetBundle = EditorGUILayout.ToggleLeft("Deterministic assetBundle", tabData.DeterministicAssetBundle);
                tabData.IgnoreTypeTreeChanges = EditorGUILayout.ToggleLeft("Ignore type tree changes", tabData.IgnoreTypeTreeChanges);
                if (tabData.IgnoreTypeTreeChanges)
                    tabData.DisableWriteTypeTree = false;

                //打包输出的资源加密，如buildInfo，assetbundle 文件名加密
                tabData.AssetBundleNameType = (AssetBundleNameType)EditorGUILayout.EnumPopup("Build bundle name type ", tabData.AssetBundleNameType);

                tabData.IncrementalBuild = EditorGUILayout.ToggleLeft("Incremental build", tabData.IncrementalBuild);
            }
            EditorGUILayout.EndVertical();
        }
        void DrawPathOptions()
        {
            EditorGUILayout.LabelField("Path Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                tabData.BuildVersion = EditorGUILayout.TextField("Build version", tabData.BuildVersion);
                tabData.InternalBuildVersion = EditorGUILayout.IntField("Internal build version", tabData.InternalBuildVersion);
                if (tabData.InternalBuildVersion < 0)
                    tabData.InternalBuildVersion = 0;

                EditorGUILayout.BeginHorizontal();
                {
                    tabData.BuildPath = EditorGUILayout.TextField("Build path", tabData.BuildPath.Trim());
                    if (GUILayout.Button("Browse", GUILayout.MaxWidth(128)))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("Bundle output path", tabData.BuildPath, string.Empty);
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            tabData.BuildPath = newPath.Replace("\\", "/");
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (!string.IsNullOrEmpty(tabData.BuildVersion))
                {
                    tabData.AssetBundleBuildPath = Utility.IO.WebPathCombine(tabData.BuildPath, tabData.BuildVersion, tabData.BuildTarget.ToString(), $"{tabData.BuildVersion}_{tabData.InternalBuildVersion}");
                }
                else
                    EditorGUILayout.HelpBox("BuildVersion is invalid !", MessageType.Error);
                EditorGUILayout.LabelField("Bundle build path", tabData.AssetBundleBuildPath);
            }
            EditorGUILayout.EndVertical();
        }
        void DrawEncryption()
        {
            EditorGUILayout.LabelField("Encryption Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                tabData.EncryptManifest = EditorGUILayout.ToggleLeft("Build info encryption", tabData.EncryptManifest);
                if (tabData.EncryptManifest)
                {
                    tabData.ManifestEncryptionKey = EditorGUILayout.TextField("Build info encryption key", tabData.ManifestEncryptionKey);
                    var aesKeyStr = tabData.ManifestEncryptionKey;
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
                tabData.AssetBundleEncryption = EditorGUILayout.ToggleLeft("AssetBundle encryption", tabData.AssetBundleEncryption);
                if (tabData.AssetBundleEncryption)
                {
                    EditorGUILayout.LabelField("AssetBundle encryption offset");
                    tabData.AssetBundleOffsetValue = EditorGUILayout.IntField("Encryption offset", tabData.AssetBundleOffsetValue);
                    if (tabData.AssetBundleOffsetValue < 0)
                        tabData.AssetBundleOffsetValue = 0;
                }
            }
            EditorGUILayout.EndVertical();
        }
        void DrawBuildDoneOption()
        {
            EditorGUILayout.LabelField("Build Done Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                tabData.CopyToStreamingAssets = EditorGUILayout.ToggleLeft("Copy to streaming assets", tabData.CopyToStreamingAssets);
                if (tabData.CopyToStreamingAssets)
                {
                    tabData.UseStreamingAssetsRelativePath = EditorGUILayout.ToggleLeft("Use streaming assets relative path", tabData.UseStreamingAssetsRelativePath);
                    if (tabData.UseStreamingAssetsRelativePath)
                    {
                        EditorGUILayout.LabelField("StreamingAssets  relative path");
                        tabData.StreamingAssetsRelativePath = EditorGUILayout.TextField("Relative path", tabData.StreamingAssetsRelativePath);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
        void GetTabData()
        {
            try
            {
                tabData = EditorUtil.GetData<AssetBundleTabData>(AssetBundleTabDataName);
            }
            catch
            {
                tabData = new AssetBundleTabData();
                EditorUtil.SaveData(AssetBundleTabDataName, tabData);
            }
        }
        void SaveTabData()
        {
            EditorUtil.SaveData(AssetBundleTabDataName, tabData);
        }
        void GetBuildHandlers()
        {
            var srcBuildHandlers = Utility.Assembly.GetDerivedTypeNames<IResourceBuildHandler>();
            buildHandlers = new string[srcBuildHandlers.Length + 1];
            buildHandlers[0] = Constants.NONE;
            Array.Copy(srcBuildHandlers, 0, buildHandlers, 1, srcBuildHandlers.Length);

            var buildHandlerMaxIndex = buildHandlers.Length - 1;
            if (tabData.BuildHandlerIndex > buildHandlerMaxIndex)
            {
                tabData.BuildHandlerIndex = buildHandlerMaxIndex;
            }
        }
        IEnumerator BuildAssetBundle(ResourceBuildParams buildParams, ResourceDataset dataset)
        {
            yield return BuildDataset.Invoke();
            ResourceManifest resourceManifest = new ResourceManifest();
            var bundleInfos = dataset.GetResourceBundleInfos();
            ResourceBuildController.PrepareBuildAssetBundle(buildParams, bundleInfos, ref resourceManifest);
            var resourceBuildHandler = Utility.Assembly.GetTypeInstance<IResourceBuildHandler>(tabData.BuildHandlerName);
            if (resourceBuildHandler != null)
            {
                resourceBuildHandler.OnBuildPrepared(buildParams);
            }
            var unityManifest = BuildPipeline.BuildAssetBundles(buildParams.AssetBundleBuildPath, buildParams.BuildAssetBundleOptions, buildParams.BuildTarget);
            ResourceBuildController.ProcessAssetBundle(buildParams, bundleInfos, unityManifest, ref resourceManifest);
            ResourceBuildController.PorcessManifest(buildParams, ref resourceManifest);
            ResourceBuildController.BuildDoneOption(buildParams);
            if (resourceBuildHandler != null)
            {
                resourceBuildHandler.OnBuildComplete(buildParams);
            }

        }
        BuildAssetBundleOptions GetBuildAssetBundleOptions()
        {
            BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
            var compressType = tabData.AssetBundleCompressType;
            switch (compressType)
            {
                case AssetBundleCompressType.Uncompressed:
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                    break;
                case AssetBundleCompressType.StandardCompression_LZMA:
                    //None=StandardCompression_LZMA
                    break;
                case AssetBundleCompressType.ChunkBasedCompression_LZ4:
                    options |= BuildAssetBundleOptions.ChunkBasedCompression;
                    break;
            }
            if (tabData.DisableWriteTypeTree)
                options |= BuildAssetBundleOptions.DisableWriteTypeTree;
            if (tabData.DeterministicAssetBundle)
                options |= BuildAssetBundleOptions.DeterministicAssetBundle;
            if (tabData.ForceRebuildAssetBundle)
                options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            if (tabData.IgnoreTypeTreeChanges)
                options |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;
            return options;
        }
    }
}
