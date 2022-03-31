using UnityEngine;
using UnityEditor;
using System.IO;
using System;
namespace Cosmos.Editor.Hotfix
{
    public class HotfixWindow : EditorWindow
    {
        static HotfixWindowData hotfixWindowData;
        internal const string HotfixWindowDataFileName = "HotfixWindowData.json";
        static DefaultAsset assetRelativeFolderAsset;
        Vector2 m_ScrollPos;
        const string CosmosHotfixSymbol = "COSMOS_HOTFIX";
        public HotfixWindow()
        {
            this.titleContent = new GUIContent("HotfixTool");
        }
        [MenuItem("Window/Cosmos/Hotfix")]
        public static void OpenWindow()
        {
            var window = GetWindow<HotfixWindow>();
        }
        private void OnEnable()
        {
            try
            {
                hotfixWindowData = EditorUtil.GetData<HotfixWindowData>(HotfixWindowDataFileName);
            }
            catch
            {
                hotfixWindowData = new HotfixWindowData();
                EditorUtil.SaveData(HotfixWindowDataFileName, hotfixWindowData);
            }
            try
            {
                assetRelativeFolderAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(hotfixWindowData.AssetsRelativePath);
            }
            catch { }
        }
        void OnDisable()
        {
            EditorUtil.SaveData(HotfixWindowDataFileName, hotfixWindowData);
        }
        private void OnGUI()
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            GUILayout.Space(16);
            EditorGUILayout.HelpBox("热更新工具，用于加载编译后的dll与pdb", MessageType.Info);
            GUILayout.Space(16);

            GUILayout.BeginVertical();
            hotfixWindowData.CompiledAssemblyPath = EditorGUILayout.TextField("CompiledAssemblyPath ", hotfixWindowData.CompiledAssemblyPath);
            assetRelativeFolderAsset = (DefaultAsset)EditorGUILayout.ObjectField("AssginAssetsRelative", assetRelativeFolderAsset, typeof(DefaultAsset), false);
            string lableFieldStr = string.IsNullOrEmpty(hotfixWindowData.AssetsRelativePath) == true ? "Null" : hotfixWindowData.AssetsRelativePath;
            EditorGUILayout.LabelField("AssetsRelativePath：", lableFieldStr);
            if (assetRelativeFolderAsset != null)
            {
                hotfixWindowData.AssetsRelativePath = AssetDatabase.GetAssetPath(assetRelativeFolderAsset);
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("BrowseAssembly", GUILayout.MaxWidth(128f)))
            {
                hotfixWindowData.CompiledAssemblyPath = EditorUtility.OpenFilePanel("BrowseAssembly", Directory.GetCurrentDirectory(), "dll");
            }
            if (GUILayout.Button("ResetRelativePath", GUILayout.MaxWidth(128f)))
            {
                assetRelativeFolderAsset = null;
                hotfixWindowData.AssetsRelativePath = string.Empty;
            }
            GUILayout.EndHorizontal();

            hotfixWindowData.AppendExtension = EditorGUILayout.ToggleLeft("AppendExtension", hotfixWindowData.AppendExtension);
            if (hotfixWindowData.AppendExtension)
            {
                hotfixWindowData.Extension = EditorGUILayout.TextField("Extension", hotfixWindowData.Extension);
            }
            GUILayout.EndVertical();

            GUILayout.Space(16);
            hotfixWindowData.AutoLoadHotfixCode = EditorGUILayout.ToggleLeft("AutoLoadHotfixCode", hotfixWindowData.AutoLoadHotfixCode);
            GUILayout.Space(16);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load"))
            {
                if (LoadHotfixAssembly())
                    EditorUtil.Debug.LogInfo("Hotfix code load done !");
            }
            if (GUILayout.Button("Reset"))
            {
                ResetHotfixAssembly();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(16);

            EditorGUILayout.EndScrollView();
        }
        static bool LoadHotfixAssembly()
        {
            bool result = false;
            if (!string.IsNullOrEmpty(hotfixWindowData.AssetsRelativePath))
                if (AssetDatabase.IsValidFolder(hotfixWindowData.AssetsRelativePath))
                {
                    Directory.CreateDirectory(Utility.IO.PathCombine(Directory.GetCurrentDirectory(), hotfixWindowData.AssetsRelativePath));
                }
            if (string.IsNullOrEmpty(hotfixWindowData.CompiledAssemblyPath))
            {
                EditorUtil.Debug.LogError("CompiledAssemblyPath is invalid !");
                return false;
            }
            if (File.Exists(hotfixWindowData.CompiledAssemblyPath))
            {
                var srcPath = Path.GetDirectoryName(hotfixWindowData.CompiledAssemblyPath);
                var fileInfo = new FileInfo(hotfixWindowData.CompiledAssemblyPath);
                var fileName = fileInfo.Name;
                var fileExtension = fileInfo.Extension;
                var fileNameWithoutExt = fileName.Replace(fileExtension, string.Empty);
                var srcDllPath = hotfixWindowData.CompiledAssemblyPath;
                var srcPdbPath = Path.Combine(srcPath, Utility.Text.Combine(fileNameWithoutExt, ".pdb"));
                string dstPath = Path.Combine(Directory.GetCurrentDirectory(), hotfixWindowData.AssetsRelativePath);
                string dstDllPath = Utility.Text.Combine(Path.Combine(dstPath, fileNameWithoutExt), ".dll");
                string dstPdbPath = Utility.Text.Combine(Path.Combine(dstPath, fileNameWithoutExt), ".pdb");
                if (hotfixWindowData.AppendExtension)
                {
                    dstDllPath = Utility.Text.Combine(dstDllPath, hotfixWindowData.Extension);
                    dstPdbPath = Utility.Text.Combine(dstPdbPath, hotfixWindowData.Extension);
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
                EditorUtil.Debug.LogError($"File {hotfixWindowData.CompiledAssemblyPath} not exist !");
            }
            return result;
        }
        static void ResetHotfixAssembly()
        {
            assetRelativeFolderAsset = null;
            hotfixWindowData.Reset();
            AssetDatabase.Refresh();
        }
        [InitializeOnLoadMethod]
        static void AutoLoadHotfixDll()
        {
            try
            {
                hotfixWindowData = EditorUtil.GetData<HotfixWindowData>(HotfixWindowDataFileName);
            }
            catch
            {
                hotfixWindowData = new HotfixWindowData();
                EditorUtil.SaveData(HotfixWindowDataFileName, hotfixWindowData);
            }
            if (hotfixWindowData.AutoLoadHotfixCode)
            {
                LoadHotfixAssembly();
            }
        }
    }
}
