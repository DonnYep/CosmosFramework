namespace Cosmos.Resource
{
    /// <summary>
    /// 资源加载通道；
    /// </summary>
    internal class ResourceLoadChannel
    {
        readonly IResourceLoadHelper resourceLoadHelper;
        readonly ResourceLoadMode resourceLoadMode;
        public IResourceLoadHelper ResourceLoadHelper { get { return resourceLoadHelper; } }
        public ResourceLoadMode ResourceLoadMode { get { return resourceLoadMode; } }
        public ResourceLoadChannel(ResourceLoadMode resourceLoadMode, IResourceLoadHelper resourceLoadHelper)
        {
            this.resourceLoadMode = resourceLoadMode;
            this.resourceLoadHelper = resourceLoadHelper;
        }
    }
}