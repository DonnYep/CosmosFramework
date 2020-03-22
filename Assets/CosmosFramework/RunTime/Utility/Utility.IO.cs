using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
namespace Cosmos
{
    public sealed partial  class Utility
    {
        public class IO
        {
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