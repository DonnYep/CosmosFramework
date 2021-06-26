using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    internal struct DownloadedData
    {
        public DownloadedData(byte[] data, string downloadPath)
        {
            Data = data;
            DownloadPath = downloadPath;
        }
        public byte[] Data { get; private set; }
        public string DownloadPath { get; private set; }
    }
}
