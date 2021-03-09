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
        static readonly Vector2 wizardSize = new Vector2( 512, 368);
        [MenuItem("Cosmos/CFrameworkWizard")]
        public static void CosmosFrameworkWizardWindow()
        {
            OpenWindow();
        }
        public static void OpenWindow()
        {
            var window = GetWindow<CosmosFrameworkWizard>();
            ((EditorWindow)window).maxSize = wizardSize;
            ((EditorWindow)window).minSize = wizardSize;
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
                Application.OpenURL("https://github.com/DonHitYep/CosmosFramework");
            }
            EditorGUILayout.HelpBox("这是一个轻量级的游戏框架。Utiity脚本提供了常用的功能函数。" +
                "Facade脚本作为中间类，已封装了所有模块的公共函数，只需调用Facade.Instance，即可出现当前大部分模块的公共方法。", MessageType.Info);
            GUILayout.EndVertical();
            EditorGUILayout.Space(16);
            //GUILayout.BeginVertical("box");
            GUILayout.Label("框架中附带的Symbo: ");
            EditorGUILayout.Space(8);
            GUILayout.Label("Addressable资源操作模块 : COSMOS_ADDRESSABLE；\n在开启此Symbol后，可使用Addressables package进行资源加载");
            EditorGUILayout.Space(8);
            //GUILayout.EndVertical();
        }
    }
}
#endif
