using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using Cosmos;
#if UNITY_EDITOR
namespace CosmosEditor
{
    public static partial class EditorUtil
    {
        public const string CosmosFramework = "CosmosFramework";
        public static string LibraryPath
        {
            get
            {
                if (string.IsNullOrEmpty(libraryPath))
                {
                    var editorPath = new DirectoryInfo(Application.dataPath);
                    var rootPath = editorPath.Parent.FullName + "/Library/";
                    libraryPath = Utility.IO.PathCombine(rootPath, CosmosFramework);
                }
                return libraryPath;
            }
        }
        static string libraryPath;
        static readonly Vector2 cosmosDevWinSize = new Vector2(512f, 384f);
        static readonly Vector2 cosmosMaxWinSize = new Vector2(768f, 768f);
        public static Vector2 CosmosDevWinSize { get { return cosmosDevWinSize; } }
        public static Vector2 CosmosMaxWinSize { get { return cosmosMaxWinSize; } }

        /// <summary>
        ///获取项目的目录，即Assets文件夹的上一层； 
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
                      })
                          .OfType<T>()
                          .ToArray();
        }
        /// <summary>
        /// 写入方式为覆写；
        /// 对象数据会被存储为json；
        /// </summary>
        public static void SaveData<T>(string fileName, T editorData)
            where T : class, new()
        {
            var json = EditorUtil.Json.ToJson(editorData, true);
            Utility.IO.OverwriteTextFile(LibraryPath, fileName, json);
        }
        public static T GetData<T>(string fileName)
            where T : class, new()
        {
            var filePath = Utility.IO.PathCombine(LibraryPath, fileName);
            var json = Utility.IO.ReadTextFileContent(filePath);
            var obj = EditorUtil.Json.ToObject<T>(json);
            return obj;
        }
        public static void ClearData(string fileName)
        {
            var filePath = Utility.IO.PathCombine(LibraryPath, fileName);
            Utility.IO.DeleteFile(filePath);
        }
        public static void DrawVerticalContext(Action context)
        {
            GUILayout.BeginVertical();
            context?.Invoke();
            GUILayout.EndVertical();
        }
        public static void DrawVerticalContext(Action context, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
            context?.Invoke();
            GUILayout.EndVertical();
        }
        public static void DrawHorizontalContext(Action context, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
            context?.Invoke();
            GUILayout.EndHorizontal();
        }
        public static void DrawHorizontalContext(Action context)
        {
            GUILayout.BeginHorizontal();
            context?.Invoke();
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// 定位目标路径在Assets下的位置；
        /// </summary>
        /// <param name="path">目标路径</param>
        public static void PingAndActiveObject(string path)
        {
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }
        public static void SelectionActiveObject(string path)
        {
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            Selection.activeObject = obj;
        }
        /// <summary>
        /// 添加单个宏；此宏不能由分号结尾；
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
        /// 移除单个宏；此宏不能由分号结尾；
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
    }
}
#endif
