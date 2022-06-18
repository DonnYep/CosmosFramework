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
        public static Texture GetAssetIcon()
        {
            return EditorGUIUtility.IconContent("Prefab Icon").image;
        }
        /// <summary>
        /// 获取原生场景资源icon
        /// </summary>
        /// <returns>icon</returns>
        public static Texture GetSceneIcon()
        {
            return EditorGUIUtility.IconContent("SceneAsset Icon").image;
        }
    }
}
