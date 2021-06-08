using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class WebRequestFailureEventArgs: GameEventArgs
    {
        /// <summary>
        /// Uniform Resource Identifier；
        /// </summary>
        public string Uri { get; private set; }
        /// <summary>
        /// 用户自定义的数据；
        /// </summary>
        public object CustomeData { get; private set; }
        public string ErrorMessage{ get; private set; }
        public override void Release()
        {
            CustomeData = null;
            Uri = null;
        }
        public static WebRequestFailureEventArgs Create(string uri, string errorMessage,object customeData=null)
        {
            var eventArgs = ReferencePool.Accquire<WebRequestFailureEventArgs>();
            eventArgs.CustomeData = customeData;
            eventArgs.Uri = uri;
            eventArgs.ErrorMessage = errorMessage;
            return eventArgs;
        }
        public static void Release(WebRequestFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
