using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        static bool logPathExists = false;
        public EditorConfig()
        {
            this.titleContent = new GUIContent("EditorConfig");
        }
        [MenuItem("Cosmos/EditorConfig")]
        public static void ApplicationConfigWindow()
        {
            OpenWindow();
        }
        public static void OpenWindow()
        {
            var window = GetWindow<EditorConfig>();
            ((EditorWindow)window).maxSize = DebugTool.CosmosMaxWinSize;
            ((EditorWindow)window).minSize = DebugTool.CosmosDevWinSize;
        }
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
            CFEditorUtility.SetEditorPrefsBool(EditorConst.CONSOLE_DEBUGLOG_KEY, EditorConst.ConsoleDebugLog);
            CFEditorUtility.SetEditorPresString(EditorConst.LOGOUTPUT_DIRECTORY_KEY, EditorConst.LogOutputDirectory);
            CFEditorUtility.SetEditorPrefsBool(EditorConst.OUTPUT_DEBUGLOG_KEY, EditorConst.OutputDebugLog);

        }
        void ResetButtonClick()
        {
            EditorConst.ConsoleDebugLog = CFEditorUtility.GetEditorPrefsBool(EditorConst.CONSOLE_DEBUGLOG_KEY);
            EditorConst.EnableScriptTemplateAnnotation = CFEditorUtility.GetEditorPrefsBool(EditorConst.ENABLE_SCRIPTTEMPLATE_ANNOTATION_KEY);
            EditorConst.LogOutputDirectory = CFEditorUtility.GetEditorPrefsString(EditorConst.LOGOUTPUT_DIRECTORY_KEY);
            EditorConst.OutputDebugLog = CFEditorUtility.GetEditorPrefsBool(EditorConst.OUTPUT_DEBUGLOG_KEY);
        }
        #region ScriptTemplateAnnotation
        void DrawScriptTemplateAnnotation()
        {
            GUI.color = Color.white;
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("是否启用创建脚本注释。若开启，则在创建脚本时自动添加脚本创建注释", MessageType.Info);
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
            GUILayout.Label("ConsoleDebugLog", GUILayout.Width(192));
            GUILayout.Space(128);
            EditorConst.ConsoleDebugLog = EditorGUILayout.Toggle(EditorConst.ConsoleDebugLog);
            GUILayout.EndVertical();

            EditorGUILayout.HelpBox("是否输出Log日志", MessageType.Info);
            GUILayout.BeginHorizontal();
            GUILayout.Label("OutputDebugLog", GUILayout.Width(192));
            GUILayout.Space(128);
            EditorConst.OutputDebugLog = EditorGUILayout.Toggle(EditorConst.OutputDebugLog);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            if (EditorConst.OutputDebugLog)
            {
                EditorConst.LogOutputDirectory = EditorGUILayout.TextField("OutputPath", EditorConst.LogOutputDirectory);
                GUILayout.BeginHorizontal();
                GUILayout.Space(8);
                if (GUILayout.Button("路径检测", GUILayout.Height(32)))
                {
                    logPathExists = Directory.Exists(EditorConst.LogOutputDirectory);
                }
                GUILayout.Space(128);
                if (GUILayout.Button("设为默认", GUILayout.Height(32)))
                {
                    EditorConst.LogOutputDirectory = EditorConst.GetDefaultLogOutputDirectory();
                }
                GUILayout.Space(8);
                GUILayout.EndHorizontal();
                if (!logPathExists)
                {
                    EditorGUILayout.HelpBox("无效路径，请重新输入 ！", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("可用路径", MessageType.Info);
                }
            }
            GUILayout.EndVertical();
            GUI.color = Color.white;
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        #endregion
    }
}
#endif
