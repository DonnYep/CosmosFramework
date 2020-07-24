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
            if (!CFEditorUtility.HasEditorPrefsKey(EditorConst.ENABLE_DEBUGLOG_KEY))
                CFEditorUtility.SetEditorPrefsBool(EditorConst.ENABLE_DEBUGLOG_KEY, true);
            if (!CFEditorUtility.HasEditorPrefsKey(EditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY))
                CFEditorUtility.SetEditorPrefsBool(EditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY, false);
            EditorConst.EnableDebugLog = CFEditorUtility.GetEditorPrefsBool(EditorConst.ENABLE_DEBUGLOG_KEY);
            EditorConst.EnableScriptTemplateAnnotation = CFEditorUtility.GetEditorPrefsBool(EditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY);
        }
    }
}
#endif
