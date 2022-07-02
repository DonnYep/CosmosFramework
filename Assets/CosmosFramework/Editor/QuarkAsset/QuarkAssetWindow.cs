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
        QuarkAssetDatabaseTab assetDatabaseTab;
        QuarkAssetBundleTab assetBundleTab;
        Vector2 scrollPosition;
        internal const string QuarkAssetWindowDataName = "QuarkAsset_WindowData.json";

        QuarkAssetWindowData windowData;
        QuarkAssetDataset latestDataset;
        /// <summary>
        /// dataset是否为空处理标记；
        /// </summary>
        bool datasetAssigned = false;
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
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            if (assetDatabaseTab == null)
                assetDatabaseTab = new QuarkAssetDatabaseTab();
            if (assetBundleTab == null)
                assetBundleTab = new QuarkAssetBundleTab();
            datasetAssigned = false;
            QuarkEditorDataProxy.QuarkAssetDataset = null;
            GetWindowData();
            assetDatabaseTab.OnEnable();
            assetBundleTab.OnEnable();
            assetBundleTab.SetAssetDatabaseTab(assetDatabaseTab);
        }

        void OnDisable()
        {
            datasetAssigned = false;
            SaveWindowData();
            assetDatabaseTab.OnDisable();
            assetBundleTab.OnDisable();
            QuarkEditorDataProxy.QuarkAssetDataset = null;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        void OnGUI()
        {
            selectedBar = GUILayout.Toolbar(selectedBar, barArray);
            GUILayout.Space(16);
            latestDataset = (QuarkAssetDataset)EditorGUILayout.ObjectField("QuarkAssetDataset", latestDataset, typeof(QuarkAssetDataset), false);
            if (QuarkEditorDataProxy.QuarkAssetDataset != latestDataset)
            {
                QuarkEditorDataProxy.QuarkAssetDataset = latestDataset;
                if (QuarkEditorDataProxy.QuarkAssetDataset != null && !datasetAssigned)
                {
                    assetDatabaseTab.OnDatasetAssign(latestDataset);
                    assetBundleTab.OnDatasetAssign();
                    datasetAssigned = true;
                }
                else
                {
                    assetDatabaseTab.OnDatasetUnassign();
                    assetBundleTab.OnDatasetUnassign();
                    datasetAssigned = false;
                }
            }
            else
            {
                if (QuarkEditorDataProxy.QuarkAssetDataset == null)
                {
                    if (datasetAssigned)
                    {
                        assetDatabaseTab.OnDatasetUnassign();
                        assetBundleTab.OnDatasetUnassign();
                        datasetAssigned = false;
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
        void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            datasetAssigned = false;
            QuarkEditorDataProxy.QuarkAssetDataset = null;
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
            if (QuarkEditorDataProxy.QuarkAssetDataset != null)
            {
                windowData.QuarkDatasetPath = AssetDatabase.GetAssetPath(QuarkEditorDataProxy.QuarkAssetDataset);
                EditorUtility.SetDirty(QuarkEditorDataProxy.QuarkAssetDataset);
                AssetDatabase.Refresh();
            }
            EditorUtil.SaveData(QuarkAssetWindowDataName, windowData);
        }
        [InitializeOnLoadMethod]
        static void InitData()
        {
            FilterLength = Application.dataPath.Length - 6;
        }
    }
}