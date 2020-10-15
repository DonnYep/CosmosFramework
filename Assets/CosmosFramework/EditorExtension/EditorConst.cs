using System.IO;
#if UNITY_EDITOR
using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// ApplicationConst的分部类，用于承载UnityEditor环境下的编辑器开发持久化数据
    /// 不会被编译到实际游戏中，仅仅开发使用
    /// </summary>
    public sealed partial class EditorConst
    {
        /// <summary>
        /// EditorPrefs持久化前缀
        /// </summary>
        public const string APPEDITORPERFIX = "CosmosEditor";
        /// <summary>
        ///是否打印控制台debug信息
        /// </summary>
        public const string CONSOLE_DEBUGLOG_KEY = "EnableDebugLog_Key";
        /// <summary>
        /// 是否输出log日志的key
        /// </summary>
        public const string OUTPUT_DEBUGLOG_KEY = "OutputDebugLog_Key";
        /// <summary>
        /// 日志输出目录
        /// </summary>
        public const string LOGOUTPUT_DIRECTORY_KEY = "LogOutput_Directory_Key";
        /// <summary>
        /// 是否启用脚本模块注释的Key
        /// </summary>
        public const string ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY = "EnableScriptTemplateAnnotation_Key";
        /// <summary>
        /// 脚本模块注释作者的key；
        /// </summary>
        public const string ANNOTATION_AUTHOR_KEY = "AnnotationAuthor_Key";
        /// <summary>
        ///Log打印是否开启，默认开启
        /// </summary>
        public static bool ConsoleDebugLog { get { return consoleDebugLog; } set { consoleDebugLog = value; } }
        /// <summary>
        /// 是否输出log日志到具体的文件
        /// </summary>
        public static bool OutputDebugLog { get { return outputDebugLog; } set { outputDebugLog = value; } }
        /// <summary>
        /// 日志输出目录路径
        /// </summary>
        public static string LogOutputDirectory { get { return logOutputDirectory; } set { logOutputDirectory = value; } }
        /// <summary>
        /// 注释作者
        /// </summary>
        public static string AnnotationAuthor { get { return annotationAuthor; } set { annotationAuthor = value; } }
        /// <summary>
        /// 是否开启脚本注释，默认关闭
        /// </summary>
        public static bool EnableScriptTemplateAnnotation { get { return enableScriptTemplateAnnotation; } set { enableScriptTemplateAnnotation = value; } }
        static string logOutputDirectory = CFEditorUtility.GetEditorPrefsString(LOGOUTPUT_DIRECTORY_KEY);
        static bool outputDebugLog = CFEditorUtility.GetEditorPrefsBool(EditorConst.OUTPUT_DEBUGLOG_KEY);
        static bool enableScriptTemplateAnnotation = CFEditorUtility.GetEditorPrefsBool(EditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY);
        static bool consoleDebugLog = CFEditorUtility.GetEditorPrefsBool(EditorConst.CONSOLE_DEBUGLOG_KEY);
        static string annotationAuthor = CFEditorUtility.GetEditorPrefsString(EditorConst.ANNOTATION_AUTHOR_KEY);
        public static string GetDefaultLogOutputDirectory()
        {
            DirectoryInfo info = new DirectoryInfo(Application.dataPath);
            string path = info.Parent.FullName;
            return path;
        }
    }
}
#endif
