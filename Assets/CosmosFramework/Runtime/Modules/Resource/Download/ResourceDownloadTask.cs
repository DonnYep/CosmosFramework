using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.Resource
{
    public class ResourceDownloadTask
    {
        public long WebRequestTaskId { get; private set; }
        public long ResourceBundleSize { get; private set; }
        public long DownloadedBundleSize { get; private set; }
    }
}
