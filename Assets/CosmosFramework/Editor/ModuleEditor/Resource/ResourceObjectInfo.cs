using System;

namespace Cosmos.Editor.Resource
{
    public struct ResourceObjectInfo : IEquatable<ResourceObjectInfo>
    {
        public string ObjectName { get; set; }
        public string AssetPath { get; set; }
        public string AssetBundleName { get; set; }
        public string FileSize { get; set; }
        public string Extension { get; set; }
        public ResourceObjectInfo(string objectName, string assetPath, string assetBundleName, string fileSize, string extension)
        {
            ObjectName = objectName;
            AssetPath = assetPath;
            FileSize = fileSize;
            AssetBundleName = assetBundleName;
            Extension = extension;
        }
        public bool Equals(ResourceObjectInfo other)
        {
            return other.ObjectName == this.ObjectName &&
                other.AssetPath == this.AssetPath &&
                other.AssetBundleName == this.AssetBundleName &&
                other.Extension == this.Extension;
        }
        public override int GetHashCode()
        {
            return $"{ObjectName}{FileSize}{AssetPath}{AssetBundleName}".GetHashCode();
        }
    }
}
