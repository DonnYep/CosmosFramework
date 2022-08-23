using UnityEngine;
using UnityEditor;
using Cosmos.Resource;

namespace Cosmos.Editor.Resource
{
    public class ResourceWindow : ModuleWindowBase
    {
        ResourceWindowData windowData;
        public ResourceWindowData ResourceWindowData { get { return windowData; } }
        readonly string ResourceWindowDataName = "ResourceEditor_WindowData.json";
        ResourceWindowTabBase assetDatabaseTab;
        ResourceWindowTabBase assetBundleTab;
        ResourceWindowTabBase assetDatasetTab;
        string[] tabArray = new string[] { "AssetDatabase", "AssetBundle", "AssetDataset" };
        ResourceDataset latestResourceDataset;
        /// <summary>
        /// dataset是否为空处理标记；
        /// </summary>
        bool isDatasetEmpty = false;
        Texture2D refreshIcon;
        public ResourceWindow()
        {
            this.titleContent = new GUIContent("ResourceWindow");
        }
        [MenuItem("Window/Cosmos/Module/Resource")]
        public static void OpenWindow()
        {
            var window = GetWindow<ResourceWindow>();
            window.minSize = EditorUtil.DevWinSize;
        }
        void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            if (assetBundleTab == null)
                assetBundleTab = new AssetBundleTab();
            if (assetDatabaseTab == null)
                assetDatabaseTab = new AssetDatabaseTab();
            if (assetDatasetTab == null)
                assetDatasetTab = new AssetDatasetTab();
            GetWindowData();
            if (!string.IsNullOrEmpty(windowData.ResourceDatasetPath))
            {
                latestResourceDataset = AssetDatabase.LoadAssetAtPath<ResourceDataset>(windowData.ResourceDatasetPath);
            }
            assetDatabaseTab.OnEnable();
            assetBundleTab.OnEnable();
            assetDatasetTab.OnEnable();
            ((AssetBundleTab)assetBundleTab).BuildDataset = ((AssetDatabaseTab)assetDatabaseTab).BuildDataset;
            refreshIcon = ResourceWindowUtility.GetAssetRefreshIcon();
        }
        void OnGUI()
        {
            DrawLabels();
        }
        void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            if (ResourceWindowDataProxy.ResourceDataset != null)
            {
                windowData.ResourceDatasetPath = AssetDatabase.GetAssetPath(ResourceWindowDataProxy.ResourceDataset);
                EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
            }
            EditorUtil.SaveData(ResourceWindowDataName, windowData);
            assetDatabaseTab.OnDisable();
            assetBundleTab.OnDisable();
            assetDatasetTab.OnDisable();
        }
        void DrawLabels()
        {
            EditorGUILayout.BeginVertical();
            windowData.SelectedTabIndex = GUILayout.Toolbar(windowData.SelectedTabIndex, tabArray);
            EditorGUILayout.BeginHorizontal();
            {
                latestResourceDataset = (ResourceDataset)EditorGUILayout.ObjectField("ResourceDataset", latestResourceDataset, typeof(ResourceDataset), false);
                if (GUILayout.Button(refreshIcon, GUILayout.MaxWidth(32)))
                {
                    if (latestResourceDataset == null)
                        return;
                    switch (windowData.SelectedTabIndex)
                    {
                        case 0:
                            assetDatabaseTab.OnDatasetRefresh();
                            break;
                        case 1:
                            assetBundleTab.OnDatasetRefresh();
                            break;
                        case 2:
                            assetDatasetTab.OnDatasetRefresh();
                            break;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            if (ResourceWindowDataProxy.ResourceDataset != latestResourceDataset)
            {
                ResourceWindowDataProxy.ResourceDataset = latestResourceDataset;
                if (ResourceWindowDataProxy.ResourceDataset != null)
                    AssignDataset();
                else
                    UnassignDataset();
            }
            else
            {
                if (ResourceWindowDataProxy.ResourceDataset == null)
                {
                    if (!isDatasetEmpty)
                        UnassignDataset();
                }
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create Dataset", GUILayout.MinWidth(128f)))
            {
                latestResourceDataset = CreateResourceDataset();
            }
            if (GUILayout.Button("Clear Dataset", GUILayout.MinWidth(128f)))
            {
                latestResourceDataset = null;
                windowData.ResourceDatasetPath = string.Empty;
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(12);
            switch (windowData.SelectedTabIndex)
            {
                case 0:
                    assetDatabaseTab.OnGUI(this.position);
                    break;
                case 1:
                    assetBundleTab.OnGUI(this.position);
                    break;
                case 2:
                    assetDatasetTab.OnGUI(this.position);
                    break;
            }

            EditorGUILayout.EndVertical();
        }
        void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            ResourceWindowDataProxy.ResourceDataset = null;
        }
        ResourceDataset CreateResourceDataset()
        {
            var so = ScriptableObject.CreateInstance<ResourceDataset>();
            so.ResourceAvailableExtenisonList.AddRange(ResourceEditorConstant.Extensions);
            so.hideFlags = HideFlags.NotEditable;
            AssetDatabase.CreateAsset(so, "Assets/New ResourceDataset.asset");
            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            var dataset = AssetDatabase.LoadAssetAtPath<ResourceDataset>("Assets/New ResourceDataset.asset");
            EditorUtil.Debug.LogInfo("ResourceDataset created successfully");
            return dataset;
        }
        void GetWindowData()
        {
            try
            {
                windowData = EditorUtil.GetData<ResourceWindowData>(ResourceWindowDataName);
            }
            catch
            {
                windowData = new ResourceWindowData();
                EditorUtil.SaveData(ResourceWindowDataName, windowData);
            }
        }
        void AssignDataset()
        {
            assetDatabaseTab.OnDatasetAssign();
            assetBundleTab.OnDatasetAssign();
            assetDatasetTab.OnDatasetAssign();
            isDatasetEmpty = false;
            windowData.ResourceDatasetPath = AssetDatabase.GetAssetPath(latestResourceDataset);
            EditorUtil.SaveData(ResourceWindowDataName, windowData);
        }
        void UnassignDataset()
        {
            assetDatabaseTab.OnDatasetUnassign();
            assetBundleTab.OnDatasetUnassign();
            assetDatasetTab.OnDatasetUnassign();
            isDatasetEmpty = true;
            windowData.ResourceDatasetPath = string.Empty;
            EditorUtil.SaveData(ResourceWindowDataName, windowData);
        }
        /// <summary>
        /// 预留
        /// </summary>
        void DrawHorizontalScope()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Bundle Data Source:");
                GUILayout.FlexibleSpace();
                var c = new GUIContent("cosmos");
                if (GUILayout.Button(c, EditorStyles.toolbarPopup))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int index = 0; index < 16; index++)
                    {

                    }
                    menu.ShowAsContext();
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}