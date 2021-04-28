using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Cosmos.Test;

namespace Cosmos.CosmosEditor
{
    public class AssetInfoBuilderWindow : EditorWindow
    {
        enum AssetInfoBar : int
        {
            BuildAsset = 0,
            PlatfromBuild = 1
        }
        static string[] projectFiles = new string[] { ".jpg", ".png", ".prefab", ".wav", ".mp3", ".shader", ".xml", ".fbx", ".mat", ".dll", ".controller", ".overrideController", ".txt" };
        int selectedBar = 0;
        string[] barArray = new string[] { "BuildAsset", "PlatfromBuild" };
        AssetInfoData assetInfoData = new AssetInfoData();
        public const string CFGAssetInfoData = "AssetInfoData.json";
        static int filterLength;
        public AssetInfoBuilderWindow()
        {
            this.titleContent = new GUIContent("AssetInfoBuilder");
        }
        [MenuItem("Cosmos/AssetInfoBuilder")]
        public static void OpenWindow()
        {
            var window = GetWindow<AssetInfoBuilderWindow>();
            ((EditorWindow)window).maxSize = CosmosEditorUtility.CosmosMaxWinSize;
            ((EditorWindow)window).minSize = CosmosEditorUtility.CosmosDevWinSize;
        }
        [InitializeOnLoadMethod]
        public static void LoadData()
        {
            filterLength = Application.dataPath.Length - 6;
        }
        private void OnGUI()
        {
            selectedBar = GUILayout.Toolbar(selectedBar, barArray, GUILayout.Height(24));
            GUILayout.Space(16);
            var bar = (AssetInfoBar)selectedBar;
            switch (bar)
            {
                case AssetInfoBar.BuildAsset:
                    DrawBuildAsset();
                    break;
                case AssetInfoBar.PlatfromBuild:
                    DrawPlatformBuild();
                    break;
            }
        }
        void DrawBuildAsset()
        {
            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("BuildAssetInfo", GUILayout.Height(32)))
            {
                assetInfoData.Dispose();

                string path = UnityEngine.Application.dataPath;
                Utility.IO.TraverseFolderFile(path, (file) =>
                {
                    var length = projectFiles.Length;
                    for (int i = 0; i < length; i++)
                    {
                        if (projectFiles[i].Equals(file.Extension))
                        {
                            var assetPath = file.FullName.Remove(0, filterLength);
                            assetInfoData.AddAssetInfo(file.Name, file.Extension, assetPath);
                        }
                    }
                });
                CosmosEditorUtility.WriteEditorData(CFGAssetInfoData, assetInfoData);
                CosmosEditorUtility.LogInfo("AssetInfo Build done ");
            }
            if (GUILayout.Button("ClearBuildInfo", GUILayout.Height(32)))
            {
                CosmosEditorUtility.ClearEditorData(CFGAssetInfoData);
                CosmosEditorUtility.LogInfo("AssetInfo clear done ");
            }
            GUILayout.EndHorizontal();
        }
        void DrawPlatformBuild()
        {

        }
    }
}