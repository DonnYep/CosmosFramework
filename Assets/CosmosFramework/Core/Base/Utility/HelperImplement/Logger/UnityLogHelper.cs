using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Cosmos;
using UnityEngine;

namespace Cosmos
{
    public class UnityLoggerHelper : ILoggerHelper
    {
        readonly string logFullPath;
        readonly string logFileName = "CosmosFrameworkClient.log";
        readonly string logFolderName = "Log";
        readonly string defaultLogPath = Directory.GetParent(UnityEngine.Application.dataPath).FullName;
        /// <summary>
        /// 默认构造，使用默认地址与默认log名称
        /// </summary>
        public UnityLoggerHelper()
        {
            var path = defaultLogPath;
            logFullPath = Utility.IO.CombineRelativePath(path, logFolderName);
            Utility.IO.CreateFolder(logFullPath);
            UnityEngine.Application.logMessageReceived += UnityLog;
        }
        /// <summary>
        /// unitylog构造
        /// </summary>
        /// <param name="logName">无后缀文件名</param>
        /// <param name="logFullPath">log绝对路径</param>
        public UnityLoggerHelper(string logName, string logFullPath)
        {
            if (string.IsNullOrEmpty(logName))
                logName = logFileName;
            if (string.IsNullOrEmpty(logFullPath))
            {
                this.logFullPath = Utility.IO.CombineRelativePath(defaultLogPath, logFolderName);
            }
            else
                this.logFullPath = logFileName;
            Utility.IO.CreateFolder(this.logFullPath);
            UnityEngine.Application.logMessageReceived += UnityLog;
        }
        /// <summary>
        /// log构造
        /// </summary>
        /// <param name="logName">无后缀文件名</param>
        public UnityLoggerHelper(string logName)
        {
            if (string.IsNullOrEmpty(logName))
            {
                logName = logFileName;
            }
            if (logName.EndsWith(".log"))
            {
                logFileName = logName;
            }
            else
            {
                logFileName = Utility.Text.Append(logName, ".log");
            }
            Utility.Debug.LogInfo(logFileName);
            string path = Directory.GetParent(UnityEngine.Application.dataPath).FullName;
            logFullPath = Utility.IO.CombineRelativePath(path, logFolderName);
            Utility.IO.CreateFolder(logFullPath);
            UnityEngine.Application.logMessageReceived += UnityLog;
        }
        public void Error(Exception exception, string msg)
        {
            string str = $"{DateTime.Now.ToString()}[ - ] > ERROR : Exception Message : {exception?.Message} ；Exception line : {exception?.StackTrace}; Msg : {msg};";
            Utility.IO.AppendWriteTextFile(logFullPath, logFileName, str);
        }
        public void Info(string msg)
        {
            string str = $"{DateTime.Now.ToString()}[ - ] > INFO : {msg};";
            Utility.IO.AppendWriteTextFile(logFullPath, logFileName, str);
        }
        public void Warring(string msg)
        {
            string str = $"{DateTime.Now.ToString()}[ - ] > WARN : {msg};";
            Utility.IO.AppendWriteTextFile(logFullPath, logFileName, str);
        }
        public void Fatal(string msg)
        {
            string str = $"{DateTime.Now.ToString()}[ - ] > FATAL : {msg};";
            Utility.IO.AppendWriteTextFile(logFullPath, logFileName, str);
        }
        void UnityLog(string msgStr, string stackTrace, LogType logType)
        {
            var msg = DecodeRichText(msgStr);
            switch (logType)
            {
                case LogType.Error:
                    Error(null, $"{msg} ;\n{stackTrace}");
                    break;
                case LogType.Warning:
                    Warring($"{msg} ;\n{stackTrace}");
                    break;
                case LogType.Log:
                    Info($"{msg} ;\n{stackTrace}");
                    break;
                case LogType.Exception:
                    Error(null, $"{msg} ;\n{stackTrace}");
                    break;
            }
        }
        string DecodeRichText(string str)
        {
            if (!str.StartsWith("<b>-->>"))
                return str;
            var startStr = str.Substring(22);
            var endStr = startStr.TrimEnd("</color></b>".ToCharArray());
            return endStr;
        }
        /// <summary>
        /// 全局异常捕获器
        /// </summary>
        /// <param name="sender">异常抛出者</param>
        /// <param name="e">未被捕获的异常</param>
        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Utility.Debug.LogError(e);
        }
    }
}
