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
        readonly string defaultLogPath = Directory.GetParent(UnityEngine.Application.dataPath).FullName;
        /// <summary>
        /// UnityDebugHelper无参构造，不输出log信息到log文件；
        /// </summary>
        public UnityDebugHelper() { }
        /// <summary>
        /// UnityDebugHelper构造；
        /// </summary>
        /// <param name="logFullPath">log输出的完整路径</param>
        public UnityDebugHelper(string logFullPath)
        {
            if (string.IsNullOrEmpty(logFullPath))
                throw new ArgumentNullException("LogFullPath is invalid !");
            this.logFullPath = logFullPath;
            Utility.IO.WriteTextFile(logFullPath, "Head");
            UnityEngine.Application.logMessageReceived += UnityLog;
        }
        public void LogInfo(object msg, object context)
        {
            if (context == null)
                Debug.Log($"<b><color={MessageColor.CYAN}>{"[INFO]-->>"} </color></b>{msg}");
            else
                Debug.Log($"<b><color={MessageColor.CYAN}>{"[INFO]-->>"}</color></b>{msg}", context as Object);
        }
        public void LogInfo(object msg, string msgColor, object context)
        {
            if (context == null)
                Debug.Log($"<b><color={msgColor }>{"[INFO]-->>"}</color></b>{msg}");
            else
                Debug.Log($"<b><color={msgColor }>{"[INFO]-->>"}</color></b>{msg}", context as Object);
        }
        public void LogError(object msg, object context)
        {
            if (context == null)
                Debug.LogError($"<b><color={MessageColor.RED}>{"[ERROR]-->>"} </color></b>{msg}");
            else
                Debug.LogError($"<b><color={MessageColor.RED}>{"[ERROR]-->>"}</color></b>{msg}", context as Object);
        }

        public void LogWarning(object msg, object context)
        {
            if (context == null)
                Debug.LogWarning($"<b><color={MessageColor.ORANGE}>{"[WARNING]-->>" }</color></b>{msg}");
            else
                Debug.LogWarning($"<b><color={MessageColor.ORANGE}>{"[WARNING]-->>" }</color></b>{msg}", context as Object);
        }
        public void LogFatal(object msg, object context)
        {
            if (context == null)
                Debug.LogError($"<b><color={MessageColor.RED}>{ "[FATAL]-->>" }</color></b>{msg}");
            else
                Debug.LogError($"<b><color={MessageColor.RED}>{ "[FATAL]-->>" }</color></b>{msg}", context as Object);
        }
        void UnityLog(string msgStr, string stackTrace, LogType logType)
        {
            string str = null;
            string[] splitedStr = null;
            try
            {
                splitedStr = Utility.Text.StringSplit(msgStr, new string[] { "</color></b>" });
            }
            catch { }
            switch (logType)
            {
                case LogType.Error:
                    {
                        if (splitedStr.Length > 1)
                            str = $"{DateTime.Now}[ - ] > ERROR : {splitedStr[1]};{stackTrace}";
                        else
                            str = $"{DateTime.Now}[ - ] > ERROR : {msgStr};{stackTrace}";
                    }
                    break;
                case LogType.Assert:
                    {
                        str = $"{DateTime.Now}[ - ] > ASSERT : {msgStr};{stackTrace}";
                    }
                    break;
                case LogType.Warning:
                    {
                        if (splitedStr.Length > 1)
                            str = $"{DateTime.Now}[ - ] > WARN : {splitedStr[1]};{stackTrace}";
                        else
                            str = $"{DateTime.Now}[ - ] > WARN : {msgStr};{stackTrace}";
                    }
                    break;
                case LogType.Log:
                    {
                        if (splitedStr.Length > 1)
                            str = $"{DateTime.Now}[ - ] > INFO : {splitedStr[1]};{stackTrace}";
                        else
                            str = $"{DateTime.Now}[ - ] > INFO : {msgStr};{stackTrace}";
                    }
                    break;
                case LogType.Exception:
                    {
                        str = $"{DateTime.Now}[ - ] > EXCEPTION : {msgStr};{stackTrace}";
                    }
                    break;
            }
            Utility.IO.AppendWriteTextFile(logFullPath, logFileName, str);
        }
    }
}