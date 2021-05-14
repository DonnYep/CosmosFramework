using System.Collections;
using System.Collections.Generic;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
#endif
# if UNITY_EDITOR
namespace Cosmos.CosmosEditor
{
    public class CosmosFrameworkWizard : EditorWindow
    {
        static readonly Vector2 wizardSize = new Vector2(512, 368);
        [MenuItem("Window/Cosmos/Wizard")]
        public static void OpenWindow()
        {
            var window = GetWindow<CosmosFrameworkWizard>();
            ((EditorWindow)window).maxSize = wizardSize;
            ((EditorWindow)window).minSize = wizardSize;
        }
        public CosmosFrameworkWizard()
        {
            this.titleContent = new GUIContent("Wizard");
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
                Application.OpenURL("https://github.com/DonnYep/CosmosFramework");
            }
            EditorGUILayout.HelpBox("CosmosFramework是一款轻量级的游戏框架。内置常用工具类，封装了大部分常用功能模块，案例可参考 Examples 。", MessageType.Info);
            GUILayout.EndVertical();
            GUILayout.Space(16);
            //GUILayout.BeginVertical("box");
            //GUILayout.Label("框架中附带的Symbol: ");
            GUILayout.Space(8);
            //GUILayout.Label("Addressable资源操作模块 : COSMOS_ADDRESSABLE；\n在开启此Symbol后，可使用Addressables package进行资源加载");
            GUILayout.Space(8);
            //GUILayout.EndVertical();
        }
    }
}
#endif
