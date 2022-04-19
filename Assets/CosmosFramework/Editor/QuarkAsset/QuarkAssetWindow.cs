using UnityEngine;
using UnityEditor;
using Quark.Asset;
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
        static QuarkAssetDatabaseTab quarkAssetDatabaseTab = new QuarkAssetDatabaseTab();
        static QuarkAssetBundleTab quarkAssetBundleTab = new QuarkAssetBundleTab();
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
            quarkAssetDatabaseTab.OnEnable();
            quarkAssetBundleTab.OnEnable();
            quarkAssetBundleTab.SetAssetDatabaseTab(quarkAssetDatabaseTab);
        }
        void OnDisable()
        {
            quarkAssetDatabaseTab.OnDisable();
            quarkAssetBundleTab.OnDisable();
        }
        void OnGUI()
        {
            selectedBar = GUILayout.Toolbar(selectedBar, barArray);
            GUILayout.Space(16);
            QuarkEditorDataProxy.QuarkAssetDataset = (QuarkAssetDataset)EditorGUILayout.ObjectField("QuarkAssetDataset", QuarkEditorDataProxy.QuarkAssetDataset, typeof(QuarkAssetDataset), false);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("CreateDataset", GUILayout.MaxWidth(128f)))
            {
                QuarkEditorDataProxy.QuarkAssetDataset = CreateQuarkAssetDataset();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(16);
            var bar = (AssetInfoBar)selectedBar;
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            switch (bar)
            {
                case AssetInfoBar.AssetDatabaseMode:
                    quarkAssetDatabaseTab.OnGUI(position);
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
            QuarkUtility.LogInfo("QuarkAssetDataset is created");
            return dataset;
        }
        [InitializeOnLoadMethod]
        static void InitData()
        {
            FilterLength = Application.dataPath.Length - 6;
        }
    }
}