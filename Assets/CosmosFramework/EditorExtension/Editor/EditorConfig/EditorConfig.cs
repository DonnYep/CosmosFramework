using System.Collections;
using System.Collections.Generic;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
#endif
# if UNITY_EDITOR
namespace Cosmos.CosmosEditor
{
    public class EditorConfig : EditorWindow
    {
        static bool isDebugMode;
        public EditorConfig()
        {
            this.titleContent = new GUIContent("CFAppConfig");
        }
        [MenuItem("Cosmos/EditorConfig")]
        public static void  ApplicationConfigWindow()
        {
            OpenWindow();
        }
        public static void OpenWindow()
        {
            var window = GetWindow<EditorConfig>();
            ((EditorWindow)window).maxSize = DebugTool.CosmosMaxWinSize;
            ((EditorWindow)window).minSize = DebugTool.CosmosDevWinSize;
        }
        //TODO DrawScriptTemplateAnnotation
        private void OnGUI()
        {
            DrawWindow();
        }
        void DrawWindow()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            GUI.color = Color.green;
            EditorGUILayout.HelpBox("创建脚本注释。", MessageType.None, true);
            #region CustomDrawEditor
            DrawDebug();
            DrawScriptTemplateAnnotation();
            #endregion
            GUI.color = Color.white;
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            if (GUILayout.Button("Set", GUILayout.Height(32)))
            {
                SetButtonClick();
            }
            GUILayout.Space(128);
            if (GUILayout.Button("Reset", GUILayout.Height(32)))
            {
                ResetButtonClick();
            }
            GUILayout.Space(8);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        void SetButtonClick()
        {
            CFEditorUtility.SetEditorPrefsBool(EditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY, EditorConst.EnableScriptTemplateAnnotation);
            CFEditorUtility.SetEditorPrefsBool(EditorConst.ENABLE_DEBUGLOG_KEY, EditorConst.EnableDebugLog);
        }
        void ResetButtonClick()
        {
            EditorConst.EnableDebugLog = CFEditorUtility.GetEditorPrefsBool(EditorConst.ENABLE_DEBUGLOG_KEY);
            EditorConst.EnableScriptTemplateAnnotation = CFEditorUtility.GetEditorPrefsBool(EditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY);
        }
        #region ScriptTemplateAnnotation
        void DrawScriptTemplateAnnotation()
        {
            GUI.color = Color.white;
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("是否启用创建脚本注释。若开启，添加脚本创建注释", MessageType.Info);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.Label("EnableScriptTemplateAnnotation", GUILayout.Width(192));
            GUILayout.Space(128);
            EditorConst.EnableScriptTemplateAnnotation = EditorGUILayout.Toggle(EditorConst.EnableScriptTemplateAnnotation);
            GUILayout.EndHorizontal();
            GUI.color = Color.white;
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        #endregion
        #region Debug
        void DrawDebug()
        {
            GUI.color = Color.white;
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("是否启用DebugLog。若开启，则显示debuglog信息，且内容都为富文本格式", MessageType.Info);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.Label("EnableDebugLog",GUILayout.Width(192));
            GUILayout.Space(128);
            EditorConst.EnableDebugLog = EditorGUILayout.Toggle(EditorConst.EnableDebugLog);
            GUILayout.EndVertical();
            GUI.color = Color.white;
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        #endregion
    }
}
#endif
