using System;
using System.Collections.Generic;
using UnityEngine;
namespace Quark.Asset
{
    internal class QuarkAssetBundle : IDisposable
    {
        /// <summary>
        /// AB包的名称；
        /// </summary>
        public string AssetBundleName { get; set; }
        /// <summary>
        /// AB在Assets目录下的地址；
        /// </summary>
        public string AssetBundlePath{ get; set; }
        /// <summary>
        /// AssetBundle 包体对象；
        /// </summary>
        public AssetBundle AssetBundle { get; set; }
        /// <summary>
        /// 包所含的资源列表；
        /// </summary>
        public HashSet<QuarkAssetObjectWapper> Assets{ get; set; }
        /// <summary>
        /// 包体对应的引用计数；
        /// </summary>
        public int ReferenceCount { get; set; }
        public QuarkAssetBundle(string assetBundleName, AssetBundle assetBundle)
        {
            AssetBundleName = assetBundleName;
            AssetBundle = assetBundle;
            Assets = new HashSet<QuarkAssetObjectWapper>();
        }
        public void Dispose()
        {
            AssetBundleName = string.Empty;
            AssetBundlePath = string.Empty;
            AssetBundle = null;
            ReferenceCount = 0;
            Assets.Clear();
        }
    }
}
