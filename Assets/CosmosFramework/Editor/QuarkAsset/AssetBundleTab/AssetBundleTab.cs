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

        public void Clear()
        {
        }
        public void OnDisable()
        {
            CosmosEditorUtility.SaveData(AssetBundleTabDataFileName, assetBundleTabData);
        }
        public void OnEnable()
        {
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
            assetBundleTabData.AppendHash = EditorGUILayout.ToggleLeft("AppendHash", assetBundleTabData.AppendHash);
            GUILayout.Space(16);

            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                if (GUILayout.Button("Build"))
                {
                    BuildAssetBundle();
                }
            });
        }
        void BrowseForFolder()
        {
            assetBundleTabData.UseDefaultPath= false;
            var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", assetBundleTabData.OutputPath, string.Empty);
            if (!string.IsNullOrEmpty(newPath))
            {
                var gamePath = System.IO.Path.GetFullPath(".");
                gamePath = gamePath.Replace("\\", "/");
                if (newPath.StartsWith(gamePath) && newPath.Length > gamePath.Length)
                    newPath = newPath.Remove(0, gamePath.Length + 1);
                assetBundleTabData.OutputPath= newPath;
            }
        }
        void ResetPath()
        {
            assetBundleTabData.UseDefaultPath = true;
            assetBundleTabData.OutputPath = "AssetBundles/";
            assetBundleTabData.OutputPath += assetBundleTabData.BuildTarget.ToString();
        }
        void BuildAssetBundle()
        {
            var path = Path.GetFullPath(".");
            var fullPath = Utility.IO.Combine(path, assetBundleTabData.OutputPath);
            fullPath= fullPath.Replace("\\", "/");
            CosmosEditorUtility.LogInfo(fullPath);
            return;
            try
            {
                if (Directory.Exists(assetBundleTabData.OutputPath))
                    Directory.Delete(assetBundleTabData.OutputPath, true);
                if (assetBundleTabData.CopyToStreamingAssets)
                    if (Directory.Exists(streamingPath))
                        Directory.Delete(streamingPath, true);
            }
            catch (System.Exception e)
            {
                CosmosEditorUtility.LogError(e);
            }
            if (!Directory.Exists(assetBundleTabData.OutputPath))
                Directory.CreateDirectory(assetBundleTabData.OutputPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}
