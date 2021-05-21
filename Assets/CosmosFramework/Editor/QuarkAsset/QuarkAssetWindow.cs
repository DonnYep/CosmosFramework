using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Cosmos.QuarkAsset;
namespace Cosmos.CosmosEditor
{
    public class QuarkAssetWindow : EditorWindow
    {
        enum AssetInfoBar : int
        {
            AssetDatabaseMode = 0,
            AssetBundleMode = 1
        }
        int selectedBar = 0;
        string[] barArray = new string[] { "AssetDatabaseBuilder", "AssetBundleBuilder" };
        public static int FilterLength { get; private set; }
        static AssetDatabaseTab assetDatabaseTab = new AssetDatabaseTab();
        static AssetBundleBuildTab assetBundleTab = new AssetBundleBuildTab();
        internal static WindowTabData WindowTabData { get; private set; }
        internal const string QuarkAssetWindowTabDataFileName = "QuarkAssetWindowTabData.json";
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
            FilterLength = Application.dataPath.Length - 6;
        }
        private void OnEnable()
        {
            try
            {
                WindowTabData = CosmosEditorUtility.GetData<WindowTabData>(QuarkAssetWindowTabDataFileName);
            }
            catch
            {
                WindowTabData = new WindowTabData();
            }
            if (WindowTabData.IncludeDirectories == null)
                WindowTabData.IncludeDirectories = new List<string>();
            assetDatabaseTab.OnEnable();
            assetBundleTab.OnEnable();
        }
        private void OnDisable()
        {
            assetDatabaseTab.OnDisable();
            assetBundleTab.OnDisable();
            CosmosEditorUtility.SaveData(QuarkAssetWindowTabDataFileName, WindowTabData);
        }
        private void OnGUI()
        {
            selectedBar = GUILayout.Toolbar(selectedBar, barArray);
            GUILayout.Space(16);
            var bar = (AssetInfoBar)selectedBar;
            switch (bar)
            {
                case AssetInfoBar.AssetDatabaseMode:
                    assetDatabaseTab.OnGUI();
                    break;
                case AssetInfoBar.AssetBundleMode:
                    assetBundleTab.OnGUI();
                    break;
            }
        }
    }
}