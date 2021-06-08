using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    public class WebRequestSuccessEventArgs : GameEventArgs
    {
        /// <summary>
        /// Uniform Resource Identifier；
        /// </summary>
        public string Uri { get; private set; }
        /// <summary>
        /// web回应的比特数据；
        /// </summary>
        public byte[] WebResponseBytes { get; private set; }
        public object CustomeData { get; private set; }
        public override void Release()
        {
            CustomeData = null;
            Uri = null;
        }
        public static WebRequestSuccessEventArgs Create(string uri, byte[] responseBytes, object customeData = null)
        {
            var eventArgs = ReferencePool.Accquire<WebRequestSuccessEventArgs>();
            eventArgs.CustomeData = customeData;
            eventArgs.WebResponseBytes = responseBytes;
            eventArgs.Uri = uri;
            return eventArgs;
        }
        public static void Release(WebRequestSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
