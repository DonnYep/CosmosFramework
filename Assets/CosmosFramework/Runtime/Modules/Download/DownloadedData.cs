using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public struct DownloadedData
    {
        public DownloadedData(string uri,byte[] data, string downloadPath)
        {
            URI = uri;
            Data = data;
            DownloadPath = downloadPath;
        }
        public string URI { get; private set; }
        public byte[] Data { get; private set; }
        public string DownloadPath { get; private set; }
    }
}
