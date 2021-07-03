using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    public enum DownloaderMode:byte
    {
        UnityWebRequest=0,
        WebClient=1
    }
}
