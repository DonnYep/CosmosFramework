using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Cosmos.QuarkAsset;
using System.Linq;

namespace Cosmos.CosmosEditor
{
    public class AssetBundleBuildTab
    {
        AssetBundleBuildTabData assetBundleTabData;
        const string AssetBundleTabDataFileName = "AssetBundleTabData.json";
        string streamingPath = "Assets/StreamingAssets";
        const string quarkABBuildInfo = "BuildInfo.json";
        const string quarkManifest = "Manifest.json";
        Dictionary<string, AssetImporter> importerCacheDict = new Dictionary<string, AssetImporter>();
        QuarkAssetABBuildInfo abBuildInfo = new QuarkAssetABBuildInfo();
        QuarkAssetManifest quarkAssetManifest = new QuarkAssetManifest();
        Dictionary<string, string> buildInfoCache = new Dictionary<string, string>();
        QuarkAssetDataset QuarkAssetDataset { get { return QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance; } }

        public void Clear()
        {
        }
        public void OnDisable()
        {
            EditorUtilities.SaveData(AssetBundleTabDataFileName, assetBundleTabData);
        }
        public void OnEnable()
        {
            try
            {
                assetBundleTabData = EditorUtilities.GetData<AssetBundleBuildTabData>(AssetBundleTabDataFileName);
            }
            catch
            {
                assetBundleTabData = new AssetBundleBuildTabData();
                EditorUtilities.SaveData(AssetBundleTabDataFileName, assetBundleTabData);
            }
        }
        public void OnGUI()
        {
            DrawAssetBundleTab();
        }
        void DrawAssetBundleTab()
        {
            assetBundleTabData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", assetBundleTabData.BuildTarget);
            assetBundleTabData.OutputPath = EditorGUILayout.TextField("OutputPath", assetBundleTabData.OutputPath);
            EditorUtilities.DrawHorizontalContext(() =>
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Browse", GUILayout.MaxWidth(92f)))
                {
                    BrowseForFolder();
                }
                if (GUILayout.Button("Reset", GUILayout.MaxWidth(92f)))
                {
                    ResetPath();
                }
                if (GUILayout.Button("OpenFolder", GUILayout.MaxWidth(92f)))
                {
                    EditorUtility.RevealInFinder(GetBuildFolder());
                }
            });
            GUILayout.Space(16);
            EditorUtilities.DrawVerticalContext(() =>
            {
                assetBundleTabData.ClearFolders = EditorGUILayout.ToggleLeft("ClearFolders", assetBundleTabData.ClearFolders);
                assetBundleTabData.CopyToStreamingAssets = EditorGUILayout.ToggleLeft("CopyToStreamingAssets", assetBundleTabData.CopyToStreamingAssets);
                assetBundleTabData.WithoutManifest = EditorGUILayout.ToggleLeft("WithoutManifest", assetBundleTabData.WithoutManifest);
            });
            GUILayout.Space(16);
            assetBundleTabData.BuildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("CompressedFormat:", assetBundleTabData.BuildAssetBundleOptions);
            assetBundleTabData.EncryptionKey = EditorGUILayout.TextField("EncryptionKey", assetBundleTabData.EncryptionKey);
            assetBundleTabData.NameHashType = (AssetBundleHashType)EditorGUILayout.EnumPopup("NameHashType", assetBundleTabData.NameHashType);
            GUILayout.Space(16);

