using System;

namespace Cosmos.Editor.Resource
{
    public struct ResourceObjectItem : IEquatable<ResourceObjectItem>
    {
        public string ObjectName;
        public string AssetPath;
        public string AssetBundleName;
        public string FileSize;
        public string Extension;
        public bool Vaild;
        public ResourceObjectItem(string objectName, string assetPath, string assetBundleName, string fileSize, string extension, bool vaild)
        {
            ObjectName = objectName;
            AssetPath = assetPath;
            FileSize = fileSize;
            AssetBundleName = assetBundleName;
            Extension = extension;
            Vaild = vaild;
        }
        public bool Equals(ResourceObjectItem other)
        {
            return other.ObjectName == this.ObjectName &&
                other.AssetPath == this.AssetPath &&
                other.AssetBundleName == this.AssetBundleName &&
                other.Extension == this.Extension &&
                other.Vaild == this.Vaild;
        }
        public override int GetHashCode()
        {
            return $"{ObjectName}{FileSize}{AssetPath}{AssetBundleName}{Vaild}".GetHashCode();
        }
    }
}
