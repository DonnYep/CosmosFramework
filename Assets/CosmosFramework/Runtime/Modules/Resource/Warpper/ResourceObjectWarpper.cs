namespace Cosmos.Resource
{
    /// <summary>
    /// 资产对象包装者
    /// </summary>
    public class ResourceObjectWarpper
    {
        int referenceCount;
        ResourceObject resourceObject;
        public ResourceObjectWarpper(ResourceObject resourceObject)
        {
            this.resourceObject = resourceObject;
        }
        public ResourceObject ResourceObject { get { return resourceObject; } }
        public int ReferenceCount
        {
            get { return referenceCount; }
            set
            {
                referenceCount = value;
                if (referenceCount < 0)
                    referenceCount = 0;
            }
        }
    }
}
