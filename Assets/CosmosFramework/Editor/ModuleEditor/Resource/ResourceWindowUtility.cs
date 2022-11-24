using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public class ResourceWindowUtility
    {
        /// <summary>
        /// 检测资源与场景是否同处于一个AB包中；
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
        public static Texture2D GetAssetRefreshIcon()
        {
            return EditorGUIUtility.FindTexture("Refresh");
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
                    headerContent = new GUIContent("Amount"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    sortedAscending = false,
                    minWidth=36,
                    width=56,
                    maxWidth=80,
                    autoResize = true,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Bundle"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    sortedAscending = false,
                    minWidth=192,
                    width = 384,
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
