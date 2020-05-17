using System.Collections;
using System.Collections.Generic;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cosmos.CosmosEditor
{
    public class CosmosFrameworkWizard : EditorWindow
    {
        [MenuItem("Cosmos/CFrameworkWizard")]
        public static void CosmosFrameworkWizardWindow()
        {
            OpenWindow();
        }
        public static void OpenWindow()
        {
            var window = GetWindow<CosmosFrameworkWizard>();
            ((EditorWindow)window).maxSize = DebugTool.CosmosDevWinSize;
            ((EditorWindow)window).minSize = DebugTool.CosmosDevWinSize;
        }
        public CosmosFrameworkWizard()
        {
            this.titleContent = new GUIContent("CFWizard");
        }
        private void OnGUI()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.Space();
            GUI.color = Color.green;
            EditorGUILayout.HelpBox("CosmosFrameworkWizard", MessageType.None, true);
            GUI.color = Color.white;
            EditorGUILayout.Space();
            if (GUILayout.Button("访问源码", GUILayout.Height(32)))
            {
                Application.OpenURL("https://github.com/DonCFramework/DonCFramework");
            }
            EditorGUILayout.HelpBox("这是一个轻量级的游戏框架。Utiity脚本提供了常用的功能函数。" +
                "Facade脚本作为中间类，已封装了所有模块的公共函数，只需调用Facade.Instance，即可出现当前大部分模块的公共方法。", MessageType.Info);
            GUILayout.EndVertical();
        }
    }
}