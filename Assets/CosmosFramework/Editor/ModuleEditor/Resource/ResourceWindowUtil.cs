using UnityEditor;
using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class ResourceWindowUtil
    {
        /// <summary>
        /// 获取原生资产icon
        /// </summary>
        /// <returns>icon</returns>
        public static Texture2D GetAssetIcon()
        {
            return EditorUtil.ToTexture2D(EditorGUIUtility.IconContent("Prefab Icon").image);
        }
        /// <summary>
        /// 获取原生场景资源icon
        /// </summary>
        /// <returns>icon</returns>
        public static Texture2D GetSceneIcon()
        {
            return EditorUtil.ToTexture2D(EditorGUIUtility.IconContent("SceneAsset Icon").image);
        }
        public static Texture2D GetFolderIcon()
        {
            return EditorUtil.ToTexture2D(EditorGUIUtility.IconContent("Folder Icon").image);
        }
    }
}
