using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class WebRequestStartEventArgs : GameEventArgs
    {
        /// <summary>
        /// Uniform Resource Identifier；
        /// </summary>
        public string Uri { get; private set; }
        /// <summary>
        /// 用户自定义的数据；
        /// </summary>
        public object CustomeData { get; private set; }
        public override void Release()
        {
            CustomeData = null;
            Uri = null;
        }
        public static WebRequestStartEventArgs Create(string uri, object customeData=null)
        {
            var eventArgs = ReferencePool.Accquire<WebRequestStartEventArgs>();
            eventArgs.CustomeData = customeData;
            eventArgs.Uri = uri;
            return eventArgs;
        }
        public static void Release(WebRequestStartEventArgs  eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
