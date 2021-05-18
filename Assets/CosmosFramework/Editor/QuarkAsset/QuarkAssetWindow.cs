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
        FastDevelopTab fastDevelopTab = new FastDevelopTab();
        AssetBundleTab assetBundleTab = new AssetBundleTab();

        //GUIContent fastDevelopContent = new GUIContent("FastDevelopMode");
        //GUIContent assetBundleContent = new GUIContent("AssetBundleMode");

        //bool fastDevelopMode;
        //bool assetBundleMode;

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
            fastDevelopTab.OnEnable();
            assetBundleTab.OnEnable();
        }
        private void OnDisable()
        {
            fastDevelopTab.OnDisable();
            assetBundleTab.OnDisable();
        }
        private void OnGUI()
        {
            //var style = EditorStyles.toolbarButton;
                selectedBar = GUILayout.Toolbar(selectedBar, barArray);
            //EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));
            //{
            //    fastDevelopMode = GUILayout.Toggle(fastDevelopMode, fastDevelopContent, style, GUILayout.Width(style.CalcSize(fastDevelopContent).x));
            //    assetBundleMode = GUILayout.Toggle(assetBundleMode, assetBundleContent, style, GUILayout.Width(style.CalcSize(assetBundleContent).x));
            //}
            //EditorGUILayout.EndHorizontal();
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