using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace Cosmos
{
    public sealed partial class Utility
    {
        public static class IO
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
            /// <summary>
            /// 删除文件夹下的所有文件以及文件夹
            /// </summary>
            /// <param name="folderPath">文件夹路径</param>
            public static void DeleteFolder(string folderPath)
            {
                if (Directory.Exists(folderPath))
                {
                    DirectoryInfo directory = Directory.CreateDirectory(folderPath);
                    FileInfo[] files = directory.GetFiles();
                    foreach (var file in files)
                    {
                        file.Delete();
                    }
                    DirectoryInfo[] folders = directory.GetDirectories();
                    foreach (var folder in folders)
                    {
                        DeleteFolder(folder.FullName);
                    }
                    directory.Delete();
                }
            }
        }
    }
}