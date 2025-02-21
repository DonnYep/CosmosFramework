using System;
using System.Linq;

namespace Cosmos.Resource
{
    /// <summary>
    /// 运行时加载的最资源单位
    /// </summary>
    internal class PackageAsset : IEquatable<PackageAsset>
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        public string AssetPath;
        /// <summary>
        /// 资源GUID
        /// </summary>
        public string AssetGuid;
        /// <summary>
        /// 资源的分类标签
        /// </summary>
        public string[] AssetTags;
        /// <summary>
        /// 所属bundle的hash
        /// </summary>
        public string BundleHash;
        /// <summary>
        /// 是否包含Tag
        /// </summary>
        public bool HasTag(string[] tags)
        {
            if (tags == null || tags.Length == 0)
                return false;
            if (AssetTags == null || AssetTags.Length == 0)
                return false;

            foreach (var tag in tags)
            {
                if (AssetTags.Contains(tag))
                    return true;
            }
            return false;
        }
        public bool Equals(PackageAsset other)
        {
            return other.AssetGuid == this.AssetGuid;
        }
    }
}
