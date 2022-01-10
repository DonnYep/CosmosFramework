namespace Cosmos.Resource
{
    /// <summary>
    /// 资源加载通道；
    /// </summary>
    public class ResourceLoadChannel 
    {
        readonly IResourceLoadHelper m_ResourceLoadHelper;
        readonly string m_ChannelName;
        public IResourceLoadHelper ResourceLoadHelper { get { return m_ResourceLoadHelper; } }
        public string ChannelName { get { return m_ChannelName; } }
        public ResourceLoadChannel(string channelName, IResourceLoadHelper resourceLoadHelper)
        {
            this.m_ChannelName= channelName;
            this.m_ResourceLoadHelper = resourceLoadHelper;
        }
    }
}