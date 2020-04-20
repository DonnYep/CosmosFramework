using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// ApplicationConst的分部类，用于承载UnityEditor环境下的编辑器开发持久化数据
    /// 不会被编译到实际游戏中，仅仅开发使用
    /// </summary>
    public sealed partial class ApplicationConst
    {
        public class Editor
        {
            /// <summary>
            /// EditorPrefs持久化前缀
            /// </summary>
            public const string APPEDITORPERFIX = "CosmosEditor";
            /// <summary>
            /// enableDebugLog的EditorPrefs key
            /// </summary>
            public const string ENABLEDEBUGLOG_KEY = "EnableDebugLog_Key";
            /// <summary>
            ///OffLineMode的EditorPrefs key
            /// </summary>
            public const string OFFLINEMODE_KEY = "OffLineMode_Key";

            /// <summary>
            ///Log打印是否开启，默认开启
            /// </summary>
            static bool enableDebugLog = Utility.Editor.GetEditorPrefsBool(ApplicationConst.Editor.ENABLEDEBUGLOG_KEY);
            public static bool EnableDebugLog { get { return enableDebugLog; }set { enableDebugLog = value; } }
            /// <summary>
            /// 是否离线
            /// </summary>
            static bool offLineMode = Utility.Editor.GetEditorPrefsBool(ApplicationConst.Editor.OFFLINEMODE_KEY);
            public static bool OffLineMode { get { return offLineMode; }set { offLineMode = value; } }
        }
    }
}