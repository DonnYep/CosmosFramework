using Cosmos.Resource;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// 资源包裹配置。一个pack包含多个bundle。
    /// </summary>
    [CreateAssetMenu(fileName = "ResourcePackageSO_new", menuName = "Cosmos/ResourcePackageSO")]
    public class ResourcePackageSO : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 资源包裹名
        /// </summary>
        public string PackageName;
        /// <summary>
        /// 资源版本
        /// </summary>
        public string PackageVersion;
        /// <summary>
        /// 下属的PackageBundleSO文件hash值
        /// </summary>
        public string[] BundleHashs;

        [SerializeField]
        public List<PackageBundleSO> BundleSOList = new List<PackageBundleSO>();
        Dictionary<string, PackageBundleSO> bundleSoDict = new Dictionary<string, PackageBundleSO>();

        internal ICollection<PackageBundleSO> bundles
        {
            get { return bundleSoDict.Values; }
        }
        public void OnAfterDeserialize()
        {
            ResetBundleMap();
        }
        public void OnBeforeSerialize()
        {
            if (BundleSOList == null)
            {
                BundleSOList = new List<PackageBundleSO>();
                foreach (var bundle in bundles)
                {
                    BundleSOList.Add(bundle);
                }
            }
        }
        internal void ResetBundleMap()
        {
            bundleSoDict.Clear();
            foreach (var bundle in BundleSOList)
            {
                if (bundle == null)
                    continue;
                bundleSoDict[bundle.name] = bundle;
            }
        }

        /// <summary>
        /// 获取解析后的包裹名
        /// </summary>
        public string GetResolvedPackageName()
        {
            if (!string.IsNullOrEmpty(PackageName))
                return PackageName;
            return name;
        }

        /// <summary>
        /// 为包裹内所有资产设置AssetBundleName
        /// </summary>
        public void AssignBundleNames()
        {
            foreach (var bundleSO in BundleSOList)
            {
                if (bundleSO != null)
                    bundleSO.AssignBundleNameToAssets();
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 清除包裹内所有资产的AssetBundleName
        /// </summary>
        public void ClearBundleNames()
        {
            foreach (var bundleSO in BundleSOList)
            {
                if (bundleSO != null)
                    bundleSO.ClearBundleNameOnAssets();
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }
    }
}
