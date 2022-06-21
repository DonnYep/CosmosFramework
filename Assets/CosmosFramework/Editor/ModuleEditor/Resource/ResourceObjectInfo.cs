using System;

namespace Cosmos.Editor.Resource
{
    public struct ResourceObjectInfo:IEquatable<ResourceObjectInfo>
    {
        public string ObjectName;
        public string AssetPath;

        public ResourceObjectInfo(string objectName, string assetPath)
        {
            ObjectName = objectName;
            AssetPath = assetPath;
        }
        public bool Equals(ResourceObjectInfo other)
        {
            return other.ObjectName == this.ObjectName &&
                other.AssetPath == this.AssetPath; ;
        }
        public override int GetHashCode()
        {
            return $"{ObjectName}{AssetPath}".GetHashCode();
        }
    }
}
