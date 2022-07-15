using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class ResourceEditorUtility
    {
        /// <summary>
        /// 获取文件夹中文件的总体大小；
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="availableExtes">可识别的后缀名</param>
        /// <returns>size</returns>
        public static long GetDirectorySize(string path, List<string> availableExtes)
        {
            if (!path.StartsWith("Assets"))
                return 0;
            if (!AssetDatabase.IsValidFolder(path))
                return 0;
            var fullPath = Path.Combine(EditorUtil.ApplicationPath(), path);
            if (!Directory.Exists(fullPath))
                return 0;
            DirectoryInfo directory = new DirectoryInfo(fullPath);
            var allFiles = directory.GetFiles();
            long totalSize = 0;
            foreach (var file in allFiles)
            {
                if (availableExtes.Contains(file.Extension))
                    totalSize += file.Length;
            }
            return totalSize;
        }
        /// <summary>
        /// 获取原生资产icon
        /// </summary>
        /// <returns>icon</returns>
        public static Texture2D GetAssetIcon()
        {
            return EditorGUIUtility.FindTexture("Prefab Icon");
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
        public static Texture2D GetAssetInvalidIcon()
        {
            return EditorGUIUtility.FindTexture("TestFailed");
        }
        public static Texture2D GetAssetValidIcon()
        {
            return EditorGUIUtility.FindTexture("TestPassed");
        }
        public static MultiColumnHeaderState CreateResourceBundleMultiColumnHeader()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Size"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    sortedAscending = false,
                    minWidth=64,
                    width=72,
                    maxWidth=128,
                    autoResize = true,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Bundle"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    sortedAscending = false,
                    minWidth=192,
                    width = 768,
                    maxWidth=1024,
                    autoResize = false,
                    canSort=true
                }
            };
            var state = new MultiColumnHeaderState(columns);
            return state;
        }
        public static MultiColumnHeaderState CreateResourceObjectMultiColumnHeader()
        {
            var columns = new[]
            {
            new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    sortedAscending = false,
                    minWidth=128,
                    width = 160,
                    maxWidth=512,
                    autoResize = true,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Extension"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    sortedAscending = false,
                  minWidth=64,
                    width=72,
                    maxWidth=128,
                    autoResize = true,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("State"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    sortedAscending = false,
                    minWidth=64,
                    width=96,
                    maxWidth=192,
                    autoResize = true,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Size"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    sortedAscending = false,
                    minWidth=64,
                    width=72,
                    maxWidth=128,
                    autoResize = true,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("AssetBundle"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    sortedAscending = false,
                    minWidth=64,
                    width=96,
                    maxWidth=384,
                    autoResize = true,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("AssetPath"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    sortedAscending = false,
                    minWidth=192,
                    width=768,
                    maxWidth=1024,
                    autoResize = true,
                }
            };

            var state = new MultiColumnHeaderState(columns);
            return state;
        }
    }
}
