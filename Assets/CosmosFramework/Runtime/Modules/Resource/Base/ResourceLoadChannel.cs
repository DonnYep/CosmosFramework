using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.Resource
{
    public class ResourceLoadChannel : IResourceLoadChannel
    {
        readonly byte m_ResourceLoadChannelId;
        readonly IResourceLoadHelper m_ResourceLoadHelper;
        public string Description { get; set; }
        public byte ResourceLoadChannelId{ get { return m_ResourceLoadChannelId; } }
        public IResourceLoadHelper ResourceLoadHelper { get { return m_ResourceLoadHelper; } }
        public ResourceLoadChannel(byte resourceLoadChannelId, IResourceLoadHelper resourceLoadHelper)
        {
            this.m_ResourceLoadChannelId = resourceLoadChannelId;
            this.m_ResourceLoadHelper = resourceLoadHelper;
        }
    }
}