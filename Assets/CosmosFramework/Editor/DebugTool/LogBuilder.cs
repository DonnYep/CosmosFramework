using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_EDITOR
namespace Cosmos.CosmosEditor
{
    public class DebugLogBuilder : EditorWindow
    {
        public static void DebugLogBuildWindow()
        {
            OpenWindow();
        }
        DebugLogBuilder()
        {
            this.titleContent = new GUIContent("DebugLogBuilder");
        }
        public static void OpenWindow()
        {
            var window = GetWindow<DebugLogBuilder>();
            ((EditorWindow)window).maxSize = DebugTool.CosmosDevWinSize;
            ((EditorWindow)window).minSize = DebugTool.CosmosDevWinSize;
        }
    }
}
#endif
