namespace Cosmos.Resource
{
    /// <summary>
    /// 资产对象包装者
    /// </summary>
    public class ResourceObjectWarpper
    {
        public ResourceObjectWarpper(ResourceObject resourceObject)
        {
            ResourceObject = resourceObject;
        }
        public ResourceObject ResourceObject { get; set; }
        public int ReferenceCount { get; set; }
    }
}
