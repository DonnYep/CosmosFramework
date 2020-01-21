using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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