using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Resource;
# if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cosmos.Editor
{
    public class DebugTool :EditorWindow
    {
        //[MenuItem("Cosmos/DebugTool")]
        public static void CosmosDevToolWindow()
        {
            OpenCosmosDevWindow();
        }
        public static void OpenCosmosDevWindow()
        {
            var window = GetWindow<DebugTool>();
            ((EditorWindow)window).maxSize = CosmosDevWinSize;
            ((EditorWindow)window).minSize = CosmosDevWinSize;
        }
        static readonly Vector2 cosmosDevWinSize = new Vector2(512f, 768f);
       public static Vector2 CosmosDevWinSize { get { return cosmosDevWinSize; } }
        DebugTool()
        {
            this.titleContent = new GUIContent("DebugTool");
        }
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            if (GUILayout.Button("DebugLog", GUILayout.Height(24),GUILayout.Width(92)))
            {
                DebugLogBuilder.OpenWindow();
            }
            if (GUILayout.Button("ResourceBuild", GUILayout.Height(24), GUILayout.Width(92)))
            {
                JsonXmlTool.OpenWindow();
            }
            if (GUILayout.Button("RefereceTool", GUILayout.Height(24), GUILayout.Width(92)))
            {
                ReferenceTool.OpenWindow();
            }
            GUILayout.EndHorizontal();
        }
    }
}