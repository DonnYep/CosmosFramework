using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif
# if UNITY_EDITOR
namespace Cosmos.CosmosEditor
{
    public static class CosmosEditorUtility
    {
        static readonly Vector2 cosmosDevWinSize = new Vector2(512f, 384f);
        static readonly Vector2 cosmosMaxWinSize = new Vector2(768f, 768f);
        public static Vector2 CosmosDevWinSize { get { return cosmosDevWinSize; } }
        public static Vector2 CosmosMaxWinSize { get { return cosmosMaxWinSize; } }
        public static string LibraryCachePath
        {
            get
            {
                return Utility.IO.CombineRelativePath(LibraryPath(), "CosmosFramework"); ;
            }
        }
        /// <summary>
        /// 刷新unity编辑器；
        /// </summary>
        public static void RefreshEditor()
        {
            UnityEditor.AssetDatabase.Refresh();
        }
        public static string LibraryPath()
        {
            var editorPath = new DirectoryInfo(Application.dataPath);
            var parentPath = editorPath.Parent;
            var dirs = parentPath.GetDirectories();
            string libraryPath = "";
            for (int i = 0; i < dirs.Length; i++)
            {
                if (dirs[i].Name == "Library")
                {
                    libraryPath = dirs[i].FullName;
                    return libraryPath;
                }
            }
            return libraryPath;
        }
        public static void WriteEditorConfig<T>(string fileName, T editorData)
            where T : IEditorData
        {
            var jsom = JsonUtility.ToJson(editorData, true);
            WriteEditorConfig(fileName, jsom);
        }
        public static void WriteEditorConfig(string fileName, string context)
        {
            Utility.IO.WriterFormattedBinary(CosmosEditorUtility.LibraryCachePath, fileName, context);
        }
        public static T ReadEditorConfig<T>(string fileName)
            where T : IEditorData
        {
            var json = ReadEditorConfig(fileName);
            var obj = JsonUtility.FromJson<T>(json);
            return obj;
        }
        public static string ReadEditorConfig(string fileName)
        {
            var filePath = Utility.IO.CombineRelativeFilePath(fileName, CosmosEditorUtility.LibraryCachePath);
            var cfgStr = Utility.IO.ReadFormattedBinary(filePath);
            return cfgStr.ToString();
        }
        public static void ClearEditorConfig(string fileName)
        {
            var filePath = Utility.IO.CombineRelativeFilePath(fileName, CosmosEditorUtility.LibraryCachePath);
            Utility.IO.DeleteFile(filePath);
        }
        public static string GetDefaultLogOutputDirectory()
        {
            DirectoryInfo info = new DirectoryInfo(Application.dataPath);
            string path = info.Parent.FullName;
            return path;
        }
        public static void LogInfo(object msg, object context = null)
        {
            if (context == null)
                Debug.Log($"<b><color={MessageColor.CYAN}>{"[EDITOR-INFO]-->>"} </color></b>{msg}");
            else
                Debug.Log($"<b><color={MessageColor.CYAN}>{"[EDITOR-INFO]-->>"}</color></b>{msg}", context as Object);
        }
        public static void LogInfo(object msg, string msgColor, object context = null)
        {
            if (context == null)
                Debug.Log($"<b><color={msgColor }>{"[EDITOR-INFO]-->>"}</color></b>{msg}");
            else
                Debug.Log($"<b><color={msgColor }>{"[EDITOR-INFO]-->>"}</color></b>{msg}", context as Object);
        }
        public static void LogWarning(object msg, object context = null)
        {
            if (context == null)
                Debug.LogWarning($"<b><color={MessageColor.ORANGE}>{"[EDITOR-WARNING]-->>" }</color></b>{msg}");
            else
                Debug.LogWarning($"<b><color={MessageColor.ORANGE}>{"[EDITOR-WARNING]-->>" }</color></b>{msg}", context as Object);
        }
        public static void LogError(object msg, object context = null)
        {
            if (context == null)
                Debug.LogError($"<b><color={MessageColor.RED}>{"[EDITOR-ERROR]-->>"} </color></b>{msg}");
            else
                Debug.LogError($"<b><color={MessageColor.RED}>{"[EDITOR-ERROR]-->>"}</color></b>{msg}", context as Object);
        }
        public static void LogFatal(object msg, object context = null)
        {
            if (context == null)
                Debug.LogError($"<b><color={MessageColor.RED}>{ "[EDITOR-FATAL]-->>" }</color></b>{msg}");
            else
                Debug.LogError($"<b><color={MessageColor.RED}>{ "[EDITOR-FATAL]-->>" }</color></b>{msg}", context as Object);
        }
    }
}
#endif
