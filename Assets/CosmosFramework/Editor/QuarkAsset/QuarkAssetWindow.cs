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
        string[] barArray = new string[] { "QuarkDatasetBuilder", "AssetBundleBuilder" };
        public static int FilterLength { get; private set; }
        static QuarkDatasetTab quarkDatasetTab = new QuarkDatasetTab();
        static QuarkAssetBundleTab quarkAssetBundleTab = new QuarkAssetBundleTab();
        internal static QuarkWindowTabData WindowTabData { get; private set; }
        internal const string QuarkAssetWindowTabDataFileName = "QuarkAssetWindowTabData.json";
        QuarkAssetDataset quarkAssetDataset;
        Vector2 m_ScrollPos;

        public QuarkAssetWindow()
        {
            this.titleContent = new GUIContent("QuarkAsset");
        }
        [MenuItem("Window/Cosmos/QuarkAsset")]
        public static void OpenWindow()
        {
            var window = GetWindow<QuarkAssetWindow>();
        }
        void OnEnable()
        {
            try
            {
                WindowTabData = EditorUtil.GetData<QuarkWindowTabData>(QuarkAssetWindowTabDataFileName);
            }
            catch
            {
                WindowTabData = new QuarkWindowTabData();
            }
            quarkDatasetTab.OnEnable();
            quarkAssetBundleTab.OnEnable();
            quarkAssetBundleTab.SetAssetDatasetTab(quarkDatasetTab);
            if (!string.IsNullOrEmpty(WindowTabData.QuarkAssetDatasetPath))
            {
                try
                {
                    quarkAssetDataset = AssetDatabase.LoadAssetAtPath<QuarkAssetDataset>(WindowTabData.QuarkAssetDatasetPath);
                }
                catch { }
            }
        }
        void OnDisable()
        {
            quarkDatasetTab.OnDisable();
            quarkAssetBundleTab.OnDisable();
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
        void OnGUI()
        {
            selectedBar = GUILayout.Toolbar(selectedBar, barArray);
            GUILayout.Space(16);
            quarkAssetDataset = (QuarkAssetDataset)EditorGUILayout.ObjectField("QuarkAssetDataset", quarkAssetDataset, typeof(QuarkAssetDataset), false);
            QuarkEditorDataProxy.QuarkAssetDataset = quarkAssetDataset;
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
                    quarkDatasetTab.OnGUI(position);
                    break;
                case AssetInfoBar.AssetBundleMode:
                    quarkAssetBundleTab.OnGUI(position);
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
            EditorUtil.Debug.LogInfo("QuarkAssetDataset is created");
            return dataset;
        }
        [InitializeOnLoadMethod]
        static void InitData()
        {
            FilterLength = Application.dataPath.Length - 6;
        }
    }
}