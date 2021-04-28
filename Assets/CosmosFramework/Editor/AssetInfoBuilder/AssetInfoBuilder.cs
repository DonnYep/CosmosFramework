using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
namespace Cosmos.CosmosEditor
{
    public class AssetInfoBuilderWiindow : EditorWindow
    {
        enum AssetInfoBar : int
        {
            BuildAsset = 0,
            PlatfromBuild = 1
        }
        static string[] projectFiles = new string[] { ".jpg", "png", "prefab", ".wav", ".mp3", "shader", "xml", ".fbx", ".mat", "dll", ".controller", ".overrideController", ".txt" };
        int selectedBar = 0;
        string[] barArray = new string[] { "BuildAsset", "PlatfromBuild" };
        AssetInfoData assetInfoData=new AssetInfoData();
        public const string CFGAssetInfoData = "AssetInfoData.json";
      static  int filterLength;
        public AssetInfoBuilderWiindow()
        {
            this.titleContent = new GUIContent("AssetInfoBuilder");
        }
        [MenuItem("Cosmos/AssetInfoBuilder")]
        public static void OpenWindow()
        {
            var window = GetWindow<AssetInfoBuilderWiindow>();
            ((EditorWindow)window).maxSize = CosmosEditorUtility.CosmosMaxWinSize;
            ((EditorWindow)window).minSize = CosmosEditorUtility.CosmosDevWinSize;
            filterLength= Application.dataPath.Length + 1;
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
                string path = UnityEngine.Application.dataPath;
                CosmosEditorUtility.LogInfo(path);
                Utility.IO.TraverseFolderFile(path, (file) => 
                {
                    var length = projectFiles.Length;
                    for (int i = 0; i < length; i++)
                    {
                        if (projectFiles[i].Equals(file.Extension))
                        {
                            var assetPath= file.FullName.Remove(0, filterLength);
                            assetInfoData.AddAssetInfo(file.Name,file.Extension, assetPath);
                        }
                    }
                });
                CosmosEditorUtility.WriteEditorConfig(CFGAssetInfoData,assetInfoData);
                assetInfoData.Dispose();
            }
            if (GUILayout.Button("CheckBuildInfo", GUILayout.Height(32)))
            {
                var data= CosmosEditorUtility.ReadEditorConfig<AssetInfoData>(CFGAssetInfoData);
                foreach (var info in data.AssetInfos)
                {
                    CosmosEditorUtility.LogInfo(info.AssetPath);
                }
            }
            if (GUILayout.Button("ClearBuildInfo", GUILayout.Height(32)))
            {
                CosmosEditorUtility.ClearEditorConfig(CFGAssetInfoData);
            }
            GUILayout.EndHorizontal();
        }
        void DrawPlatformBuild()
        {

        }
    }
}