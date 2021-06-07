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
        Dictionary<string, WebRequestChannel> requestChannelDict;
    }
}
