using System;
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
        public AssetBundleTabData TabData
        {
            get { return tabData; }
        }
        Vector2 scrollPosition;
        string[] buildHandlers;
        AssetBundleNoProfileLabel noProfileLabel = new AssetBundleNoProfileLabel();
        AssetBundleProfileLabel profileLabel = new AssetBundleProfileLabel();
        public AssetBundleTab(EditorWindow parentWindow) : base(parentWindow)
        {
        }

        public override void OnEnable()
        {
            tabData = EditorUtil.SafeGetData<AssetBundleTabData>(ResourceEditorConstants.EDITOR_CACHE_RELATIVE_PATH, AssetBundleTabDataName);
            buildHandlers = EditorUtil.GetDerivedTypeHandlers<IResourceBuildHandler>();
            noProfileLabel.OnEnable(this, buildHandlers);
            profileLabel.OnEnable(this, buildHandlers);
        }
        public override void OnDisable()
        {
            noProfileLabel.OnDisable();
            profileLabel.OnDisable();
            SaveTabData();
        }
        public override void OnGUI(Rect rect)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.LabelField("Build Profile Options", EditorStyles.boldLabel);
            tabData.UseBuildProfile = EditorGUILayout.ToggleLeft("Use build profile", tabData.UseBuildProfile);
            if (tabData.UseBuildProfile)
            {
                profileLabel.OnGUI();
            }
            else
            {
                noProfileLabel.OnGUI();
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
                    if (tabData.UseBuildProfile)
                    {
                        if (!profileLabel.HasProfile)
                        {
                            EditorUtil.Debug.LogError("AssetBundleBuildProfile is invalid !");
                            return;
                        }
                        buildParams = profileLabel.GetBuildParams();
                        isAesKeyInvalid = profileLabel.IsAesKeyInvalid;
                    }
                    else
                    {
                        buildParams = noProfileLabel.GetBuildParams();
                        isAesKeyInvalid = noProfileLabel.IsAesKeyInvalid;
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
                    if (tabData.UseBuildProfile)
                        profileLabel.Reset();
                    else
                        noProfileLabel.Reset();
                    ParentWindow.Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        void SaveTabData()
        {
            EditorUtil.SaveData(ResourceEditorConstants.EDITOR_CACHE_RELATIVE_PATH, AssetBundleTabDataName, tabData);
        }
        IEnumerator BuildAssetBundle(ResourceBuildParams buildParams, ResourceDataset dataset)
        {
            var startTime = DateTime.Now;

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
            var endTime = DateTime.Now;
            var elapsedTime = endTime - startTime;
            EditorUtil.Debug.LogInfo($"Assetbundle build done , elapsed time {elapsedTime.Hours}h :{elapsedTime.Minutes}m :{elapsedTime.Seconds}s :{elapsedTime.Milliseconds}ms");
        }
        void BuildFullAssetBundle(ResourceBuildParams buildParams, ResourceDataset dataset, ResourceManifest resourceManifest)
        {
            var bundleInfos = dataset.GetResourceBundleInfos();
            var extensions = dataset.ResourceAvailableExtenisonList.ToArray();
            ResourceBuildController.PrepareBuildAssetBundle(buildParams, bundleInfos, extensions, ref resourceManifest);
            var buildHandlerName = buildParams.BuildHandlerName;
            var resourceBuildHandler = Utility.Assembly.GetTypeInstance<IResourceBuildHandler>(buildHandlerName);
            if (resourceBuildHandler != null)
            {
                resourceBuildHandler.OnBuildPrepared(buildParams);
            }
            var unityManifest = BuildPipeline.BuildAssetBundles(buildParams.AssetBundleAbsoluteBuildPath, buildParams.BuildAssetBundleOptions, buildParams.BuildTarget);
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
            var extensions = dataset.ResourceAvailableExtenisonList.ToArray();
            ResourceBuildController.CompareIncrementalBuildCache(buildParams, bundleInfos, extensions, out var cacheCompareResult);

            ResourceBuildController.PrepareBuildAssetBundle(buildParams, bundleInfos, extensions,ref resourceManifest);
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
                var unityManifest = BuildPipeline.BuildAssetBundles(buildParams.AssetBundleAbsoluteBuildPath, abBuildList.ToArray(), buildParams.BuildAssetBundleOptions, buildParams.BuildTarget);

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
