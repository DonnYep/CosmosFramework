using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
namespace Cosmos
{
    public static partial class Utility
    {
        public static class IO
        {
            /// <summary>
            /// 持久化数据层路径，可写入
            /// </summary>
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
            /// <summary>
            /// 纯 .NET方法；
            /// 合并地址,返回相对路径；
            /// 参考示例：Resources\JsonData\
            /// </summary>
            /// <param name="relativePath">相对路径</param>
            /// <returns></returns>
            public static string CombineRelativePath(params string[] relativePath)
            {
                Utility.Text.ClearStringBuilder();
                for (int i = 0; i < relativePath.Length; i++)
                {
                    Utility.Text .StringBuilderCache.Append(relativePath[i] + "/");
                }
                return Utility.Text.StringBuilderCache.ToString();
            }
            /// <summary>
            /// 纯 .NET方法；
            /// 合并地址,返回相对路径；
            /// 参考示例：Resources\JsonData\CF.json
            /// </summary>
            /// <param name="fileFullName">文件的完整名称（包括文件扩展名）</param>
            /// <param name="relativePath">相对路径</param>
            /// <returns></returns>
            public static string CombineRelativeFilePath(string fileFullName, params string[] relativePath)
            {
                return Utility.IO. CombineRelativePath(relativePath) + fileFullName;
            }
        }
    }
}