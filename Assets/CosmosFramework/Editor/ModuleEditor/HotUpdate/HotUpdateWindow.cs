using UnityEngine;
using UnityEditor;
using System.IO;
using System;
namespace Cosmos.Editor.Hotfix
{
    public class HotUpdateWindow : ModuleWindowBase
    {
        static HotUpdateWindowData windowData;
        internal const string HotUpdateWindowDataFileName = "HotUpdateWindowData.json";
        Vector2 scrollPos;

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
                //GUILayout.Space(16);
                //EditorGUILayout.HelpBox("热更新工具，用于加载编译后的dll与pdb", MessageType.Info);
                GUILayout.Space(16);

                GUILayout.BeginVertical();

                windowData.CompiledDllPath = EditorGUILayout.TextField("Compiled dll path", windowData.CompiledDllPath);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("BrowsePath", GUILayout.MaxWidth(128f)))
                    {
                        windowData.CompiledDllPath = EditorUtility.OpenFilePanel("Browse compiled path", Directory.GetCurrentDirectory(), "dll");
                    }
                    if (GUILayout.Button("ResetPath", GUILayout.MaxWidth(128f)))
                    {
                        windowData.DllPastePath = string.Empty;
                    }
                }
                GUILayout.Space(16);
                GUILayout.EndHorizontal();

                var pathValid = !string.IsNullOrEmpty(windowData.DllPastePath);
                windowData.DllPastePath = EditorGUILayout.TextField("Dll project directory", windowData.DllPastePath);
                string fullDllPath = string.Empty;
                if (pathValid)
                {
                    fullDllPath = Utility.IO.WebPathCombine(EditorUtil.ApplicationPath(), windowData.DllPastePath);
                    windowData.FullDllPastePath = fullDllPath;
                }
                else
                {
                    fullDllPath = EditorUtil.ApplicationPath();
                }
                EditorGUILayout.LabelField("Full dll paste path：", fullDllPath);


                windowData.AppendExtension = EditorGUILayout.ToggleLeft("AppendExtension", windowData.AppendExtension);
                if (windowData.AppendExtension)
                {
                    windowData.Extension = EditorGUILayout.TextField("Extension", windowData.Extension);
                }
                GUILayout.EndVertical();

                GUILayout.Space(16);
                windowData.AutoLoadHotUpdateCode = EditorGUILayout.ToggleLeft("AutoLoadHotUpdateAssemblies", windowData.AutoLoadHotUpdateCode);
                GUILayout.Space(16);

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("LoadAssemblies"))
                    {
                        if (LoadAssemblies())
                            EditorUtil.Debug.LogInfo("HotUpdate code load done !");
                    }
                    if (GUILayout.Button("Reset"))
                    {
                        ResetHotfixAssembly();
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(16);
            }
            EditorGUILayout.EndScrollView();
        }
        void DrawPathOption()
        {
            EditorGUILayout.LabelField("Build Options", EditorStyles.boldLabel);

        }
        static bool LoadAssemblies()
        {
            bool result = false;
            if (!string.IsNullOrEmpty(windowData.DllPastePath))
                if (AssetDatabase.IsValidFolder(windowData.DllPastePath))
                {
                    Directory.CreateDirectory(Utility.IO.PathCombine(Directory.GetCurrentDirectory(), windowData.DllPastePath));
                }
            if (string.IsNullOrEmpty(windowData.CompiledDllPath))
            {
                EditorUtil.Debug.LogError("CompiledAssemblyPath is invalid !");
                return false;
            }
            if (File.Exists(windowData.CompiledDllPath))
            {
                var srcPath = Path.GetDirectoryName(windowData.CompiledDllPath);
                var fileInfo = new FileInfo(windowData.CompiledDllPath);
                var fileName = fileInfo.Name;
                var fileExtension = fileInfo.Extension;
                var fileNameWithoutExt = fileName.Replace(fileExtension, string.Empty);
                var srcDllPath = windowData.CompiledDllPath;
                var srcPdbPath = Path.Combine(srcPath, Utility.Text.Combine(fileNameWithoutExt, ".pdb"));
                string dstPath = Path.Combine(Directory.GetCurrentDirectory(), windowData.DllPastePath);
                string dstDllPath = Utility.Text.Combine(Path.Combine(dstPath, fileNameWithoutExt), ".dll");
                string dstPdbPath = Utility.Text.Combine(Path.Combine(dstPath, fileNameWithoutExt), ".pdb");
                if (windowData.AppendExtension)
                {
                    dstDllPath = Utility.Text.Combine(dstDllPath, windowData.Extension);
                    dstPdbPath = Utility.Text.Combine(dstPdbPath, windowData.Extension);
                }
                try
                {
                    File.Copy(srcDllPath, dstDllPath, true);
                    File.Copy(srcPdbPath, dstPdbPath, true);
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
                EditorUtil.Debug.LogError($"File {windowData.CompiledDllPath} not exist !");
            }
            return result;
        }
        static void ResetHotfixAssembly()
        {
            windowData.Reset();
            AssetDatabase.Refresh();
        }
        [InitializeOnLoadMethod]
        static void AutoLoadHotfixDll()
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
            if (windowData.AutoLoadHotUpdateCode)
            {
                LoadAssemblies();
            }
        }
    }
}
