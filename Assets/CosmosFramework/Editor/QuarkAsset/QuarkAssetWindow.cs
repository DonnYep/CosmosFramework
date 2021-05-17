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
            FastDevelopMode = 0,
            AssetBundleMode = 1
        }
        int selectedBar = 0;
        string[] barArray = new string[] { "FastDevelopMode", "AssetBundleMode" };
        public static int FilterLength { get; private set; }
        static QuarkAssetDataset QuarkAssetDataset { get { return QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance; } }
        /// <summary>
        /// Editor配置文件；
        /// </summary>
        static QuarkAssetConfigData quarkAssetConfigData;
        internal static QuarkAssetConfigData QuarkAssetConfigData { get { return quarkAssetConfigData; } }
        public const string QuarkAssetConfigDataFileName = "QuarkAssetConfigData.json";

        FastDevelopTab fastDevelopTab = new FastDevelopTab();
        AssetBundleTab assetBundleTab = new AssetBundleTab();
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
                quarkAssetConfigData = CosmosEditorUtility.GetData<QuarkAssetConfigData>(QuarkAssetConfigDataFileName);
            }
            catch
            {
                quarkAssetConfigData = new QuarkAssetConfigData();
                quarkAssetConfigData.IncludeDirectories = new List<string>();
            }
            fastDevelopTab.OnEnable();
            assetBundleTab.OnEnable();
        }
        private void OnGUI()
        {
            selectedBar = GUILayout.Toolbar(selectedBar, barArray, GUILayout.Height(24));
            GUILayout.Space(16);
                var bar = (AssetInfoBar)selectedBar;
                switch (bar)
                {
                    case AssetInfoBar.FastDevelopMode:
                        fastDevelopTab.OnGUI();
                        break;
                    case AssetInfoBar.AssetBundleMode:
                    assetBundleTab.OnGUI();
                        break;
                }
        }
    }
}