using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源加载通道接口；
    /// 此接口可用于适配多种资源加载模式，并可使其同时并存；
    /// </summary>
    public interface IResourceLoadChannel
    {
        string ChannelName { get; }
        IResourceLoadHelper ResourceLoadHelper { get; }
    }
}
