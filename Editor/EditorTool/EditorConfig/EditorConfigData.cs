﻿using System;
namespace Cosmos.Editor.Config
{
    [Serializable]
    public class EditorConfigData 
    {
        public bool ConsoleDebugLog { get; set; }
        public bool OutputDebugLog { get; set; }
        public string LogOutputDirectory { get; set; }
        public bool EnableScriptHeader { get; set; }
        public string HeaderAuthor { get; set; }
    }
}