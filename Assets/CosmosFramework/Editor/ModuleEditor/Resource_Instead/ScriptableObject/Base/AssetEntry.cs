using System;

namespace Cosmos.Editor.Resource
{
    [Serializable]
    /// <summary>
    /// 用于SO加载
    /// </summary>
    public class AssetEntry : IEquatable<AssetEntry>
    {
        /// <summary>
        /// unity生成的guid
        /// </summary>
        public string Guid;
        public string AssetPath;
        public string Extension;
        public string Name;
        public string[] AssetTags;
        /// <summary>
        /// editor下，bundleHash指的是所属的SO文件guid。
        /// </summary>
        public string BundleHash;

        public bool Equals(AssetEntry other)
        {
            return this.Guid == other.Guid;
        }
    }
}
