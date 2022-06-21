using UnityEngine;
using UnityEditor;
using System.IO;
using Cosmos.Resource;

namespace Cosmos.Editor.Resource
{
    public class ResourceWindow : ModuleWindowBase
    {
        ResourceWindowData windowData;
        public ResourceWindowData ResourceWindowData { get { return windowData; } }
        readonly string ResourceWindowDataName = "ResourceEditor_WindowData.json";
        AssetDatabaseTab assetDatabaseTab = new AssetDatabaseTab();
        AssetBundleTab assetBundleTab = new AssetBundleTab();
        string[] tabArray = new string[] { "AssetDatabase", "AssetBundle" };
        ResourceDataset latestResourceDataset;

        /// <summary>
        /// dataset是否为空处理标记；
        /// </summary>
        bool emptyDatasetFlag= false;
        public ResourceWindow()
        {
            this.titleContent = new GUIContent("ResourceWindow");
        }
        [MenuItem("Window/Cosmos/ModuleEditor/Resource")]
        public static void OpenIntegrateWindow()
        {
            var window = GetWindow<ResourceWindow>();
            window.minSize = EditorUtil.DevWinSize;
        }
        private void OnEnable()
        {
            GetWindowData();
            if (!string.IsNullOrEmpty(windowData.ResourceDatasetPath))
            {
                latestResourceDataset = AssetDatabase.LoadAssetAtPath<ResourceDataset>(windowData.ResourceDatasetPath);
            }
            assetDatabaseTab.OnEnable();
            assetBundleTab.OnEnable();
        }
        private void OnDisable()
        {
            if (ResourceEditorDataProxy.ResourceDataset != null)
            {
                windowData.ResourceDatasetPath = AssetDatabase.GetAssetPath(ResourceEditorDataProxy.ResourceDataset);
                EditorUtility.SetDirty(ResourceEditorDataProxy.ResourceDataset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            EditorUtil.SaveData(ResourceWindowDataName, windowData);
            assetDatabaseTab.OnDisable();
            assetBundleTab.OnDisable();
        }
        private void OnGUI()
        {
            DrawLables();
        }
        void DrawLables()
        {
            EditorGUILayout.BeginVertical();
            windowData.SelectedTabIndex = GUILayout.Toolbar(windowData.SelectedTabIndex, tabArray);
            latestResourceDataset = (ResourceDataset)EditorGUILayout.ObjectField("ResourceDataset", latestResourceDataset, typeof(ResourceDataset), false);
            if (ResourceEditorDataProxy.ResourceDataset != latestResourceDataset)
            {
                ResourceEditorDataProxy.ResourceDataset = latestResourceDataset;
                if (ResourceEditorDataProxy.ResourceDataset != null)
                {
                    assetDatabaseTab.OnDatasetAssign();
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
                if (ResourceEditorDataProxy.ResourceDataset == null)
                {
                    if (!emptyDatasetFlag)
                    {
                        assetDatabaseTab.OnDatasetUnassign();
                        assetBundleTab.OnDatasetUnassign();
                        emptyDatasetFlag = true;
                    }
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

            switch (windowData.SelectedTabIndex)
            {
                case 0:
                    assetDatabaseTab.OnGUI(this.position);
                    break;
                case 1:
                    assetBundleTab.OnGUI();
                    break;
            }

            EditorGUILayout.EndVertical();
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