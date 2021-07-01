using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    public class DownloadFinishEventArgs : GameEventArgs
    {
        public string[] SuccessURIs { get; private set; }
        public string[] FailureURIs { get; private set; }
        public object CustomeData { get; private set; }
        public override void Release()
        {
            SuccessURIs = null;
            FailureURIs = null;
            CustomeData = null;
        }
        public static DownloadFinishEventArgs Create(string[] successURIs , string[] failureURIs, object customeData)
        {
            var eventArgs = ReferencePool.Accquire<DownloadFinishEventArgs>();
            eventArgs.SuccessURIs= successURIs;
            eventArgs.FailureURIs= failureURIs;
            eventArgs.CustomeData = customeData;
            return eventArgs;
        }
        public static void Release(DownloadFinishEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
