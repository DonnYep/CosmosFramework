using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
# if UNITY_EDITOR
namespace Cosmos.CosmosEditor
{
    public class EditorConfigWindow : EditorWindow
    {
        static bool isDebugMode;
        static bool logPathExists = false;
        public static EditorConfigData EditorConfigData { get; private set; }
        [MenuItem("Window/Cosmos/EditorConfig")]
        public static void OpenWindow()
        {
            var window = GetWindow<EditorConfigWindow>();
            ((EditorWindow)window).maxSize = EditorUtil.CosmosMaxWinSize;
            ((EditorWindow)window).minSize = EditorUtil.CosmosDevWinSize;
        }
        [InitializeOnLoadMethod]
        public static void LoadData()
        {
            try
            {
                EditorConfigData = EditorUtil.GetData<EditorConfigData>(EditorConfigFileName);
            }
            catch
            {
                EditorConfigData = new EditorConfigData();
                EditorUtil.SaveData(EditorConfigFileName, EditorConfigData);
            }
        }
        static readonly string EditorConfigFileName = "EditorConfig.json";
        public EditorConfigWindow()
        {
            this.titleContent = new GUIContent("EditorConfig");
        }
        private void OnGUI()
        {
            DrawWindow();
        }
        private void OnDisable()
        {
            SaveEditorConfigData();
        }
        void DrawWindow()
        {
            EditorUtil.DrawVerticalContext(() =>
            {
                EditorGUILayout.Space();
                GUI.color = Color.green;
                EditorGUILayout.HelpBox("log日志输出。", MessageType.None, true);
                #region CustomDrawEditor
                DrawDebug();
                DrawScriptHeader();
                #endregion
                GUI.color = Color.white;
                EditorGUILayout.Space();
                EditorUtil.DrawHorizontalContext(() =>
                {
                    if (GUILayout.Button("Reset"))
                    {
                        ResetButtonClick();
                    }
                });
            });
        }
        void SaveEditorConfigData()
        {
            try
            {
                EditorUtil.SaveData(EditorConfigFileName, EditorConfigData == null ? new EditorConfigData() : EditorConfigData);
            }
            catch{}
        }
        void ResetButtonClick()
        {
            try
            {
                EditorConfigData =EditorUtil.GetData<EditorConfigData>(EditorConfigFileName);
                EditorUtil.Debug.LogInfo("重置 CosmosFramework EditorConfigData 成功");
            }
            catch (Exception e)
            {
                EditorUtil.Debug.LogError("重置 CosmosFramework EditorConfigData 失败: " + e);
            }
        }
        #region ScriptHeader
        void DrawScriptHeader()
        {
            GUI.color = Color.white;
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("是否启用创建脚本注释。若开启，则在创建脚本时自动添加脚本创建注释", MessageType.Info);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.Label("EnableScriptHeader", GUILayout.Width(192));
            GUILayout.Space(128);
            EditorConfigData.EnableScriptHeader= EditorGUILayout.Toggle(EditorConfigData.EnableScriptHeader);
            GUILayout.EndHorizontal();
            if (EditorConfigData.EnableScriptHeader)
            {
                GUILayout.Space(16);
                EditorGUILayout.LabelField("输入作者名称");
                GUILayout.BeginHorizontal();
                EditorConfigData.HeaderAuthor = EditorGUILayout.TextField("Author", EditorConfigData.HeaderAuthor);
                GUILayout.EndHorizontal();
            }
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
            EditorConfigData.ConsoleDebugLog = EditorGUILayout.Toggle(EditorConfigData.ConsoleDebugLog);
            GUILayout.EndVertical();

            EditorGUILayout.HelpBox("是否输出Log日志", MessageType.Info);
            GUILayout.BeginHorizontal();
            GUILayout.Label("OutputDebugLog", GUILayout.Width(192));
            GUILayout.Space(128);
            EditorConfigData.OutputDebugLog = EditorGUILayout.Toggle(EditorConfigData.OutputDebugLog);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            if (EditorConfigData.OutputDebugLog)
            {
                EditorConfigData.LogOutputDirectory = EditorGUILayout.TextField("OutputPath", EditorConfigData.LogOutputDirectory);
                GUILayout.BeginHorizontal();
                GUILayout.Space(8);
                if (GUILayout.Button("路径检测", GUILayout.Height(32)))
                {
                    logPathExists = Directory.Exists(EditorConfigData.LogOutputDirectory);
                }
                GUILayout.Space(128);
                if (GUILayout.Button("设为默认", GUILayout.Height(32)))
                {
                    EditorConfigData.LogOutputDirectory = EditorUtil.GetDefaultLogOutputDirectory();
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
