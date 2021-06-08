using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class WebRequestUpdateEventArgs : GameEventArgs
    {
        /// <summary>
        /// Uniform Resource Identifier；
        /// </summary>
        public string Uri { get; private set; }
        public float Progress { get; private set; }
        public object CustomeData { get; private set; }
        public override void Release()
        {
            CustomeData = null;
            Progress = 0;
        }
        public static WebRequestUpdateEventArgs Create(string uri, object customeData=null)
        {
            var eventArgs = ReferencePool.Accquire<WebRequestUpdateEventArgs>();
            eventArgs.CustomeData = customeData;
            eventArgs.Uri = uri;
            return eventArgs;
        }
        public static void  Update(float progress,WebRequestUpdateEventArgs eventArgs)
        {
            eventArgs.Progress = progress;
        }
    }
}
