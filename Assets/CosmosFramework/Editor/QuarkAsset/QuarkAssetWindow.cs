using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Cosmos.Test;
using Cosmos.QuarkAsset;
namespace Cosmos.CosmosEditor
{
    public class QuarkAssetWindow : EditorWindow
    {
        enum AssetInfoBar : int
        {
            BuildAsset = 0,
            PlatfromBuild = 1
        }
        static string[] projectFiles = new string[]
        {
        ".3ds",".bmp",".blend",".eps",".exif",".gif",".icns",".ico",".jpeg",
        ".jpg",".ma",".max",".mb",".pcx",".png",".psd",".svg",".controller",
        ".wav",".txt",".prefab",".xml",".shadervariants",".shader",".anim",
        ".unity",".mat",".dll",".mask",".overrideController",".tif",".spriteatlas"
        };
        int selectedBar = 0;
        string[] barArray = new string[] { "BuildAsset", "PlatfromBuild" };
        static int filterLength;
         QuarkAssetData quarkAssetData=new QuarkAssetData();
        public QuarkAssetWindow()
        {
            this.titleContent = new GUIContent("QuarkAsset");
        }
        [MenuItem("Window/Cosmos/QuarkAsset")]
        public static void OpenWindow()
        {
            var window = GetWindow<QuarkAssetWindow>();
            ((EditorWindow)window).maxSize = CosmosEditorUtility.CosmosMaxWinSize;
            ((EditorWindow)window).minSize = CosmosEditorUtility.CosmosDevWinSize;
        }
        [InitializeOnLoadMethod]
        static void InitData()
        {
            filterLength = Application.dataPath.Length - 6;
            var jsonHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute,IJsonHelper>();
            Utility.Json.SetHelper(jsonHelper);
        }
        private void OnEnable()
        {
            quarkAssetData = QuarkUtility.LoadQuarkAssetData();
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
            GUILayout.BeginVertical();
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                EditorGUILayout.LabelField("QuarkAssetLoadMode", GUILayout.Width(128));
                quarkAssetData.QuarkAssetLoadMode = (QuarkAssetLoadMode)EditorGUILayout.EnumPopup(quarkAssetData.QuarkAssetLoadMode);
            });
            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                EditorGUILayout.LabelField("GenerateAssetPathCode", GUILayout.Width(128));
                quarkAssetData.GenerateAssetPathCode = EditorGUILayout.Toggle(quarkAssetData.GenerateAssetPathCode);
            });

            CosmosEditorUtility.DrawHorizontalContext(() =>
            {
                if (GUILayout.Button("Build", GUILayout.Height(32)))
                {
                    List<QuarkAssetObject> quarkAssetList = new List<QuarkAssetObject>();
                    quarkAssetList?.Clear();
                    string path = Application.dataPath;
                    Utility.IO.TraverseFolderFile(path, (file) =>
                    {
                        var length = projectFiles.Length;
                        for (int i = 0; i < length; i++)
                        {
                            if (projectFiles[i].Equals(file.Extension))
                            {
                                var assetPath = file.FullName.Remove(0, filterLength);
                                var assetName = file.Name.Replace(file.Extension, string.Empty);
                                var assetObj = new QuarkAssetObject()
                                {
                                    AssetExtension = file.Extension,
                                    AssetName = assetName,
                                    AssetPath = assetPath
                                };
                                quarkAssetList.Add(assetObj);
                            }
                        }
                    });
                    quarkAssetData.QuarkAssetObjectList = quarkAssetList;
                    QuarkUtility.SetAndSaveQuarkAsset(quarkAssetData);
                    CosmosEditorUtility.LogInfo("Quark asset data build done ");
                }
                if (GUILayout.Button("Clear", GUILayout.Height(32)))
                {
                    QuarkUtility.ClearQuarkAsset();
                    CosmosEditorUtility.LogInfo("Quark asset data clear done ");
                }
            });
            GUILayout.EndVertical();

        }
        void DrawPlatformBuild()
        {

        }
    }
}