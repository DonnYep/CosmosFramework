using System;
using UnityEngine;
using Cosmos;

namespace Cosmos.Resource
{
    [Serializable]
    public class ResourceObjectInfo : IEquatable<ResourceObjectInfo>
    {
        [SerializeField]
        string objectName;
        [SerializeField]
        string objectPath;
        [SerializeField]
        string bundleName;
        [SerializeField]
        long objectSize;
        [SerializeField]
        string objectFormatBytes;
        [SerializeField]
        string extension;
        [SerializeField]
        bool objectValid;
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
        public long ObjectSize
        {
            get { return objectSize; }
            set { objectSize = value; }
        }
        /// <summary>
        /// 资源比特大小；
        /// </summary>
        public string ObjectFormatBytes
        {
            get { return objectFormatBytes; }
            set { objectFormatBytes = value; }
        }
        /// <summary>
        /// 后缀名；
        /// </summary>
        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }
        public bool  ObjectVaild
        {
            get { return objectValid; }
            set { objectValid = value; }
        }
        public ResourceObjectInfo() { }
        public ResourceObjectInfo(string objectName, string objectPath, string bundleName, string extension)
        {
            this.objectName = objectName;
            this.objectPath = objectPath;
            this.bundleName = bundleName;
            this.extension = extension;
        }
        public bool Equals(ResourceObjectInfo other)
        {
            return other.Extension == this.Extension &&
                other.BundleName == this.BundleName &&
                other.ObjectName == this.ObjectName &&
                other.ObjectPath == this.ObjectPath;
        }
        public override bool Equals(object obj)
        {
            return (obj is ResourceObjectInfo) && Equals((ResourceObjectInfo)obj);
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
