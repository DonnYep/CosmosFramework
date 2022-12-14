using System;
using UnityEngine;
namespace Cosmos.Resource
{
    /// <summary>
    /// 资源对象
    /// </summary>
    [Serializable]
    public class ResourceObject : IEquatable<ResourceObject>
    {
        [SerializeField]
        string objectName;
        [SerializeField]
        string objectPath;
        [SerializeField]
        string bundleName;
        [SerializeField]
        string extension;
        /// <summary>
        /// 资源名
        /// </summary>
        public string ObjectName
        {
            get { return objectName; }
            set { objectName = value; }
        }
        /// <summary>
        /// 资源在asset目录下的地址
        /// </summary>
        public string ObjectPath
        {
            get { return objectPath; }
            set { objectPath = value; }
        }
        /// <summary>
        /// 资源Bundle的名称
        /// </summary>
        public string BundleName
        {
            get { return bundleName; }
            set { bundleName = value; }
        }
        /// <summary>
        /// 后缀名；
        /// </summary>
        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }
        public ResourceObject() { }
        public bool Equals(ResourceObject other)
        {
            return other.Extension == this.Extension &&
                other.BundleName == this.BundleName &&
                other.ObjectName == this.ObjectName &&
                other.ObjectPath == this.ObjectPath;
        }
        public override bool Equals(object obj)
        {
            return (obj is ResourceObject) && Equals((ResourceObject)obj);
        }
        public override int GetHashCode()
        {
            return $"{objectPath},{bundleName}".GetHashCode();
        }
        public override string ToString()
        {
            return $"AssetPath: {objectPath} , BundleName: {bundleName}";
        }
    }
}
