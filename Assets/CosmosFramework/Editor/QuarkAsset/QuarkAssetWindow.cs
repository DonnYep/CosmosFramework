using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Cosmos.Quark;
using System;

namespace Cosmos.CosmosEditor
{
    public class QuarkAssetWindow : EditorWindow
    {
        static QuarkAssetWindow instance;
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
        QuarkAssetDataset quarkAssetDataset;
        Vector2 m_ScrollPos;

        bool isExistedInvoked;
        bool isEmptyInvoked;
        int latestDirCount = 0;

        public QuarkAssetWindow()
        {
            this.titleContent = new GUIContent("QuarkAsset");
        }
        public void OnLoad()
        {
            if (quarkAssetDataset != null)
            {
                OnAsginQuarkDataset();
            }
            Debug.Log("OnLoad");
        }
        [MenuItem("Window/Cosmos/QuarkAsset")]
        public static void OpenWindow()
        {
            var window = GetWindow<QuarkAssetWindow>();
        }
        [InitializeOnLoadMethod]
        static void InitData()
        {
            FilterLength = Application.dataPath.Length - 6;
        }
        private void Awake()
        {
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
            assetBundleTab.SetAssetDatabaseTab(assetDatabaseTab);
            if (!string.IsNullOrEmpty(WindowTabData.QuarkAssetDatasetPath))
            {
                try
                {
                    quarkAssetDataset = AssetDatabase.LoadAssetAtPath<QuarkAssetDataset>(WindowTabData.QuarkAssetDatasetPath);
                }
                catch { }
            }
        }
        private void OnDisable()
        {
            assetDatabaseTab.OnDisable();
            assetBundleTab.OnDisable();
            if (quarkAssetDataset != null)
            {
                try
                {
                    var path = AssetDatabase.GetAssetPath(quarkAssetDataset);
                    WindowTabData.QuarkAssetDatasetPath = path;
                }
                catch { }
            }
            else
            {
                WindowTabData.QuarkAssetDatasetPath = string.Empty;
            }
            EditorUtil.SaveData(QuarkAssetWindowTabDataFileName, WindowTabData);
            if (quarkAssetDataset != null)
                EditorUtility.SetDirty(quarkAssetDataset);
        }
        private void OnGUI()
        {
            selectedBar = GUILayout.Toolbar(selectedBar, barArray);
            GUILayout.Space(16);
            quarkAssetDataset = (QuarkAssetDataset)EditorGUILayout.ObjectField("QuarkAssetDataset", quarkAssetDataset, typeof(QuarkAssetDataset), false);
            if (quarkAssetDataset != null)
            {
                QuarkEditorDataProxy.CanRender = true;
                if (!isExistedInvoked)
                {
                    OnAsginQuarkDataset();
                    isExistedInvoked = true;
                    isEmptyInvoked = false;
                }
                if (latestDirCount != quarkAssetDataset.IncludeDirectories.Count)
                {
                    OnDirChanged();
                    latestDirCount = quarkAssetDataset.IncludeDirectories.Count;
                }
            }
            else
            {
                QuarkEditorDataProxy.CanRender = false;
                if (!isEmptyInvoked)
                {
                    OnClearQuarkDataset();
                    isEmptyInvoked = true;
                    isExistedInvoked = false;
                    latestDirCount = 0;
                    OnDirChanged();
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("CreateDataset", GUILayout.MaxWidth(128f)))
            {
                quarkAssetDataset = CreateQuarkAssetDataset();
            }
            GUILayout.EndHorizontal();
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
        QuarkAssetDataset CreateQuarkAssetDataset()
        {
            var so = ScriptableObject.CreateInstance<QuarkAssetDataset>();
            so.hideFlags = HideFlags.NotEditable;
            AssetDatabase.CreateAsset(so, "Assets/New QuarkAssetDataset.asset");
            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            var dataset = AssetDatabase.LoadAssetAtPath<QuarkAssetDataset>("Assets/New QuarkAssetDataset.asset");
            dataset.Init();
            EditorUtil.Debug.LogInfo("QuarkAssetDataset is created");
            return dataset;
        }
        void OnAsginQuarkDataset()
        {
            assetDatabaseTab.SetQuarkAssetDataset(quarkAssetDataset);
            assetBundleTab.SetQuarkAssetDataset(quarkAssetDataset);
            EditorUtil.Debug.LogInfo("OnAsginQuarkDataset");
        }
        void OnClearQuarkDataset()
        {
            assetDatabaseTab.Clear();
            assetBundleTab.Clear();
            EditorUtil.Debug.LogInfo("OnClearQuarkDataset");
        }
        void OnDirChanged()
        {
            CheckAssetDatasetDirPath();
            EditorUtil.Debug.LogInfo("OnDirChanged", MessageColor.BLUE);
        }
        void CheckAssetDatasetDirPath()
        {
            if (quarkAssetDataset == null)
                return;
            var dirs = quarkAssetDataset.IncludeDirectories.ToArray();
            if (dirs == null || dirs.Length <= 0)
                return;
            var length = dirs.Length;
            string[] newDir = new string[length];
            for (int i = 0; i < length; i++)
            {
                var dirPath = dirs[i];
                var path = Path.Combine(Directory.GetCurrentDirectory(), dirPath);
                var hash = AssetDatabase.AssetPathToGUID(dirPath);
                //var hashToDir= AssetDatabase.GUIDToAssetPath(hash);

                //if (!Directory.Exists(path)&& !File.Exists(path))
                //{
                //    quarkAssetDataset.IncludeDirectories.Remove(dirPath);
                //}
            }
        }
    }
}