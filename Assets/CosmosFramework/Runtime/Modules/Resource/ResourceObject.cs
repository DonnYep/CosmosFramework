using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源实体对象；
    /// </summary>
    public struct ResourceObject : IEquatable<ResourceObject>
    {
        /// <summary>
        /// 资源的相对路径
        /// </summary>
        public string ResourcePath;
        /// <summary>
        /// 资源Bundle的名称
        /// </summary>
        public string ResourceBundleName;
        /// <summary>
        /// 资源类型；
        /// </summary>
        public string ResourceType;
        public ResourceObject(string resourcePath, string resourceBundleName,string resourceType)
        {
            ResourcePath = resourcePath;
            ResourceBundleName = resourceBundleName;
            ResourceType = resourceType;
        }
        public bool Equals(ResourceObject other)
        {
            return other.ResourcePath == this.ResourcePath &&
                other.ResourceBundleName == this.ResourceBundleName;
        }
        public override bool Equals(object obj)
        {
            return (obj is ResourceObject) && Equals((ResourceObject)obj);
        }
        public override int GetHashCode()
        {
            return $"{ResourcePath},{ResourceBundleName}".GetHashCode();
        }
        public override string ToString()
        {
            return $"ResourcePath: {ResourcePath} , ResourceBundleName: {ResourceBundleName}";
        }
        public ResourceObject Clone()
        {
            return new ResourceObject(this.ResourcePath, this.ResourceBundleName,this.ResourceType);
        }
    }
}
