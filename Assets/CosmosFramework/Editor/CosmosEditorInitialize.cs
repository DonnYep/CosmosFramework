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
    public class CosmosEditorInitialize
    {
        static CosmosEditorInitialize()
        {
            EditorApplication.delayCall += OnInitialization;
        }
        /// <summary>
        /// 初始化编辑函数，读取提前配置的环境
        /// </summary>
        static void OnInitialization()
        {
            if (!CosmosEditorUtility.HasEditorPrefsKey(CosmosEditorConst.CONSOLE_DEBUGLOG_KEY))
                CosmosEditorUtility.SetEditorPrefsBool(CosmosEditorConst.CONSOLE_DEBUGLOG_KEY, true);
            if (!CosmosEditorUtility.HasEditorPrefsKey(CosmosEditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY))
                CosmosEditorUtility.SetEditorPrefsBool(CosmosEditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY, false);
            if (!CosmosEditorUtility.HasEditorPrefsKey(CosmosEditorConst.OUTPUT_DEBUGLOG_KEY))
                CosmosEditorUtility.SetEditorPrefsBool(CosmosEditorConst.OUTPUT_DEBUGLOG_KEY, false);
            if (!CosmosEditorUtility.HasEditorPrefsKey(CosmosEditorConst.LOGOUTPUT_DIRECTORY_KEY))
                CosmosEditorUtility.SetEditorPresString(CosmosEditorConst.LOGOUTPUT_DIRECTORY_KEY, CosmosEditorConst.GetDefaultLogOutputDirectory());
            CosmosEditorConst.ConsoleDebugLog= CosmosEditorUtility.GetEditorPrefsBool(CosmosEditorConst.CONSOLE_DEBUGLOG_KEY);
            CosmosEditorConst.EnableScriptTemplateAnnotation = CosmosEditorUtility.GetEditorPrefsBool(CosmosEditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY);
            CosmosEditorConst.LogOutputDirectory = CosmosEditorUtility.GetEditorPrefsString(CosmosEditorConst.LOGOUTPUT_DIRECTORY_KEY);
        }
    }
}
#endif
