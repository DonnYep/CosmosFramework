using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.Resource
{
    public class ResourceLoadChannel : IResourceLoadChannel
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