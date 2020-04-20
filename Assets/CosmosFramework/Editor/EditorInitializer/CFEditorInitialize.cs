using System.Collections;
using System.Collections.Generic;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
#endif
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
            if (!Utility.Editor.HasEditorPrefsKey(ApplicationConst.Editor.ENABLEDEBUGLOG_KEY))
                Utility.Editor.SetEditorPrefsBool(ApplicationConst.Editor.ENABLEDEBUGLOG_KEY, true);
            if (!Utility.Editor.HasEditorPrefsKey(ApplicationConst.Editor.OFFLINEMODE_KEY))
                Utility.Editor.SetEditorPrefsBool(ApplicationConst.Editor.OFFLINEMODE_KEY, false);
            ApplicationConst.Editor.EnableDebugLog = Utility.Editor.GetEditorPrefsBool(ApplicationConst.Editor.ENABLEDEBUGLOG_KEY);
            ApplicationConst.Editor.OffLineMode = Utility.Editor.GetEditorPrefsBool(ApplicationConst.Editor.OFFLINEMODE_KEY);
        }
    }
}