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
        readonly string ResourceWindowDataName = "ResourceWindowData.json";
        AssetDatabaseTab assetDatabaseTab = new AssetDatabaseTab();
        AssetBundleTab assetBundleTab = new AssetBundleTab();
        string[] tabArray = new string[] { "AssetDatabase", "AssetBundle" };
        ResourceDataset latestResourceDataset;
        public ResourceWindow()
        {
            this.titleContent = new GUIContent("ResourceWindow");
        }
        [MenuItem("Window/Cosmos/ModuleEditor/Resource")]
        public static void OpenIntegrateWindow()
        {
            var window = GetWindow<ResourceWindow>();
            window.maxSize = EditorUtil.MaxWinSize;
            window.minSize = EditorUtil.DevWinSize;
        }
        private void OnEnable()
        {
            try
            {
                windowData = EditorUtil.GetData<ResourceWindowData>(ResourceWindowDataName);
                if (!string.IsNullOrEmpty(windowData.ResourceDatasetPath))
                {
                    ResourceEditorDataProxy.ResourceDataset = AssetDatabase.LoadAssetAtPath<ResourceDataset>(windowData.ResourceDatasetPath);
                }
            }
            catch
            {
                windowData = new ResourceWindowData();
                EditorUtil.SaveData(ResourceWindowDataName, windowData);
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
            if (latestResourceDataset != ResourceEditorDataProxy.ResourceDataset)
            {
                ResourceEditorDataProxy.ResourceDataset = latestResourceDataset;
                if (ResourceEditorDataProxy.ResourceDataset != null)
                    assetDatabaseTab.OnAssign();
                else
                    assetDatabaseTab.OnUnassign();

            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create Dataset", GUILayout.MinWidth(128f)))
            {
                ResourceEditorDataProxy.ResourceDataset = CreateResourceDataset();
            }
            if (GUILayout.Button("Clear Dataset", GUILayout.MinWidth(128f)))
            {
                ResourceEditorDataProxy.ResourceDataset = null;
                windowData.ResourceDatasetPath = string.Empty;
            }
            EditorGUILayout.EndHorizontal();

            switch (windowData.SelectedTabIndex)
            {
                case 0:
                    assetDatabaseTab.OnGUI();
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
            so.hideFlags = HideFlags.NotEditable;
            AssetDatabase.CreateAsset(so, "Assets/New ResourceDataset.asset");
            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            var dataset = AssetDatabase.LoadAssetAtPath<ResourceDataset>("Assets/New ResourceDataset.asset");
            EditorUtil.Debug.LogInfo("ResourceDataset created successfully");
            return dataset;
        }

        void DrawBuildAssetBundle()
        {
            windowData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", windowData.BuildTarget);
            windowData.BuildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("BuildAssetBundleOptions", windowData.BuildAssetBundleOptions);
            windowData.OutputPath = EditorGUILayout.TextField("OutputPath", windowData.OutputPath.Trim());
            bool outputPathNull = string.IsNullOrEmpty(windowData.OutputPath);
            if (outputPathNull)
            {
                EditorGUILayout.HelpBox("Out put path is null !", MessageType.Error);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build"))
            {
                if (!outputPathNull)
                    BuildPipeline.BuildAssetBundles(GetBuildFolder(), windowData.BuildAssetBundleOptions, windowData.BuildTarget);
            }
            if (GUILayout.Button("Reset"))
            {
                windowData = new ResourceWindowData();
                Repaint();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(16);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OpenOutputPath"))
            {
                EditorUtility.RevealInFinder(GetBuildFolder());
            }
            if (GUILayout.Button("ClearAssetBundles"))
            {
            }
            GUILayout.EndHorizontal();
        }
        string GetBuildFolder()
        {
            var path = Path.Combine(EditorUtil.ApplicationPath(), windowData.OutputPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
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