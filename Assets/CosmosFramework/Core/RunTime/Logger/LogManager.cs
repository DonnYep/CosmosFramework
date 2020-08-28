using System;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Log
{
    /// <summary>
    /// 异常处理模块
    /// </summary>
    internal class LogManager : Module<LogManager>
    {
        /// <summary>
        /// 是否写输出日志
        /// </summary>
        public bool WriteLog { get; set; }
        ILogHelper logHelper;
        internal void SetHelper(ILogHelper logHelper)
        {
            this.logHelper = logHelper;
        }
        //TODO 全局异常处理模块，调用C# 与 Unity特性实现

        internal void Info(string msg)
        {
            logHelper.Info(msg);
        }
        internal void Warring(string msg)
        {
            logHelper.Warring(msg);
        }
        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="msg">消息体</param>
        internal void Error(Exception exception, string msg)
        {
            logHelper.Error(exception,msg);
        }
    }
}