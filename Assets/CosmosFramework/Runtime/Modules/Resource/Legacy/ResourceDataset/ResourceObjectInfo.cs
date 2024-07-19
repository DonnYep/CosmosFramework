using System;
using UnityEngine;

namespace Cosmos.Resource
{
    [Serializable]
    public class ResourceObjectInfo : IEquatable<ResourceObjectInfo>
    {

        [SerializeField]
        long objectSize;
        [SerializeField]
        string objectFormatBytes;
        [SerializeField]
        bool objectValid;
        ResourceObject resourceObject;
        public ResourceObject ResourceObject
        {
            get { return resourceObject; }
        }
        public long ObjectSize
        {
            get { return objectSize; }
            set { objectSize = value; }
        }
        /// <summary>
        /// 资源比特大小。
        /// </summary>
        public string ObjectFormatBytes
        {
            get { return objectFormatBytes; }
            set { objectFormatBytes = value; }
        }
        public bool ObjectVaild
        {
            get { return objectValid; }
            set { objectValid = value; }
        }
        public ResourceObjectInfo() { }
        public ResourceObjectInfo(string objectName, string objectPath, string bundleName, string extension)
        {
            resourceObject = new ResourceObject()
            {
                ObjectName = objectName,
                ObjectPath = objectPath,
                BundleName = bundleName,
                Extension = extension,
            };
        }
        public bool Equals(ResourceObjectInfo other)
        {
            return other.ResourceObject == this.ResourceObject;
        }
        public override bool Equals(object obj)
        {
            return (obj is ResourceObjectInfo) && Equals((ResourceObjectInfo)obj);
        }
        public override int GetHashCode()
        {
            return $"{ResourceObject},{objectSize}".GetHashCode();
        }
        public override string ToString()
        {
            return ResourceObject.ToString();
        }
    }
}
