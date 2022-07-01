using UnityEngine;
using UnityEditor;
using Quark.Asset;
using Cosmos.Editor;

namespace Quark.Editor
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
        static QuarkAssetDatabaseTab assetDatabaseTab = new QuarkAssetDatabaseTab();
        static QuarkAssetBundleTab assetBundleTab = new QuarkAssetBundleTab();
        Vector2 scrollPosition;
        internal const string QuarkAssetWindowDataName = "QuarkAsset_WindowData.json";

        QuarkAssetWindowData windowData;
        QuarkAssetDataset latestDataset;
        /// <summary>
        /// dataset是否为空处理标记；
        /// </summary>
        bool emptyDatasetFlag = false;
        public QuarkAssetWindow()
        {
            this.titleContent = new GUIContent("QuarkAsset");
        }
        [MenuItem("Window/Cosmos/QuarkAsset", false, 100)]
        public static void OpenWindow()
        {
            var window = GetWindow<QuarkAssetWindow>();
        }
        void OnEnable()
        {
            GetWindowData();
            assetDatabaseTab.OnEnable();
            assetBundleTab.OnEnable();
            assetBundleTab.SetAssetDatabaseTab(assetDatabaseTab);
        }
        void OnDisable()
        {
            SaveWindowData();
            assetDatabaseTab.OnDisable();
            assetBundleTab.OnDisable();
        }
        void OnGUI()
        {
            selectedBar = GUILayout.Toolbar(selectedBar, barArray);
            GUILayout.Space(16);
            latestDataset = (QuarkAssetDataset)EditorGUILayout.ObjectField("QuarkAssetDataset", latestDataset, typeof(QuarkAssetDataset), false);
            if (QuarkEditorDataProxy.QuarkAssetDataset != latestDataset)
            {
                QuarkEditorDataProxy.QuarkAssetDataset = latestDataset;
                if (QuarkEditorDataProxy.QuarkAssetDataset != null)
                {
                    assetDatabaseTab.OnDatasetAssign(latestDataset);
                    assetBundleTab.OnDatasetAssign();
                    emptyDatasetFlag = false;
                }
                else
                {
                    assetDatabaseTab.OnDatasetUnassign();
                    assetBundleTab.OnDatasetUnassign();
                }
            }
            else
            {
                if (QuarkEditorDataProxy.QuarkAssetDataset != null)
                {
                    if (!emptyDatasetFlag)
                    {
                        assetDatabaseTab.OnDatasetUnassign();
                        assetBundleTab.OnDatasetUnassign();
                        emptyDatasetFlag = true;
                    }
                }
            }
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("CreateDataset", GUILayout.MaxWidth(128f)))
                {
                    latestDataset = CreateQuarkAssetDataset();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(16);
            var bar = (AssetInfoBar)selectedBar;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
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
            QuarkUtility.LogInfo("QuarkAssetDataset is created");
            return dataset;
        }
        void GetWindowData()
        {
            try
            {
                windowData = EditorUtil.GetData<QuarkAssetWindowData>(QuarkAssetWindowDataName);
            }
            catch
            {
                windowData = new QuarkAssetWindowData();
                EditorUtil.SaveData(QuarkAssetWindowDataName, windowData);
            }
            if (!string.IsNullOrEmpty(windowData.QuarkDatasetPath))
            {
                latestDataset = AssetDatabase.LoadAssetAtPath<QuarkAssetDataset>(windowData.QuarkDatasetPath);
            }
        }
        void SaveWindowData()
        {
            EditorUtil.SaveData(QuarkAssetWindowDataName, windowData);
        }
        [InitializeOnLoadMethod]
        static void InitData()
        {
            FilterLength = Application.dataPath.Length - 6;
        }
    }
}