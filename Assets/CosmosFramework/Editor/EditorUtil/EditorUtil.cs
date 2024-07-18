using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text;

namespace Cosmos.Editor
{
    public static partial class EditorUtil
    {
        public const string EDITOR_CACHE_FOLDER_NAME = "CosmosFramework";
        public static string LibraryPath
        {
            get
            {
                if (string.IsNullOrEmpty(libraryPath))
                {
                    var editorPath = new DirectoryInfo(Application.dataPath);
                    var rootPath = editorPath.Parent.FullName + "/Library/";
                    libraryPath = Utility.IO.PathCombine(rootPath, EDITOR_CACHE_FOLDER_NAME);
                }
                return libraryPath;
            }
        }
        /// <summary>
        /// 项目工程地址，即Assets文件夹的上一层。
        /// </summary>
        public static string ProjectPath
        {
            get
            {
                var path = Directory.GetCurrentDirectory();
                return Utility.IO.FormatURL(path);
            }
        }
        static string libraryPath;
        public static readonly Vector2 DevWinSize = new Vector2(512f, 384f);
        public static readonly Vector2 MaxWinSize = new Vector2(768f, 768f);

        /// <summary>
        ///获取项目的目录，即Assets文件夹的上一层。
        /// </summary>
        public static string ApplicationPath()
        {
            return Directory.GetCurrentDirectory();
            //var dirInfo = new DirectoryInfo(Application.dataPath);
            //return dirInfo.Parent.FullName;
            //return Path.GetFullPath(".");
        }
        public static T[] GetAllAssets<T>(string path) where T : class
        {
            string[] fileEntries = Directory.GetFiles(path);
            return fileEntries.Select(fileName =>
                      {
                          string assetPath = fileName.Substring(fileName.IndexOf("Assets"));
                          assetPath = Path.ChangeExtension(assetPath, null);
                          if (!AssetDatabase.IsValidFolder(assetPath))
                              return UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
                          else
                              return null;
                      }).OfType<T>().ToArray();
        }
        /// <summary>
        /// 写入方式为覆写，对象数据会被存储为json。
        /// </summary>
        public static void SaveData<T>(string fileName, T editorData)
            where T : class, new()
        {
            var json = EditorUtil.Json.ToJson(editorData);
            Utility.IO.OverwriteTextFile(LibraryPath, fileName, json);
        }
        public static void SaveData<T>(string relativePath, string fileName, T editorData)
            where T : class, new()
        {
            var json = EditorUtil.Json.ToJson(editorData);
            var path = Path.Combine(LibraryPath, relativePath);
            Utility.IO.OverwriteTextFile(path, fileName, json);
        }
        public static T GetData<T>(string fileName)
            where T : class, new()
        {
            var filePath = Utility.IO.PathCombine(LibraryPath, fileName);
            var json = Utility.IO.ReadTextFileContent(filePath);
            var obj = EditorUtil.Json.ToObject<T>(json);
            return obj;
        }
        public static T GetData<T>(string relativePath, string fileName)
            where T : class, new()
        {
            var filePath = Utility.IO.PathCombine(LibraryPath, relativePath, fileName);
            var json = Utility.IO.ReadTextFileContent(filePath);
            var obj = EditorUtil.Json.ToObject<T>(json);
            return obj;
        }
        public static T SafeGetData<T>(string relativePath, string fileName, bool saveIfNotExist = true)
             where T : class, new()
        {
            T data = default;
            try
            {
                data = EditorUtil.GetData<T>(relativePath, fileName);
            }
            catch
            {
                data = new T();
                if (saveIfNotExist)
                    EditorUtil.SaveData(relativePath, fileName, data);
            }
            return data;
        }
        public static void SaveText(string fileName, string content)
        {
            Utility.IO.OverwriteTextFile(LibraryPath, fileName, content);
        }
        public static string GetText(string fileName)
        {
            var filePath = Utility.IO.PathCombine(LibraryPath, fileName);
            return Utility.IO.ReadTextFileContent(filePath);
        }
        public static void ClearData(string fileName)
        {
            var filePath = Utility.IO.PathCombine(LibraryPath, fileName);
            Utility.IO.DeleteFile(filePath);
        }
        /// <summary>
        /// 定位目标路径在Assets下的位置。
        /// </summary>
        /// <param name="path">目标路径</param>
        public static void PingAndActiveObject(string path)
        {
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }
        public static void ActiveObject(string path)
        {
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            Selection.activeObject = obj;
        }
        /// <summary>
        /// 添加单个宏，此宏不能由分号结尾。
        /// </summary>
        /// <param name="define">宏定义</param>
        /// <returns>是否添加成功</returns>
        public static bool AddSymbol(string define)
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            if (allDefines.Contains(define))
                return false;
            allDefines.Add(define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
            return true;
        }
        /// <summary>
        /// 移除单个宏，此宏不能由分号结尾。
        /// </summary>
        /// <param name="define">宏定义</param>
        /// <returns>是否移除成功</returns>
        public static bool RemoveSymbol(string define)
        {
            //Symbols.Remove(define);
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            if (!allDefines.Contains(define))
                return false;
            allDefines.Remove(define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
            return true;
        }
        /// <summary>
        /// 获取在build列表中场景的文件地址
        /// </summary>
        /// <returns>场景文件的地址列表</returns>
        public static string[] GetEditorScenesPath()
        {
            var scenes = EditorBuildSettings.scenes;
            return scenes.Select(s => s.path).ToArray();
        }
        /// <summary>
        /// 获取在build列表中enable状态的场景的文件地址
        /// </summary>
        /// <returns>场景文件的地址列表</returns>
        public static string[] GetEditorEnabledScenesPath()
        {
            var scenes = EditorBuildSettings.scenes;
            return scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        }
        /// <summary>
        /// 获取文件夹中文件的总体大小
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="availableExtes">可识别的后缀名</param>
        /// <returns>size</returns>
        public static long GetUnityDirectorySize(string path, List<string> availableExtes)
        {
            if (!path.StartsWith("Assets"))
                return 0;
            if (!AssetDatabase.IsValidFolder(path))
                return 0;
            var fullPath = Path.Combine(EditorUtil.ApplicationPath(), path);
            if (!Directory.Exists(fullPath))
                return 0;
            DirectoryInfo directory = new DirectoryInfo(fullPath);
            var allFiles = directory.GetFiles("*.*", SearchOption.AllDirectories);
            long totalSize = 0;
            foreach (var file in allFiles)
            {
                if (availableExtes.Contains(file.Extension))
                    totalSize += file.Length;
            }
            return totalSize;
        }
        /// <summary>
        /// 生成一个ScriptableObject对象
        /// </summary>
        /// <typeparam name="T">ScriptableObject对象</typeparam>
        /// <param name="path">Assets/下的地址，如: Assets/MyObject.asset</param>
        /// <param name="hideFlags">状态类型</param>
        /// <returns>生成的ScriptableObject</returns>
        public static T CreateScriptableObject<T>(string path, HideFlags hideFlags = HideFlags.None) where T : ScriptableObject
        {
            var dir = Path.GetDirectoryName(path);
            dir = Utility.IO.FormatURL(dir);
            var folderName = dir.Replace("Assets/", "");
            var isValid = AssetDatabase.IsValidFolder(dir);
            if (!isValid)
                AssetDatabase.CreateFolder("Assets", folderName);
            var so = ScriptableObject.CreateInstance<T>();
            so.hideFlags = hideFlags;
            AssetDatabase.CreateAsset(so, path);
            EditorUtility.SetDirty(so);
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = so;
#if UNITY_2021_1_OR_NEWER
            AssetDatabase.SaveAssetIfDirty(so);
#elif UNITY_2019_1_OR_NEWER
            AssetDatabase.SaveAssets();
#endif
            AssetDatabase.Refresh();
            return so;
        }
        public static void SaveScriptableObject(ScriptableObject scriptableObject)
        {
            if (scriptableObject == null)
                return;
            EditorUtility.SetDirty(scriptableObject);
#if UNITY_2021_1_OR_NEWER
            AssetDatabase.SaveAssetIfDirty(scriptableObject);
#elif UNITY_2019_1_OR_NEWER
            AssetDatabase.SaveAssets();
#endif
            AssetDatabase.Refresh();
        }
        /// <summary>
        ///  获取编辑渲染使用的hander数组。
        ///  默认首个元素为<see cref="Constants.NONE"/>
        /// </summary>
        /// <typeparam name="T">基类</typeparam>
        /// <returns>handler数组</returns>
        public static string[] GetDerivedTypeHandlers<T>()
            where T : class
        {
            var srcBuildHandlers = Utility.Assembly.GetDerivedTypeNames<T>();
            var buildHandlers = new string[srcBuildHandlers.Length + 1];
            buildHandlers[0] = Constants.NONE;
            Array.Copy(srcBuildHandlers, 0, buildHandlers, 1, srcBuildHandlers.Length);
            return buildHandlers;
        }
        /// <summary>
        /// 选择工程的相对路径
        /// </summary>
        /// <param name="relativePath">工程的相对路径</param>
        /// <returns>选择的工程相对路径</returns>
        public static string BrowseProjectReativeFolder(string relativePath)
        {
            var projectPath = ProjectPath;
            var projectAbsolutePath = Path.Combine(projectPath, relativePath);
            var selectedPath = EditorUtility.OpenFolderPanel("ProjectReativeFolder", projectAbsolutePath, string.Empty);
            string selectedRelativePath = string.Empty;
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (selectedPath.Contains(projectPath))
                {
                    var removeCount = projectPath.Length + 1;
                    selectedRelativePath = selectedPath.Remove(0, removeCount);
                }
            }
            return selectedRelativePath;
        }

