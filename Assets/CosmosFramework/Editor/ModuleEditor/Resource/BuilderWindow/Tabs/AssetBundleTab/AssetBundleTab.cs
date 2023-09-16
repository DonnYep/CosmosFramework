using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections;
using Cosmos.Resource;
using System.Collections.Generic;

namespace Cosmos.Editor.Resource
{
    public class AssetBundleTab : ResourceBuilderWindowTabBase
    {
        public Func<Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutine> BuildDataset;
        public const string AssetBundleTabDataName = "ResourceBuilderWindow_AsseBundleTabData.json";
        AssetBundleTabData tabData;
        Vector2 scrollPosition;
        string[] buildHandlers;
        AssetBundleNoPresetLabel noPresetLabel = new AssetBundleNoPresetLabel();
        AssetBundlePresetLabel presetLabel = new AssetBundlePresetLabel();
        public AssetBundleTab(EditorWindow parentWindow) : base(parentWindow)
        {
        }

        public override void OnEnable()
        {
            GetTabData();
            buildHandlers = EditorUtil.GetDerivedTypeHandlers<IResourceBuildHandler>();
            noPresetLabel.OnEnable(this, buildHandlers);
            presetLabel.OnEnable(this, buildHandlers);
        }
        public override void OnDisable()
        {
            SaveTabData();
            noPresetLabel.OnDisable();
            presetLabel.OnDisable();
        }
        public override void OnGUI(Rect rect)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.LabelField("Build Preset Options", EditorStyles.boldLabel);
            tabData.UseBuildPreset = EditorGUILayout.ToggleLeft("Use build preset", tabData.UseBuildPreset);
            if (tabData.UseBuildPreset)
            {
                presetLabel.OnGUI(rect);
            }
            else
            {
                noPresetLabel.OnGUI(rect);
            }
            GUILayout.Space(16);
            DrawBuildButton();
            EditorGUILayout.EndScrollView();
        }
        void DrawBuildButton()
        {
            EditorGUILayout.BeginHorizontal();
            {
                var buttonWidth = EditorGUIUtility.currentViewWidth / 2;
                if (GUILayout.Button("Build assetBundle", GUILayout.MaxWidth(buttonWidth)))
                {
                    if (ResourceBuilderWindowDataProxy.ResourceDataset == null)
                    {
                        EditorUtil.Debug.LogError("ResourceDataset is invalid !");
                        return;
                    }

                    ResourceBuildParams buildParams = default;
                    bool isAesKeyInvalid = false;
                    if (tabData.UseBuildPreset)
                    {
                        buildParams = presetLabel.GetBuildParams();
                        isAesKeyInvalid = presetLabel.IsAesKeyInvalid;
                    }
                    else
                    {
                        buildParams = noPresetLabel.GetBuildParams();
                        isAesKeyInvalid = noPresetLabel.IsAesKeyInvalid;
                    }

                    if (!isAesKeyInvalid)
                    {
                        EditorUtil.Debug.LogError("Encryption key should be 16,24 or 32 bytes long");
                        return;
                    }

                    if (buildParams.ForceRemoveAllAssetBundleNames)
                        AssetBundleCommand.ForceRemoveAllAssetBundleNames();
                    EditorUtil.Coroutine.StartCoroutine(BuildAssetBundle(buildParams, ResourceBuilderWindowDataProxy.ResourceDataset));
                }
                if (GUILayout.Button("Reset options", GUILayout.MaxWidth(buttonWidth)))
                {
                    tabData = new AssetBundleTabData();
                    ParentWindow.Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        void GetTabData()
        {
            try
            {
                tabData = EditorUtil.GetData<AssetBundleTabData>(ResourceEditorConstants.CACHE_RELATIVE_PATH, AssetBundleTabDataName);
            }
            catch
            {
                tabData = new AssetBundleTabData();
                EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, AssetBundleTabDataName, tabData);
            }
        }
        void SaveTabData()
        {
            EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, AssetBundleTabDataName, tabData);
        }
        IEnumerator BuildAssetBundle(ResourceBuildParams buildParams, ResourceDataset dataset)
        {
            yield return BuildDataset.Invoke();
            ResourceManifest resourceManifest = new ResourceManifest();
            switch (buildParams.ResourceBuildType)
            {
                case ResourceBuildType.Full:
                    BuildFullAssetBundle(buildParams, dataset, resourceManifest);
                    break;
                case ResourceBuildType.Incremental:
                    BuildIncrementalAssetBundle(buildParams, dataset, resourceManifest);
                    break;
            }
        }
        void BuildFullAssetBundle(ResourceBuildParams buildParams, ResourceDataset dataset, ResourceManifest resourceManifest)
        {
            var bundleInfos = dataset.GetResourceBundleInfos();
            ResourceBuildController.PrepareBuildAssetBundle(buildParams, bundleInfos, ref resourceManifest);
            var buildHandlerName = buildParams.BuildHandlerName;
            var resourceBuildHandler = Utility.Assembly.GetTypeInstance<IResourceBuildHandler>(buildHandlerName);
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
            ResourceBuildController.RevertAssetBundlesName(bundleInfos);
        }
        void BuildIncrementalAssetBundle(ResourceBuildParams buildParams, ResourceDataset dataset, ResourceManifest resourceManifest)
        {
            var bundleInfos = dataset.GetResourceBundleInfos();
            ResourceBuildController.CompareIncrementalBuildCache(buildParams, bundleInfos, out var cacheCompareResult);

            ResourceBuildController.PrepareBuildAssetBundle(buildParams, bundleInfos, ref resourceManifest);
            var buildHandlerName = buildParams.BuildHandlerName;
            var resourceBuildHandler = Utility.Assembly.GetTypeInstance<IResourceBuildHandler>(buildHandlerName);
            if (resourceBuildHandler != null)
            {
                resourceBuildHandler.OnBuildPrepared(buildParams);
            }

            var needBuildBundles = new List<ResourceBundleCacheInfo>();
            needBuildBundles.AddRange(cacheCompareResult.Changed);
            needBuildBundles.AddRange(cacheCompareResult.NewlyAdded);
            var length = needBuildBundles.Count;
            var abBuildList = new List<AssetBundleBuild>();

            if (length > 0)
            {
                EditorUtil.Debug.LogInfo($"{length } bundles  changed !");
                for (int i = 0; i < length; i++)
                {
                    AssetBundleBuild assetBundleBuild = default;
                    var cacheInfo = needBuildBundles[i];
                    switch (buildParams.AssetBundleNameType)
                    {
                        case AssetBundleNameType.DefaultName:
                            {
                                assetBundleBuild = new AssetBundleBuild()
                                {
                                    assetBundleName = cacheInfo.BundleName,
                                    assetNames = cacheInfo.AssetNames
                                };
                            }
                            break;
                        case AssetBundleNameType.HashInstead:
                            {
                                assetBundleBuild = new AssetBundleBuild()
                                {
                                    assetBundleName = cacheInfo.BundleHash,
                                    assetNames = cacheInfo.AssetNames
                                };
                            }
                            break;
                    }
                    abBuildList.Add(assetBundleBuild);
                }
                var unityManifest = BuildPipeline.BuildAssetBundles(buildParams.AssetBundleBuildPath, abBuildList.ToArray(), buildParams.BuildAssetBundleOptions, buildParams.BuildTarget);

                ResourceBuildController.ProcessAssetBundle(buildParams, bundleInfos, unityManifest, ref resourceManifest);
                ResourceBuildController.PorcessManifest(buildParams, ref resourceManifest);
                ResourceBuildController.BuildDoneOption(buildParams);
                if (resourceBuildHandler != null)
                {
                    resourceBuildHandler.OnBuildComplete(buildParams);
                }
                ResourceBuildController.GenerateIncrementalBuildLog(buildParams, cacheCompareResult);
            }
            else
            {
                EditorUtil.Debug.LogInfo("No bundle changed !");
            }
            ResourceBuildController.RevertAssetBundlesName(bundleInfos);
        }
    }
}
