using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
namespace Cosmos
{
    public sealed partial class Utility
    {
        public class IO
        {
            public static readonly string PathURL =
#if UNITY_ANDROID
        "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
        Application.dataPath + "/Raw/";  
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
        "file://" + Application.dataPath + "/StreamingAssets/";
#else
        string.Empty;  
#endif
            public static void CreateFolder(string path)
            {
                var dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.Create();
                    Utility.DebugLog("Path:" + path + "Folder Crated ");
                }
            }
        }
    }
}