using System;

namespace Cosmos.Editor.Resource
{
    public struct ResourceObjectInfo:IEquatable<ResourceObjectInfo>
    {
        public string ObjectName { get; private set; }
        public string AssetPath { get; private set; }
        public string FileSize { get; private set; }
        public long FileSizeLength { get; private set; }
        public string AssetBundleName{ get; private set; }
        public ResourceObjectInfo(string objectName, string assetPath,string assetBundleName,string fileSize,long fileSizeLength)
        {
            ObjectName = objectName;
            AssetPath = assetPath;
            FileSize = fileSize;
            FileSizeLength = fileSizeLength;
            AssetBundleName = assetBundleName;
        }
        public bool Equals(ResourceObjectInfo other)
        {
            return other.ObjectName == this.ObjectName &&
                other.AssetPath == this.AssetPath&&
                other.FileSizeLength==this.FileSizeLength&&
                other.AssetBundleName==this.AssetBundleName;
        }
        public override int GetHashCode()
        {
            return $"{ObjectName}{FileSize}{AssetPath}{AssetBundleName}".GetHashCode();
        }
    }
}
