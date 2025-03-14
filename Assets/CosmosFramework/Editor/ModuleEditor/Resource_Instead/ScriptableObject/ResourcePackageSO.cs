using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

namespace Cosmos.Editor.Resource
{
    [CreateAssetMenu(fileName = "ResourcePackageSO_new", menuName = "Cosmos/ResourcePackageSO")]
    /// <summary>
    /// 一个pack包含多个bundle
    /// </summary>
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
                bundleSoDict[bundle.name] = bundle;
            }
        }
    }
}
