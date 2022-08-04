using System;

namespace Cosmos.Editor.Resource
{
    public struct ResourceObjectInfo : IEquatable<ResourceObjectInfo>
    {
        public string ObjectName;
        public string AssetPath;
        public string AssetBundleName;
        public string FileSize;
        public string Extension;
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
