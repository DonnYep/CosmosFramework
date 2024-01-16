using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public partial class ResourceEditorUtility
    {
        /// <summary>
        /// 检测资源与场景是否同处于一个AB包中
        /// </summary>
        /// <param name="bundlePath">包地址</param>
        /// <returns>是否处于同一个包</returns>
        public static bool CheckAssetsAndScenesInOneAssetBundle(string bundlePath)
        {
            if (File.Exists(bundlePath))//若是文件
                return false;
            var exts = Directory.GetFiles(bundlePath, ".", SearchOption.AllDirectories).Select(path => Path.GetExtension(path)).ToHashSet();
            exts.Remove(".meta");
            if (exts.Contains(".unity"))
            {
                exts.Remove(".unity");
                return exts.Count != 0;
            }
            return false;
        }
        public static Texture2D GetHorizontalLayoutGroupIcon()
        {
            return EditorGUIUtility.FindTexture("HorizontalLayoutGroup Icon");
        }
        /// <summary>
        /// 获取原生场景资源icon
        /// </summary>
        /// <returns>icon</returns>
        public static Texture2D GetSceneIcon()
        {
            return EditorGUIUtility.FindTexture("SceneAsset Icon");
        }
        /// <summary>
        /// 获取原生Folder资源icon
        /// </summary>
        /// <returns>icon</returns>
        public static Texture2D GetFolderIcon()
        {
            return EditorGUIUtility.FindTexture("Folder Icon");
        }
        public static Texture2D GetFolderEmptyIcon()
        {
            return EditorGUIUtility.FindTexture("FolderEmpty Icon");
        }
        public static Texture2D GetFindDependenciesIcon()
        {
            return EditorGUIUtility.FindTexture("UnityEditor.FindDependencies");
        }
        public static Texture2D GetAssetRefreshIcon()
        {
            return EditorGUIUtility.FindTexture("Refresh");
        }
        public static Texture2D GetInvalidIcon()
        {
            return EditorGUIUtility.FindTexture("TestFailed");
        }
        public static Texture2D GetValidIcon()
        {
            return EditorGUIUtility.FindTexture("TestPassed");
        }
        public static Texture2D GetIgnoredcon()
        {
            return EditorGUIUtility.FindTexture("TestIgnored");
        }
        public static Texture2D GetFilterByTypeIcon()
        {
            return EditorGUIUtility.FindTexture("FilterByType");
        }
        public static Texture2D GetCreateAddNewIcon()
        {
            return EditorGUIUtility.FindTexture("CreateAddNew");
        }
        public static Texture2D GetSaveActiveIcon()
        {
            return EditorGUIUtility.FindTexture("SaveActive");
        }
    }
}
