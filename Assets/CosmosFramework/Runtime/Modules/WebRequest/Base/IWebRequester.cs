using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.WebRequest
{
    /// <summary>
    /// Web请求器，可自定义实现下载方式；
    /// </summary>
    public class IWebRequester
    {
        public void DownloadAsync(string url,object customeData) { }
        public void UploadAsync(string url,byte[] data, object customeData) { }
    }
}
