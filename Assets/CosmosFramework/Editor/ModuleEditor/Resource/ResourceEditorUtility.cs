using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class ResourceEditorUtility
    {
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
                    width = 256,
                    maxWidth=512,
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
