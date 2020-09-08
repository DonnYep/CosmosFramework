using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public sealed partial class Utility
    {
        public static class Logger
        {
            /// <summary>
            /// 是否写输出日志
            /// </summary>
            public static bool WriteLog { get; set; }
            static ILoggerHelper logHelper;
            public static void SetHelper(ILoggerHelper helper)
            {
                logHelper = helper;
            }
            public static void Info(string msg)
            {
                logHelper.Info(msg);
            }
            public static void Warring(string msg)
            {
                logHelper.Warring(msg);
            }
            /// <summary>
            /// 输出错误
            /// </summary>
            /// <param name="exception">异常</param>
            /// <param name="msg">消息体</param>
            public static void Error(Exception exception, string msg)
            {
                logHelper.Error(exception, msg);
            }
            public static void Fatal(Exception exception, string msg)
            {
                logHelper.Fatal(msg);
            }
        }
    }
}
