using System.Collections;
using System.Collections.Generic;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
#endif
# if UNITY_EDITOR
namespace Cosmos.CosmosEditor
{
    [InitializeOnLoad]
    public class CFEditorInitialize
    {
        static CFEditorInitialize()
        {
            EditorApplication.delayCall += OnInitialization;
        }
        /// <summary>
        /// 初始化编辑函数，读取提前配置的环境
        /// </summary>
        static void OnInitialization()
        {
            if (!CFEditorUtility.HasEditorPrefsKey(EditorConst.CONSOLE_DEBUGLOG_KEY))
                CFEditorUtility.SetEditorPrefsBool(EditorConst.CONSOLE_DEBUGLOG_KEY, true);
            if (!CFEditorUtility.HasEditorPrefsKey(EditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY))
                CFEditorUtility.SetEditorPrefsBool(EditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY, false);
            if (!CFEditorUtility.HasEditorPrefsKey(EditorConst.OUTPUT_DEBUGLOG_KEY))
                CFEditorUtility.SetEditorPrefsBool(EditorConst.OUTPUT_DEBUGLOG_KEY, false);
            if (!CFEditorUtility.HasEditorPrefsKey(EditorConst.LOGOUTPUT_DIRECTORY_KEY))
                CFEditorUtility.SetEditorPresString(EditorConst.LOGOUTPUT_DIRECTORY_KEY, EditorConst.GetDefaultLogOutputDirectory());
            EditorConst.ConsoleDebugLog = CFEditorUtility.GetEditorPrefsBool(EditorConst.CONSOLE_DEBUGLOG_KEY);
            EditorConst.EnableScriptTemplateAnnotation = CFEditorUtility.GetEditorPrefsBool(EditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY);
            EditorConst.LogOutputDirectory = CFEditorUtility.GetEditorPrefsString(EditorConst.LOGOUTPUT_DIRECTORY_KEY);
        }
    }
}
#endif
