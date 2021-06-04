using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Cosmos.Quark;
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
        Vector2 m_ScrollPos;

        public QuarkAssetWindow()
        {
            this.titleContent = new GUIContent("QuarkAsset");
        }
        [MenuItem("Window/Cosmos/QuarkAsset")]
        public static void OpenWindow()
        {
            var window = GetWindow<QuarkAssetWindow>();
            //((EditorWindow)window).maxSize = EditorUtil.CosmosMaxWinSize;
            //((EditorWindow)window).minSize = EditorUtil.CosmosDevWinSize;
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
                WindowTabData = EditorUtil.GetData<WindowTabData>(QuarkAssetWindowTabDataFileName);
            }
            catch
            {
                WindowTabData = new WindowTabData();
            }
            assetDatabaseTab.OnEnable();
            assetBundleTab.OnEnable();
        }
        private void OnDisable()
        {
            assetDatabaseTab.OnDisable();
            assetBundleTab.OnDisable();
            EditorUtil.SaveData(QuarkAssetWindowTabDataFileName, WindowTabData);
            EditorUtility.SetDirty(QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance);
        }
        private void OnGUI()
        {
            selectedBar = GUILayout.Toolbar(selectedBar, barArray);
            GUILayout.Space(16);
            var bar = (AssetInfoBar)selectedBar;
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            switch (bar)
            {
                case AssetInfoBar.AssetDatabaseMode:
                    assetDatabaseTab.OnGUI();
                    break;
                case AssetInfoBar.AssetBundleMode:
                    assetBundleTab.OnGUI();
                    break;
            }
            EditorGUILayout.EndScrollView();
        }
    }
}