using System;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Log
{
    /// <summary>
    /// 异常处理模块
    /// </summary>
    public class LogManager : Module<LogManager>
    {
        /// <summary>
        /// 是否写输出日志
        /// </summary>
        public bool WriteLog { get; set; }
        ILogHelper logHelper;
        public void SetHelper(ILogHelper logHelper)
        {
            this.logHelper = logHelper;
        }
        public void Info(string msg)
        {
            logHelper.Info(msg);
        }
        public void Warring(string msg)
        {
            logHelper.Warring(msg);
        }
        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="msg">消息体</param>
        public void Error(Exception exception, string msg)
        {
            logHelper.Error(exception, msg);
        }
        public void Fatal(Exception exception, string msg)
        {
            logHelper.Fatal(msg);
        }
    }
}