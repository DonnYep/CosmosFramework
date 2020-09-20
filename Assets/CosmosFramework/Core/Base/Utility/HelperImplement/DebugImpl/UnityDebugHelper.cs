using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Cosmos
{
    public class UnityDebugHelper : IDebugHelper
    {
        readonly string logFullPath;
        readonly string logFileName = "CosmosFrameworkClient.log";
        readonly string logFolderName = "Log";
        readonly string defaultLogPath = Directory.GetParent(UnityEngine.Application.dataPath).FullName;

        /// <summary>
        /// 默认构造，使用默认地址与默认log名称
        /// </summary>
        public UnityDebugHelper()
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
        public UnityDebugHelper(string logName, string logFullPath)
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
        public UnityDebugHelper(string logName)
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
            string path = Directory.GetParent(UnityEngine.Application.dataPath).FullName;
            logFullPath = Utility.IO.CombineRelativePath(path, logFolderName);
            Utility.IO.CreateFolder(logFullPath);
            UnityEngine.Application.logMessageReceived += UnityLog;
        }
        public void LogInfo(object msg, object context)
        {
            if (context == null)
                Debug.Log("<b>-->><color=" + MessageColor.BLUE + ">" + msg + "</color></b>");
            else
                Debug.Log("<b>-->><color=" + MessageColor.BLUE + ">" + msg + "</color></b>", context as Object);
            string str = $"{DateTime.Now.ToString()}[ - ] > INFO : {msg};{context};";
            Utility.IO.AppendWriteTextFile(logFullPath, logFileName, str);
        }
        public void LogInfo(object msg, string msgColor, object context)
        {
            if (context == null)
                Debug.Log("<b>-->><color=" + msgColor + ">" + msg + "</color></b>");
            else
                Debug.Log("<b>-->><color=" + msgColor + ">" + msg + "</color></b>", context as Object);
            string str = $"{DateTime.Now.ToString()}[ - ] > INFO : {msg};{context};";
            Utility.IO.AppendWriteTextFile(logFullPath, logFileName, str);
        }
        public void LogError(object msg, object context)
        {
            if (context == null)
                Debug.LogError("<b>-->><color=#FF0000>" + msg + "</color></b>");
            else
                Debug.LogError("<b>-->><color=#FF0000>" + msg + "</color></b>", context as Object);
            string str = $"{DateTime.Now.ToString()}[ - ] > ERROR :  {msg};{context};";
            Utility.IO.AppendWriteTextFile(logFullPath, logFileName, str);
        }

        public void LogWarning(object msg, object context)
        {
            if (context == null)
                Debug.LogWarning("<b>-->><color=#FF5E00>" + msg + "</color></b>");
            else
                Debug.LogWarning("<b>-->><color=#FF5E00>" + msg + "</color></b>", context as Object);
            string str = $"{DateTime.Now.ToString()}[ - ] > WARN : {msg};{context};;";
            Utility.IO.AppendWriteTextFile(logFullPath, logFileName, str);
        }
        public void LogFatal(object msg, object context)
        {
            if (context == null)
                Debug.LogError("<b>-->><color=#FF5E00>" + msg + "</color></b>");
            else
                Debug.LogError("<b>-->><color=#FF5E00>" + msg + "</color></b>", context as Object);
            string str = $"{DateTime.Now.ToString()}[ - ] > FATAL : {msg};{context};";
            Utility.IO.AppendWriteTextFile(logFullPath, logFileName, str);
        }
        void UnityLog(string msgStr, string stackTrace, LogType logType)
        {
        }
    }
}