        /// <summary>
        /// 遍历文件夹下的文件。传入的文件夹名参考：Assets/Game。
        /// </summary>
        /// <param name="folder">文件夹名</param>
        /// <param name="handler">处理方法</param>
        public static void TraverseFolderFile(string folder, Action<UnityEngine.Object> handler)
        {
            if (string.IsNullOrEmpty(folder))
                throw new ArgumentNullException("Folder Name is invalid !");
            if (handler == null)
                throw new ArgumentNullException("Handler is invalid !");
            if (AssetDatabase.IsValidFolder(folder))
            {
                var assets = GetAllAssets<UnityEngine.Object>(folder);
                if (assets != null)
                {
                    var length = assets.Length;
                    for (int i = 0; i < length; i++)
                    {
                        handler.Invoke(assets[i]);
                    }
                }
                var subFolders = AssetDatabase.GetSubFolders(folder);
                if (subFolders != null)
                {
                    foreach (var subF in subFolders)
                    {
                        TraverseFolderFile(subF, handler);
                    }
                }
            }
        }
        /// <summary>
        /// 遍历assets目录下的所有文件
        /// </summary>
        /// <param name="handler">遍历回调</param>
        public static void TraverseAllFolderFile(Action<UnityEngine.Object> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("Handler is invalid !");
            var assets = GetAllAssets<UnityEngine.Object>("Assets");
            if (assets != null)
            {
                var length = assets.Length;
                for (int i = 0; i < length; i++)
                {
                    handler.Invoke(assets[i]);
                }
            }
            var subFolder = AssetDatabase.GetSubFolders("Assets");
            if (subFolder != null)
            {
                foreach (var subF in subFolder)
                {
                    TraverseFolderFile(subF, handler);
                }
            }
        }

        #region EditorCoroutine
        /// <summary>
        /// EditorCoroutine 嵌套协程无法识别 yield return IEnumerator；
        /// 嵌套协程尽量使用yield return EditorCoroutine；
        /// </summary>
        public static Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutine StartCoroutine(IEnumerator coroutine)
        {
            return Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(coroutine);
        }
        public static void StopCoroutine(Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutine coroutine)
        {
            Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StopCoroutine(coroutine);
        }
        public static void StopCoroutine(IEnumerator coroutine)
        {
            Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(coroutine);
        }
        #endregion

    }
}