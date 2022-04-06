using System;
using UnityEngine;
namespace Quark.Asset
{
    internal class QuarkAssetBundle :IDisposable
    {
        /// <summary>
        /// AB包的名称；
        /// </summary>
        public string AssetBundleName { get; set; }
        /// <summary>
        /// AssetBundle 包体对象；
        /// </summary>
        public AssetBundle AssetBundle { get; set; }
        /// <summary>
        /// 包体对应的引用计数；
        /// </summary>
        public int ReferenceCount { get; set; }
        public QuarkAssetBundle(string assetBundleName, AssetBundle assetBundle)
        {
            AssetBundleName = assetBundleName;
            AssetBundle = assetBundle;
        }
        public void Dispose()
        {
            AssetBundleName = string.Empty ;
            AssetBundle = null;
            ReferenceCount = 0;
        }
    }
}
