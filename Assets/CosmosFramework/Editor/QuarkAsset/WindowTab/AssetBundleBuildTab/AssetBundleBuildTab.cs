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
        Dictionary<string, AssetImporter> importerCacheDict = new Dictionary<string, AssetImporter>();
        QuarkAssetABBuildInfo abBuildInfo = new QuarkAssetABBuildInfo();
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
                    //var abName = "Assets_ABTest_Prefab_Lab_prefab";
                    //var strs = AssetDatabase.GetAssetBundleDependencies(abName.ToLower(), true);
                    //Utility.Assert.Traverse(strs, str => EditorUtilities.Debug.LogInfo(str));
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
            RemoveUselessFile();
            WriteBuildInfo();
            ResetBuildInfo();
        }
        void SetBuildInfo()
        {
            EditorUtilities.Debug.LogInfo("开始打包");
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
                var includeDirectroies = QuarkAssetWindow.WindowTabData.IncludeDirectories;
                foreach (var dir in includeDirectroies)
                {
                    SetAssetBundleName(dir);
                }
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                foreach (var map in abBuildInfo.AssetDataMaps)
                {
                    map.Value.DependList = AssetDatabase.GetAssetBundleDependencies(map.Value.ABName, true).ToList();
                }
            }
        }
        IEnumerator EnumBuild()
        {
            EditorUtilities.TraverseAllFolderFile((file) =>
            {
                if (!(file is MonoScript))
                {
                    var dir = AssetDatabase.GetAssetPath(file);
                    SetAssetBundleName(dir);
                }
            });
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            foreach (var map in abBuildInfo.AssetDataMaps)
            {
                map.Value.DependList = AssetDatabase.GetAssetBundleDependencies(map.Value.ABName, true).ToList();
            }
            yield return null;
        }
        void ResetBuildInfo()
        {
            EditorUtilities.Debug.LogInfo("打包完成");
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
        void RemoveUselessFile()
        {
            var buildPath = GetBuildPath();
            Utility.IO.TraverseFolderFilePath(buildPath, (path) =>
            {
                var ext = Path.GetExtension(path);
                if (ext == ".manifest")
                {
                    File.Delete(path);
                }
            });
        }
        void WriteBuildInfo()
        {
            var json = EditorUtilities.Json.ToJson(abBuildInfo,true);
            var fullPath = Utility.IO.Combine(GetBuildPath(), quarkABBuildInfo);
            Utility.IO.WriteTextFile(fullPath, json, false);
            abBuildInfo.Dispose();
        }
        void SetAssetBundleName(string path)
        {
            string name = string.Empty;
            name = Utility.Text.Replace(path, new string[] { "\\", "/", "." }, "_");
            string abName = string.Empty;
            switch (assetBundleTabData.NameHashType)
            {
                case AssetBundleHashType.DefaultName:
                    {
                        abName = name.ToLower();
                    }
                    break;
                case AssetBundleHashType.AppendHash:
                    {
                        var hash = AssetDatabase.AssetPathToGUID(path);
                        abName = Utility.Text.Combine(name.ToLower(), "_", hash);
                    }
                    break;
                case AssetBundleHashType.HashInstead:
                    {
                        var hash = AssetDatabase.AssetPathToGUID(path);
                        abName = hash;
                    }
                    break;
            }
            SetAssetImporter(path, abName);
        }
        AssetImporter SetAssetImporter(string path, string abName)
        {
            AssetImporter importer = null;
            if (!importerCacheDict.TryGetValue(path, out importer))
            {
                importer = AssetImporter.GetAtPath(path);
                importerCacheDict[path] = importer;
                importer.assetBundleName = abName;
                if (importer == null)
                    EditorUtilities.Debug.LogError("AssetImporter is empty : " + path);
                if (!abBuildInfo.AssetDataMaps.TryGetValue(importer.assetPath, out var assetData))
                {
                    assetData = new QuarkAssetABBuildInfo.AssetData()
                    {
                        DependList = AssetDatabase.GetAssetBundleDependencies( abName,true).ToList(),
                        Hash = AssetDatabase.AssetPathToGUID(importer.assetPath),
                        Id = abBuildInfo.AssetDataMaps.Count,
                        ABName = abName
                    };
                    abBuildInfo.AssetDataMaps[importer.assetPath] = assetData;
                }
            }
            return importer;
        }
    }
}
