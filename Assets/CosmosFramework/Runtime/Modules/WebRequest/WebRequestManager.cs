using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    /// <summary>
    /// 下载模块；
    /// </summary>
    //[Module]
    public class WebRequestManager : Module, IWebRequestManager
    {

        WebRequestMode currentWebRequestMode;
        public WebRequestMode CurrentWebRequestMode { get { return currentWebRequestMode; } }
        Dictionary<WebRequestMode, WebRequestChannel> builtInRequestChannelDict;
        EventHandler<WebRequestStartEventArgs> startCallback;
        EventHandler<WebRequestUpdateEventArgs> updateCallback;
        EventHandler<WebRequestSuccessEventArgs> successCallback;
        EventHandler<WebRequestFailureEventArgs> failureCallback;
        /// <summary>
        /// 请求开始回调；
        /// </summary>
        public EventHandler<WebRequestStartEventArgs> StartCallback { get { return startCallback; } }
        /// <summary>
        /// 请求执行中回调；
        /// </summary>
        public EventHandler<WebRequestUpdateEventArgs> UpdateCallback { get { return updateCallback; } }
        /// <summary>
        /// 请求成功回调；
        /// </summary>
        public EventHandler<WebRequestSuccessEventArgs> SuccessCallback { get { return successCallback; } }
        /// <summary>
        /// 请求失败回调；
        /// </summary>
        public EventHandler<WebRequestFailureEventArgs> FailureCallback { get { return failureCallback; } }
        public override void OnInitialization()
        {
            builtInRequestChannelDict = new Dictionary<WebRequestMode, WebRequestChannel>();
        }
    }
}
