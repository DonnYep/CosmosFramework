using System.Collections;
using System.Collections.Generic;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cosmos.CosmosEditor
{
    public class ApplicationConfigTool : EditorWindow
    {
        static bool isDebugMode;
        public ApplicationConfigTool()
        {
            this.titleContent = new GUIContent("CFAppConfig");
        }
        [MenuItem("Cosmos/ApplicationConfig")]
        public static void  ApplicationConfigWindow()
        {
            OpenWindow();
        }
        public static void OpenWindow()
        {
            var window = GetWindow<ApplicationConfigTool>();
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
            Utility.Editor.SetEditorPrefsBool(ApplicationConst.Editor.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY, ApplicationConst.Editor.EnableScriptTemplateAnnotation);
            Utility.Editor.SetEditorPrefsBool(ApplicationConst.Editor.ENABLE_DEBUGLOG_KEY, ApplicationConst.Editor.EnableDebugLog);
        }
        void ResetButtonClick()
        {
            ApplicationConst.Editor.EnableDebugLog = Utility.Editor.GetEditorPrefsBool(ApplicationConst.Editor.ENABLE_DEBUGLOG_KEY);
            ApplicationConst.Editor.EnableScriptTemplateAnnotation = Utility.Editor.GetEditorPrefsBool(ApplicationConst.Editor.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY);
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
            ApplicationConst.Editor.EnableScriptTemplateAnnotation = EditorGUILayout.Toggle(ApplicationConst.Editor.EnableScriptTemplateAnnotation);
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
            ApplicationConst.Editor.EnableDebugLog = EditorGUILayout.Toggle(ApplicationConst.Editor.EnableDebugLog);
            GUILayout.EndVertical();
            GUI.color = Color.white;
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        #endregion
    }
}