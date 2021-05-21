using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Cosmos.QuarkAsset;
namespace Cosmos.CosmosEditor
{
    public class AssetBundleBuildTab
    {
        AssetBundleBuildTabData assetBundleTabData;
        const string AssetBundleTabDataFileName = "AssetBundleTabData.json";
        string streamingPath = "Assets/StreamingAssets";
        Dictionary<string, List<string>> packageAssetMap;
        Dictionary<string, AssetImporter> importerCacheDict = new Dictionary<string, AssetImporter>();

        public void Clear()
        {
        }
        public void OnDisable()
        {
            CosmosEditorUtility.SaveData(AssetBundleTabDataFileName, assetBundleTabData);
        }
        public void OnEnable()
        {
            packageAssetMap = new Dictionary<string, List<string>>();
            try
            {
                assetBundleTabData = CosmosEditorUtility.GetData<AssetBundleBuildTabData>(AssetBundleTabDataFileName);
            }
            catch
            {
                assetBundleTabData = new AssetBundleBuildTabData();
                CosmosEditorUtility.SaveData(AssetBundleTabDataFileName, assetBundleTabData);
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
            CosmosEditorUtility.DrawHorizontalContext(() =>
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
            CosmosEditorUtility.DrawVerticalContext(() =>
            {
                assetBundleTabData.ClearFolders = EditorGUILayout.ToggleLeft("ClearFolders", assetBundleTabData.ClearFolders);
                assetBundleTabData.CopyToStreamingAssets = EditorGUILayout.ToggleLeft("CopyToStreamingAssets", assetBundleTabData.CopyToStreamingAssets);
            });
            GUILayout.Space(16);
            assetBundleTabData.BuildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("CompressedFormat:", assetBundleTabData.BuildAssetBundleOptions);
            assetBundleTabData.EncryptionKey = EditorGUILayout.TextField("EncryptionKey", assetBundleTabData.EncryptionKey);
            assetBundleTabData.NameHashType = (AssetBundleHashType)EditorGUILayout.EnumPopup("NameHashType", assetBundleTabData.NameHashType);
            GUILayout.Space(16);

            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                if (GUILayout.Button("Build"))
                {
                    BuildAssetBundle();
                }
                if (GUILayout.Button("Reset"))
                {
                    assetBundleTabData = new AssetBundleBuildTabData();
                    CosmosEditorUtility.SaveData(AssetBundleTabDataFileName, assetBundleTabData);
                }
            });
        }
        void BrowseForFolder()
        {
            assetBundleTabData.UseDefaultPath = false;
            var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", assetBundleTabData.OutputPath, string.Empty);
            if (!string.IsNullOrEmpty(newPath))
            {
                var gamePath = CosmosEditorUtility.ApplicationPath();
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
            var path = Utility.IO.Combine(CosmosEditorUtility.ApplicationPath(), assetBundleTabData.OutputPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        string GetBuildPath()
        {
            var path = Utility.IO.Combine(CosmosEditorUtility.ApplicationPath(), assetBundleTabData.OutputPath);
            return path;
        }
        void BuildAssetBundle()
        {
            SetBuildInfo();
            BuildPipeline.BuildAssetBundles(GetBuildFolder(), assetBundleTabData.BuildAssetBundleOptions, assetBundleTabData.BuildTarget);
            ResetBuildInfo();
        }
        void SetBuildInfo()
        {
            CosmosEditorUtility.LogInfo("开始打包");
            if (assetBundleTabData.ClearFolders)
            {
                var path = Utility.IO.Combine(CosmosEditorUtility.ApplicationPath(), assetBundleTabData.OutputPath);
                var streamPath= Utility.IO.Combine(CosmosEditorUtility.ApplicationPath(), streamingPath);
                if (Directory.Exists(path))
                {
                    Utility.IO.DeleteFolder(path);
                }
                try
                {
                    Utility.IO.DeleteFolder(streamPath);
                }
                catch {}
            }
            if (QuarkAssetWindow.WindowTabData.UnderAssetsDirectory)
            {
                CosmosEditorUtility.StartCoroutine(EnumBuild());
            }
            else
            {
                var includeDirectroies = QuarkAssetWindow.WindowTabData.IncludeDirectories;
                foreach (var dir in includeDirectroies)
                {
                    SetAssetBundleName(dir);
                }
            }
        }
        IEnumerator EnumBuild()
        {
            CosmosEditorUtility.TraverseAllFolderFile((file) =>
            {
                if (!(file is MonoScript))
                {
                    var dir = AssetDatabase.GetAssetPath(file);
                    SetAssetBundleName(dir);
                }
            });
            yield return null;
        }
        void ResetBuildInfo()
        {
            CosmosEditorUtility.LogInfo("打包完成");
            foreach (var imp in importerCacheDict)
            {
                imp.Value.assetBundleName = null; ;
            }
            importerCacheDict.Clear();

            if (assetBundleTabData.CopyToStreamingAssets)
            {
                var buildPath =Utility.IO.Combine( CosmosEditorUtility.ApplicationPath(),assetBundleTabData.OutputPath);
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
        void SetAssetBundleName(string path)
        {
            string name = string.Empty;
            if (AssetDatabase.IsValidFolder(path))
                name = path;
            else
            {
                var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                name = obj.name;
            }
            switch (assetBundleTabData.NameHashType)
            {
                case AssetBundleHashType.DefaultName:
                    {
                        AssetImporter importer = GetAssetImporter(path);
                        importer.assetBundleName = name;
                    }
                    break;
                case AssetBundleHashType.AppendHash:
                    {
                        AssetImporter importer = GetAssetImporter(path);
                        var hash = AssetDatabase.AssetPathToGUID(path);
                        importer.assetBundleName = Utility.Text.Combine(name, "_", hash);
                    }
                    break;
                case AssetBundleHashType.HashInstead:
                    {
                        var hash = AssetDatabase.AssetPathToGUID(path);
                        AssetImporter importer = GetAssetImporter(path);
                        importer.assetBundleName = hash;
                    }
                    break;
            }
        }
        AssetImporter GetAssetImporter(string path)
        {
            AssetImporter importer = null;
            if (!importerCacheDict.TryGetValue(path, out importer))
            {
                importer = AssetImporter.GetAtPath(path);
                importerCacheDict[path] = importer;
                if (importer == null)
                    CosmosEditorUtility.LogError("AssetImporter is empty : " + path);
            }
            return importer;
        }
    }
}
