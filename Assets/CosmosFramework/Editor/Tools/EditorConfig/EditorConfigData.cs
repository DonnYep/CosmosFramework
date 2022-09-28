using System;
namespace Cosmos.Editor.Config
{
    [Serializable]
    public class EditorConfigData 
    {
        public bool ConsoleDebugLog;
        public bool OutputDebugLog;
        public string LogOutputDirectory;
        public bool EnableScriptHeader;
        public string HeaderAuthor;
        public string AuthorMail;
    }
}