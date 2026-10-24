using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cosmos.Resource
{
    /// <summary>
    /// 一次资源请求的完整描述，由资源桥构建后交给 Provider。
    /// <para>新增请求维度时只需扩展此结构，Provider 接口保持稳定。</para>
    /// </summary>
    public struct ResourceRequest
    {
        /// <summary>
        /// 资源信息
        /// </summary>
        public AssetInfo AssetInfo;
        /// <summary>
        /// 目标资产类型
        /// </summary>
        public Type AssetType;
        /// <summary>
        /// 加载方式
        /// </summary>
        public AssetLoadMode LoadMode;
        /// <summary>
        /// 场景加载方式（LoadMode 为 Scene 时有效）
        /// </summary>
        public LoadSceneMode SceneMode;
        /// <summary>
        /// 场景加载完成后是否立即激活（LoadMode 为 Scene 时有效）
        /// </summary>
        public bool ActivateOnLoad;
        /// <summary>
        /// 加载优先级（Provider 支持优先级时有效，数值越大越先执行）
        /// </summary>
        public uint Priority;
        /// <summary>
        /// 实例化父节点（LoadMode 为 Instance 时有效）
        /// </summary>
        public Transform InstanceParent;

        /// <summary>
        /// 创建单资产请求。
        /// </summary>
        public static ResourceRequest Asset(AssetInfo assetInfo, Type assetType, uint priority = 0)
        {
            return new ResourceRequest()
            {
                AssetInfo = assetInfo,
                AssetType = assetType,
                LoadMode = AssetLoadMode.SingleAsset,
                Priority = priority,
            };
        }
        /// <summary>
        /// 创建子资产请求。
        /// </summary>
        public static ResourceRequest SubAssets(AssetInfo assetInfo, Type assetType, uint priority = 0)
        {
            return new ResourceRequest()
            {
                AssetInfo = assetInfo,
                AssetType = assetType,
                LoadMode = AssetLoadMode.SubAssets,
                Priority = priority,
            };
        }
        /// <summary>
        /// 创建全部资产请求。
        /// </summary>
        public static ResourceRequest AllAssets(AssetInfo assetInfo, Type assetType, uint priority = 0)
        {
            return new ResourceRequest()
            {
                AssetInfo = assetInfo,
                AssetType = assetType,
                LoadMode = AssetLoadMode.AllAssets,
                Priority = priority,
            };
        }
        /// <summary>
        /// 创建场景请求。
        /// </summary>
        public static ResourceRequest Scene(AssetInfo assetInfo, LoadSceneMode sceneMode, bool activateOnLoad, uint priority = 0)
        {
            return new ResourceRequest()
            {
                AssetInfo = assetInfo,
                AssetType = typeof(Scene),
                LoadMode = AssetLoadMode.Scene,
                SceneMode = sceneMode,
                ActivateOnLoad = activateOnLoad,
                Priority = priority,
            };
        }
        /// <summary>
        /// 创建实例化请求。
        /// </summary>
        public static ResourceRequest Instance(AssetInfo assetInfo, Transform parent, uint priority = 0)
        {
            return new ResourceRequest()
            {
                AssetInfo = assetInfo,
                AssetType = typeof(GameObject),
                LoadMode = AssetLoadMode.Instance,
                InstanceParent = parent,
                Priority = priority,
            };
        }
    }
}
