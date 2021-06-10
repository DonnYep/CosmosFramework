using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Cosmos.Download
{
    public class WebClientDownloadAgentHelper//:IDownloadAgentHelper
    {
        //public FutureTask DownloadFileAsync(string uri, DownloadCallback downloadCallback)
        //{
        //    using (WebClient webClient = new WebClient())
        //    {
        //        try
        //        {
        //            downloadCallback.StartCallback?.Invoke();
        //            var task = webClient.DownloadDataTaskAsync(uri);
        //            webClient.DownloadProgressChanged += (sender, eventArgs) =>
        //            {
        //                var progress = eventArgs.ProgressPercentage;
        //                var percentage = (float)progress / 100f;
        //                downloadCallback.UpdateCallback.Invoke(percentage);
        //            };
        //            webClient.DownloadDataCompleted += (sender, eventArgs) =>
        //            {
        //                downloadCallback.SuccessCallback.Invoke(eventArgs.Result);
        //            };
        //            var futureTask = FutureTask.Create(task);
        //            return futureTask;
        //        }
        //        catch (Exception exception)
        //        {
        //            downloadCallback.FailureCallback(exception.ToString());
        //            throw exception;
        //        }
        //    }
        //}
    }
}
