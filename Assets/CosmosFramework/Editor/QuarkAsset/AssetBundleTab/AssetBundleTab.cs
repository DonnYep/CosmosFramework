using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Cosmos.QuarkAsset;
namespace Cosmos.CosmosEditor
{
    public class AssetBundleTab
    {
        AssetBundleTabData assetBundleTabData;
        const string AssetBundleTabDataFileName = "AssetBundleTabData.json";
        string streamingPath = "Assets/StreamingAssets";
        Dictionary<string, List<string>> packageAssetMap;
        Dictionary<string, AssetImporter> importerDict;
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
            importerDict = new Dictionary<string, AssetImporter>();
            try
            {
                assetBundleTabData = CosmosEditorUtility.GetData<AssetBundleTabData>(AssetBundleTabDataFileName);
            }
            catch
            {
                assetBundleTabData = new AssetBundleTabData();
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
            assetBundleTabData.NameHashType= (AssetBundleHashType)EditorGUILayout.EnumPopup("NameHashType", assetBundleTabData.NameHashType);
            GUILayout.Space(16);

            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                if (GUILayout.Button("Build"))
                {
                    BuildAssetBundle();
                }
                if (GUILayout.Button("Reset"))
                {
                    assetBundleTabData = new AssetBundleTabData();
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
            RenameAssetBundle();
        }
        void SetBuildInfo()
        {
            CosmosEditorUtility.LogInfo("开始打包");

            if (assetBundleTabData.ClearFolders)
            {
                var path = Utility.IO.Combine(CosmosEditorUtility.ApplicationPath(), assetBundleTabData.OutputPath);
                if (Directory.Exists(path))
                {

                    Utility.IO.DeleteFolder(path);
                }
            }

            var includeDirectroies = QuarkAssetWindow.IncludeDirectories;
            foreach (var dir in includeDirectroies)
            {
                string abName = string.Empty;
                if (AssetDatabase.IsValidFolder(dir))
                {
                    var strs = Utility.Text.StringSplit(dir, new string[] { "/" });
                    if (strs != null || strs.Length > 1)
                    {
                        abName = strs[strs.Length - 1];
                    }
                    CosmosEditorUtility.TraverseFolderFile(dir, (obj) =>
                    {
                        var path = AssetDatabase.GetAssetPath(obj);
                        AssetImporter importer = AssetImporter.GetAtPath(path);
                        importer.assetBundleName = abName;
                        importerDict.TryAdd(path, importer);
                    });
                }
                else
                {
                    AssetImporter importer = AssetImporter.GetAtPath(dir);
                    var dependencies = QuarkAssetEditorUtility.GetDependencises(dir);
                    var obj = AssetDatabase.LoadAssetAtPath<Object>(dir);
                    importer.assetBundleName = obj.name;
                    importerDict.TryAdd(dir, importer);
                    //var hash = AssetDatabase.AssetPathToGUID(dir);
                    //var depList = new List<string>();
                    //depList.AddRange(dependencies);
                    //packageAssetMap.Add(hash, depList);
                }
            }
       
        }
        void ResetBuildInfo()
        {
            CosmosEditorUtility.LogInfo("打包完成");
            foreach (var imp in importerDict)
            {
                imp.Value.assetBundleName = null; ;
            }
            importerDict.Clear();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        void RenameAssetBundle()
        {
            var path= GetBuildPath();
            switch (assetBundleTabData.NameHashType)
            {
                case AssetBundleHashType.None:
                    break;
                case AssetBundleHashType.AppendHash:
                    {

                    }
                    break;
                case AssetBundleHashType.HashInstead:
                    break;
                default:
                    break;
            }
        }
    }
}
