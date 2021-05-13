using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public abstract class AssetInfoBase
    {
        /// <summary>
        /// AssetBundle的名称
        /// </summary>
        public string AssetBundleName { get; set; }
        /// <summary>
        /// Asset的路径
        /// </summary>
        public string AssetPath { get; set; }
        /// <summary>
        /// Resources文件夹中的路径
        /// </summary>
        public string ResourcePath { get; set; }
        /// <summary>
        /// 资源名；
        /// </summary>
        public string AssetName { get; set; }
        public AssetInfoBase(string assetBundleName, string assetPath, string resourcePath)
        {
            AssetBundleName = string.IsNullOrEmpty(assetBundleName) ? assetBundleName : assetBundleName.ToLower();
            AssetPath = assetPath;
            ResourcePath = resourcePath;
        }
        public AssetInfoBase(string resourcePath) : this(string.Empty, string.Empty, resourcePath)
        {
            ResourcePath = resourcePath;
        }
        public AssetInfoBase() : this(string.Empty, string.Empty, string.Empty){}
        public void SetAssetInfo(AssetInfoBase assetInfo)
        {
            this.AssetBundleName = assetInfo.AssetBundleName;
            this.AssetPath = assetInfo.AssetPath;
            this.ResourcePath = assetInfo.ResourcePath;
            this.AssetName = assetInfo.AssetName;
        }
    }
}
