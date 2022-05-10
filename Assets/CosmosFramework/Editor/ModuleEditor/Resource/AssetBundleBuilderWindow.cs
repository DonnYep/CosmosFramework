using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.IO;

namespace Cosmos.Editor
{
    public class AssetBundleBuilderWindow : EditorWindow
    {
        AssetBundleBuilderData data;
        readonly string AssetBundleBuilderDataName = "AssetBundleBuilderData.json";
        public AssetBundleBuilderWindow()
        {
            this.titleContent = new GUIContent("AssetBundleBuilder");
        }
        [MenuItem("Window/Cosmos/ModuleEditor/Resource")]
        public static void OpenIntegrateWindow()
        {
            var window = GetWindow<AssetBundleBuilderWindow>();
            window.maxSize = EditorUtil.MaxWinSize;
            window.minSize = EditorUtil.DevWinSize;
        }
        private void OnEnable()
        {
            try
            {
                data = EditorUtil.GetData<AssetBundleBuilderData>(AssetBundleBuilderDataName);
            }
            catch
            {
                data = new AssetBundleBuilderData();
                EditorUtil.SaveData(AssetBundleBuilderDataName, data);
            }
        }
        private void OnDisable()
        {
            EditorUtil.SaveData(AssetBundleBuilderDataName, data);
        }
        private void OnGUI()
        {
            DrawBuildAssetBundle();
        }
        void DrawBuildAssetBundle()
        {
            data.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", data.BuildTarget);
            data.BuildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("BuildAssetBundleOptions", data.BuildAssetBundleOptions);
            data.OutputPath = EditorGUILayout.TextField("OutputPath", data.OutputPath.Trim());
            bool outputPathNull = string.IsNullOrEmpty(data.OutputPath);
            if (outputPathNull)
            {
                EditorGUILayout.HelpBox("Out put path is null !", MessageType.Error);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build"))
            {
                if (!outputPathNull)
                    BuildPipeline.BuildAssetBundles(GetBuildFolder(), data.BuildAssetBundleOptions, data.BuildTarget);
            }
            if (GUILayout.Button("Reset"))
            {
                data = new AssetBundleBuilderData();
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
            var path = Path.Combine(EditorUtil.ApplicationPath(), data.OutputPath);
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