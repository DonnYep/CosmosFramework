using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.WebRequest
{
    /// <summary>
    /// 下载通道；
    /// </summary>
    public class WebRequestChannel
    {
        readonly string channelName;
        readonly IWebRequester webRequester;
        string description;
        /// <summary>
        /// 通道名称；
        /// </summary>
        public string ChannelName { get { return channelName;  } }
        /// <summary>
        /// 请求器；
        /// </summary>
        public IWebRequester WebRequester { get { return webRequester; } }
        /// <summary>
        /// 通道描述；
        /// </summary>
        public string Description { get { return description; } set { description = value; } }
        public WebRequestChannel(string channelName, IWebRequester webRequester)
        {
            this.channelName= channelName;
            this. webRequester = webRequester;
        }
    }
}
