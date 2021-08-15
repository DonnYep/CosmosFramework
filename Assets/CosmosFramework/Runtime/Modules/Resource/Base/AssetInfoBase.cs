using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public abstract class AssetInfoBase
    {
        readonly string assetPath;
        readonly string assetBundleName;
        /// <summary>
        /// AssetBundle的名称
        /// </summary>
        public string AssetBundleName { get { return assetBundleName; } }
        /// <summary>
        /// AB包地址、Resource地址或其他路径地址；
        /// </summary>
        public string AssetPath { get { return assetPath; } }
        public AssetInfoBase() { }
        public AssetInfoBase(string assetPath) : this(string.Empty, assetPath) { }
        public AssetInfoBase(string assetBundleName, string assetPath)
        {
            this.assetBundleName = string.IsNullOrEmpty(assetBundleName) ? assetBundleName : assetBundleName.ToLower();
            this.assetPath = assetPath;
        }
    }
}
