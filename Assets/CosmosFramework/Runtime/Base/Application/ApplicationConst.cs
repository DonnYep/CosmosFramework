using System.IO;
using UnityEngine;
namespace Cosmos
{
    public sealed partial class ApplicationConst
    {
        /// <summary>
        /// PlayerPrefs持久化前缀
        /// </summary>
        public const string APPPERFIX = "Cosmos";
        public const string CosmosFramework = "CosmosFramework";
        public static string LibraryPath
        {
            get
            {
                if (string.IsNullOrEmpty(libraryPath))
                {
                    var editorPath = new DirectoryInfo(Application.dataPath);
                    var rootPath = editorPath.Parent.FullName + "/Library/";
                    libraryPath = Utility.IO.CombineRelativePath(rootPath, CosmosFramework);
                }
                return libraryPath;
            }
        }
        static string libraryPath;
    }
}