            EditorUtilities.DrawHorizontalContext(() =>
            {
                if (GUILayout.Button("Build"))
                {
                    BuildAssetBundle();
                }
                if (GUILayout.Button("Reset"))
                {
                    assetBundleTabData = new AssetBundleBuildTabData();
                    EditorUtilities.SaveData(AssetBundleTabDataFileName, assetBundleTabData);
                }
            });
        }
        void BrowseForFolder()
        {
            assetBundleTabData.UseDefaultPath = false;
            var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", assetBundleTabData.OutputPath, string.Empty);
            if (!string.IsNullOrEmpty(newPath))
            {
                var gamePath = EditorUtilities.ApplicationPath();
                gamePath = gamePath.Replace("\\", "/");
                if (newPath.StartsWith(gamePath) && newPath.Length > gamePath.Length)
                    newPath = newPath.Remove(0, gamePath.Length + 1);
                assetBundleTabData.OutputPath = newPath;
            }
        }
        void ResetPath()
        {
            assetBundleTabData.UseDefaultPath = true;
            assetBundleTabData.OutputPath = "AssetBundles/";
            assetBundleTabData.OutputPath += assetBundleTabData.BuildTarget.ToString();
        }
        string GetBuildFolder()
        {
            var path = Utility.IO.Combine(EditorUtilities.ApplicationPath(), assetBundleTabData.OutputPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        string GetBuildPath()
        {
            var path = Utility.IO.Combine(EditorUtilities.ApplicationPath(), assetBundleTabData.OutputPath);
            return path;
        }
        void BuildAssetBundle()
        {
            SetBuildInfo();
            BuildPipeline.BuildAssetBundles(GetBuildFolder(), assetBundleTabData.BuildAssetBundleOptions, assetBundleTabData.BuildTarget);
            OperateManifest();
        }
        void SetBuildInfo()
        {
            EditorUtilities.Debug.LogInfo("Start build asset bundle");
            if (assetBundleTabData.ClearFolders)
            {
                var path = Utility.IO.Combine(EditorUtilities.ApplicationPath(), assetBundleTabData.OutputPath);
                var streamPath = Utility.IO.Combine(EditorUtilities.ApplicationPath(), streamingPath);
                if (Directory.Exists(path))
                {
                    Utility.IO.DeleteFolder(path);
                }
                try
                {
                    Utility.IO.DeleteFolder(streamPath);
                }
                catch { }
            }
            if (QuarkAssetWindow.WindowTabData.UnderAssetsDirectory)
            {
                EditorUtilities.Coroutine.StartCoroutine(EnumBuild());
            }
            else
            {
                var dirs = QuarkAssetDataset.IncludeDirectories;
                TraverseTargetDirectories(dirs.ToArray());
            }
        }
        IEnumerator EnumBuild()
        {
            var dirs = EditorUtilities.IO.GetAllBundleableFilePath();
            TraverseTargetDirectories(dirs);
            yield return null;
        }
        void TraverseTargetDirectories(string[] dirs)
        {
            foreach (var dir in dirs)
            {
                SetAssetBundleName(dir);
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            foreach (var map in abBuildInfo.AssetDataMaps)
            {
                map.Value.DependList = AssetDatabase.GetAssetBundleDependencies(map.Value.ABName, true).ToList();
                buildInfoCache.TryAdd(map.Value.ABName, map.Key);
            }
        }
        void ResetBuildInfo()
        {
            EditorUtilities.Debug.LogInfo("Asset bundle built done");
            foreach (var imp in importerCacheDict)
            {
                imp.Value.assetBundleName = null; ;
            }
            importerCacheDict.Clear();

            if (assetBundleTabData.CopyToStreamingAssets)
            {
                var buildPath = Utility.IO.Combine(EditorUtilities.ApplicationPath(), assetBundleTabData.OutputPath);
                if (Directory.Exists(buildPath))
                {
                    if (!AssetDatabase.IsValidFolder(streamingPath))
                    {
                        AssetDatabase.CreateFolder("Assets", "StreamingAssets");
                    }
                    var streamingAssetPath = Utility.IO.Combine(Application.dataPath, "StreamingAssets");
                    Utility.IO.DirectoryCopy(buildPath, streamingAssetPath);
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }
        void OperateManifest()
        {
            var buildPath = GetBuildPath();
            var url = Utility.Unity.CombinePath(buildPath, assetBundleTabData.BuildTarget.ToString());
            EditorUtilities.IO.DownloadAssetBundleAsync(url, (percent) =>
            {
                var per = percent * 100;
                EditorUtility.DisplayProgressBar("LoadManifest", $"当前进度 ：{per} %", percent);
            }, (mainBundle) =>
            {
                EditorUtility.ClearProgressBar();
                var manifest = mainBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                var abNames = manifest.GetAllAssetBundles();
                Utility.Assert.Traverse(abNames, (ab) =>
                {
                    var hash = manifest.GetAssetBundleHash(ab);
                    foreach (var adm in abBuildInfo.AssetDataMaps)
                    {
                        if (adm.Value.ABName == ab)
                        {
                            adm.Value.ABHash = hash.ToString();
                        }
                    }
                });
                SetManifestInfo(abNames);
                switch (assetBundleTabData.NameHashType)
                {
                    case AssetBundleHashType.AppendHash:
                        {
                            var m_buildPath = GetBuildPath();
                            Utility.IO.TraverseFolderFilePath(m_buildPath, (path) =>
                            {
                                var fileName = Path.GetFileName(path);
                                var fileDir = Path.GetDirectoryName(path);
                                EditorUtilities.Debug.LogInfo(fileDir);
                                if (!fileName.Contains(".manifest"))
                                {
                                    if (buildInfoCache.TryGetValue(fileName, out var abKey))
                                    {
                                        abBuildInfo.AssetDataMaps.TryGetValue(abKey, out var ad);
                                        var newFileName = fileName + "_" + ad.ABHash;
                                        Utility.IO.RenameFile(path, newFileName);
                                    }
                                }
                            });
                        }
                        break;
                    case AssetBundleHashType.HashInstead:
                        {
                            var m_buildPath = GetBuildPath();
                            Utility.IO.TraverseFolderFilePath(m_buildPath, (path) =>
                            {
                                var fileName = Path.GetFileName(path);
                                var fileDir = Path.GetDirectoryName(path);
                                if (!fileName.Contains(".manifest"))
                                {
                                    if (buildInfoCache.TryGetValue(fileName, out var abKey))
                                    {
                                        abBuildInfo.AssetDataMaps.TryGetValue(abKey, out var ad);
                                        var newFileName = ad.ABHash;
                                        Utility.IO.RenameFile(path, newFileName);
                                    }
                                }
                            });
                        }
                        break;
                }
                if (assetBundleTabData.WithoutManifest)
                {
                    var abBuildPath = GetBuildPath();
                    Utility.IO.TraverseFolderFilePath(abBuildPath, (path) =>
                    {
                        var ext = Path.GetExtension(path);
                        if (ext == ".manifest")
                        {
                            File.Delete(path);
                        }
                    });
                }
                WriteBuildInfo();
                ResetBuildInfo();
            });
        }
        void SetManifestInfo(string[] abNames)
        {
            var urls = new string[abNames.Length];
            var length = urls.Length;
            for (int i = 0; i < length; i++)
            {
                urls[i]= Utility.Unity.CombinePath(GetBuildPath(),abNames[i]);
            }
            EditorUtilities.IO.DownloadAssetBundlesAsync(urls, percent =>
            {
                EditorUtility.DisplayProgressBar("AssetBundleLoading", $"{percent * 100} %", percent);
            }, null, bundles => 
            {
                EditorUtility.ClearProgressBar();
                foreach (var bundle in bundles)
                {
                    EditorUtilities.Debug.LogInfo(bundle.name, MessageColor.YELLOW);
                    var allAssets = bundle.GetAllAssetNames();
                    for (int i = 0; i < allAssets.Length; i++)
                    {
                        EditorUtilities.Debug.LogInfo(allAssets[i], MessageColor.GREEN);
                    }
                }
            });

        }
        void WriteBuildInfo()
        {
            var json = EditorUtilities.Json.ToJson(abBuildInfo, true);
            var fullPath = Utility.IO.Combine(GetBuildPath(), quarkABBuildInfo);
            Utility.IO.WriteTextFile(fullPath, json, false);
            abBuildInfo.Dispose();
            buildInfoCache.Clear();
        }
        void SetAssetBundleName(string path)
        {
            string abName = string.Empty;
            abName = Utility.Text.Replace(path, new string[] { "\\", "/", ".", " " }, "_").ToLower();
            AssetImporter importer = null;
            if (!importerCacheDict.TryGetValue(path, out importer))
            {
                importer = AssetImporter.GetAtPath(path);
                importerCacheDict[path] = importer;
                importer.assetBundleName = abName;
                if (importer == null)
                    EditorUtilities.Debug.LogError("AssetImporter is empty : " + path);
                else
                if (!abBuildInfo.AssetDataMaps.TryGetValue(importer.assetPath, out var assetData))
                {
                    assetData = new QuarkAssetABBuildInfo.AssetData()
                    {
                        DependList = AssetDatabase.GetAssetBundleDependencies(abName, true).ToList(),
                        Id = abBuildInfo.AssetDataMaps.Count,
                        ABName = abName,
                    };
                    abBuildInfo.AssetDataMaps[importer.assetPath] = assetData;
                }
            }
        }
    }
}
