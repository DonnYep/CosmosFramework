using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Cosmos.Download
{
    public interface IDownloadFileListHelper
    {
        /// <summary>
        /// 解析下载文件清单；
        /// </summary>
        /// <param name="context">传入的数据信息</param>
        /// <returns>文件清单数组</returns>
        string[] ParseDownloadFileList(string context);
    }
}
