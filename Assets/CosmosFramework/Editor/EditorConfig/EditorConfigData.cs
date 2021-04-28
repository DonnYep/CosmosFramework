using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos.CosmosEditor
{
    [Serializable]
    public class EditorConfigData : IEditorData
    {
        public bool ConsoleDebugLog;
        public bool OutputDebugLog;
        public string LogOutputDirectory;
        public bool EnableScriptHeader;
        public string HeaderAuthor;

        public void Dispose()
        {
            ConsoleDebugLog = false;
            OutputDebugLog = false;
            LogOutputDirectory = null;
            EnableScriptHeader = false;
            HeaderAuthor = null;
        }
    }
}