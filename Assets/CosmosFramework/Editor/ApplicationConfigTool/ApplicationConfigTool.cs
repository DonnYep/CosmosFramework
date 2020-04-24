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
            ((EditorWindow)window).maxSize = DebugTool.CosmosDevWinSize;
            ((EditorWindow)window).minSize = DebugTool.CosmosDevWinSize;
        }
        private void OnGUI()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            GUI.color = Color.green;
            EditorGUILayout.HelpBox("项目应用配置面板，用于Debug。", MessageType.None, true);
            GUI.color = Color.white;
            EditorGUILayout.Space();
            GUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("是否启用DebugLog。若开启，则显示debuglog信息，且内容都为富文本格式", MessageType.Info);
            ApplicationConst.Editor.EnableDebugLog = EditorGUILayout.Toggle("EnableDebugLog", ApplicationConst.Editor.EnableDebugLog);
            GUI.color = Color.white;
            EditorGUILayout.Space();
            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set", GUILayout.Height(32)))
            {
                SaveEditorPrefs();
            }
            GUILayout.Space(64);
            if (GUILayout.Button("Reset", GUILayout.Height(32)))
            {
                ResetEditorPrefs();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        void SaveEditorPrefs()
        {
            Utility.Editor.SetEditorPrefsBool(ApplicationConst.Editor.ENABLEDEBUGLOG_KEY, ApplicationConst.Editor.EnableDebugLog);
        }
        void ResetEditorPrefs()
        {
            ApplicationConst.Editor.EnableDebugLog = Utility.Editor.GetEditorPrefsBool(ApplicationConst.Editor.ENABLEDEBUGLOG_KEY);
        }
    }
}