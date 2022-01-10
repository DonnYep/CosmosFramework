using Cosmos;
using System;
using System.IO;
using UnityEngine;
using static Cosmos.Utility.Debug;
using Object = UnityEngine.Object;
public class AndroidBuildDebugHelper : IDebugHelper
{
    readonly string logFullPath;

    /// <summary>
    /// 默认构造，使用默认地址与默认log名称
    /// </summary>
    public AndroidBuildDebugHelper()
    {
        logFullPath = Path.Combine(Application.persistentDataPath, "ClientLog.txt");
        Utility.IO.WriteTextFile(logFullPath, "Head");
        UnityEngine.Application.logMessageReceived += UnityLog;
    }
    public void LogInfo(object msg, object context)
    {
        if (context == null)
            Debug.Log($"<b><color={MessageColor.CYAN}>{"[INFO]-->>"} </color></b>{msg}");
        else
            Debug.Log($"<b><color={MessageColor.CYAN}>{"[INFO]-->>"}</color></b>{msg}", context as Object);
        string str = $"{DateTime.Now}[ - ] > INFO : {msg};{context}";
        Utility.IO.AppendWriteTextFile(logFullPath, str);
    }
    public void LogInfo(object msg, string msgColor, object context)
    {
        if (context == null)
            Debug.Log($"<b><color={msgColor }>{"[INFO]-->>"}</color></b>{msg}");
        else
            Debug.Log($"<b><color={msgColor }>{"[INFO]-->>"}</color></b>{msg}", context as Object);
        string str = $"{DateTime.Now}[ - ] > INFO : {msg};{context}";
        Utility.IO.AppendWriteTextFile(logFullPath, str);
    }
    public void LogError(object msg, object context)
    {
        if (context == null)
            Debug.LogError($"<b><color={MessageColor.RED}>{"[ERROR]-->>"} </color></b>{msg}");
        else
            Debug.LogError($"<b><color={MessageColor.RED}>{"[ERROR]-->>"}</color></b>{msg}", context as Object);
        string str = $"{DateTime.Now}[ - ] > ERROR :  {msg};{context}";
        Utility.IO.AppendWriteTextFile(logFullPath, str);
    }

    public void LogWarning(object msg, object context)
    {
        if (context == null)
            Debug.LogWarning($"<b><color={MessageColor.ORANGE}>{"[WARNING-->>" }</color></b>{msg}");
        else
            Debug.LogWarning($"<b><color={MessageColor.ORANGE}>{"WARNING]-->>" }</color></b>{msg}", context as Object);
        string str = $"{DateTime.Now}[ - ] > WARN : {msg};{context}";
        Utility.IO.AppendWriteTextFile(logFullPath, str);
    }
    public void LogFatal(object msg, object context)
    {
        if (context == null)
            Debug.LogError($"<b><color={MessageColor.RED}>{ "[FATAL]-->>" }</color></b>{msg}");
        else
            Debug.LogError($"<b><color={MessageColor.RED}>{ "[FATAL]-->>" }</color></b>{msg}", context as Object);
        string str = $"{DateTime.Now}[ - ] > FATAL : {msg};{context}";
        Utility.IO.AppendWriteTextFile(logFullPath, str);
    }
    void UnityLog(string msgStr, string stackTrace, LogType logType)
    {
    }
}


















