using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEditorInternal;

namespace Cosmos.Editor.Hotfix
{
    public class HotUpdateWindow : ModuleWindowBase
    {
        static HotUpdateWindowData windowData;
        internal const string HotUpdateWindowDataFileName = "HotUpdateWindowData.json";
        Vector2 scrollPos;
        ReorderableList reorderableList;

        [MenuItem("Window/Cosmos/Module/HotUpdate")]
        public static void OpenWindow()
        {
            var window = GetWindow<HotUpdateWindow>();
            window.titleContent = new GUIContent("HotUpdateWindow");
        }
        private void OnEnable()
        {
            try
            {
                windowData = EditorUtil.GetData<HotUpdateWindowData>(HotUpdateWindowDataFileName);
            }
            catch
            {
                windowData = new HotUpdateWindowData();
                EditorUtil.SaveData(HotUpdateWindowDataFileName, windowData);
            }
        }
        void OnDisable()
        {
            EditorUtil.SaveData(HotUpdateWindowDataFileName, windowData);
        }
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {
                GUILayout.Space(16);
                DrawAssembliesOptions();
                GUILayout.Space(16);
                DrawExtensionOptions();
                GUILayout.Space(16);
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Copy Assemblies"))
                    {
                        //if (CopyAssemblies())
                        //    EditorUtil.Debug.LogInfo("HotUpdate code copy done !");
                    }
                    if (GUILayout.Button("Reset Options"))
                    {
                        ResetOptions();
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        void DrawAssembliesOptions()
        {
            EditorGUILayout.LabelField("Assemblies Options", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            {
                windowData.AssembliesPath = EditorGUILayout.TextField("Assemblies path", windowData.AssembliesPath);
                if (GUILayout.Button("Browse", GUILayout.MaxWidth(128f)))
                {
                    windowData.AssembliesPath = EditorUtility.OpenFolderPanel("BrowseAssemblies", Directory.GetCurrentDirectory(), Directory.GetCurrentDirectory());
                }
            }
            GUILayout.EndHorizontal();

            var pathValid = !string.IsNullOrEmpty(windowData.AssembliesPastePath);
            windowData.AssembliesPastePath = EditorGUILayout.TextField("Assemblies Paste path", windowData.AssembliesPastePath);
            string fullPastePath = string.Empty;
            if (pathValid)
            {
                fullPastePath = Utility.IO.WebPathCombine(EditorUtil.ApplicationPath(), windowData.AssembliesPastePath);
                windowData.FullAssembliesPastePath = fullPastePath;
            }
            else
            {
                fullPastePath = Utility.IO.WebPathCombine(EditorUtil.ApplicationPath());
            }
            EditorGUILayout.LabelField("Paste full path：", fullPastePath);
        }
        void DrawExtensionOptions()
        {
            EditorGUILayout.LabelField("Extension Options", EditorStyles.boldLabel);
            windowData.AppendExtension = EditorGUILayout.ToggleLeft("Append extension", windowData.AppendExtension);
            if (windowData.AppendExtension)
            {
                windowData.Extension = EditorGUILayout.TextField("Extension", windowData.Extension);
            }
        }
        bool CopyAssemblies()
        {
            bool result = false;
            if (string.IsNullOrEmpty(windowData.AssembliesPath))
            {
                EditorUtil.Debug.LogError("CompiledAssemblyPath is invalid !");
                return false;
            }

            if (!string.IsNullOrEmpty(windowData.AssembliesPastePath))
            {
                if (AssetDatabase.IsValidFolder(windowData.AssembliesPastePath))
                {
                    Directory.CreateDirectory(Utility.IO.PathCombine(Directory.GetCurrentDirectory(), windowData.AssembliesPastePath));
                }
            }

            if (File.Exists(windowData.AssembliesPath))
            {
                var fileInfo = new FileInfo(windowData.AssembliesPath);
                var fileName = fileInfo.Name;
                var fileExtension = fileInfo.Extension;
                var fileNameWithoutExt = fileName.Replace(fileExtension, string.Empty);
                var srcDllPath = windowData.AssembliesPath;
                string dstPath = Path.Combine(Directory.GetCurrentDirectory(), windowData.AssembliesPastePath);
                string dstDllPath = Utility.Text.Combine(Path.Combine(dstPath, fileNameWithoutExt), HotUpdateWindowConstants.DllExtension);
                if (windowData.AppendExtension)
                {
                    dstDllPath = Utility.Text.Combine(dstDllPath, windowData.Extension);
                }
                try
                {
                    File.Copy(srcDllPath, dstDllPath, true);
                    AssetDatabase.Refresh();
                    result = true;
                }
                catch (Exception e)
                {
                    EditorUtil.Debug.LogError(e);
                }
            }
            else
            {
                EditorUtil.Debug.LogError($"File {windowData.AssembliesPath} not exist !");
            }
            return result;
        }
        void ResetOptions()
        {
            windowData.Reset();
            AssetDatabase.Refresh();
            Repaint();
        }
    }
}
