using System;
using UnityEngine;
using System.Collections.Generic;
namespace Quark.Asset
{
    /// <summary>
    /// QuarkAssetDataset用于在Editor Runtime快速开发时使用；
    /// build之后需配合AB资源使用；
    /// </summary>
    [Serializable]
    public sealed class QuarkAssetDataset : ScriptableObject, IDisposable, IQuarkLoaderData
    {
        Dictionary<string, List<QuarkAssetObject>> assetBundleDict;
        [SerializeField]
        int quarkAssetCount;
        [SerializeField]
        List<QuarkBundleInfo> namePathInfoList;
        [SerializeField]
        List<QuarkAssetObject> quarkAssetObjectList;
        public int QuarkAssetCount { get { return quarkAssetCount; } set { quarkAssetCount = value; } }
        /// <summary>
        /// 包含的路径；
        /// <see cref="QuarkBundleInfo"/>
        /// </summary>
        public List<QuarkBundleInfo> NamePathInfoList
        {
            get
            {
                if (namePathInfoList == null)
                    namePathInfoList = new List<QuarkBundleInfo>();
                return namePathInfoList;
            }
        }
        public List<QuarkAssetObject> QuarkAssetObjectList
        {
            get
            {
                if (quarkAssetObjectList == null)
                    quarkAssetObjectList = new List<QuarkAssetObject>();
                return quarkAssetObjectList;
            }
        }
        /// <summary>
        /// AB名===AB中的资源；
        /// </summary>
        public Dictionary<string, List<QuarkAssetObject>> AssetBundleDict
        {
            get
            {
                if (assetBundleDict == null)
                    assetBundleDict = new Dictionary<string, List<QuarkAssetObject>>();
                return assetBundleDict;
            }
        }
        public void Dispose()
        {
            quarkAssetCount = 0;
            quarkAssetObjectList?.Clear();
            namePathInfoList?.Clear();
            assetBundleDict?.Clear();
        }
    }
}