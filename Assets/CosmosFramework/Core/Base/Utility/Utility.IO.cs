using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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
                    Utility.Debug.LogInfo("Path:" + path + "Folder is created");
                }
            }
            public static void CreateFolder(string path, string folderName)
            {
                var fullPath = CombineRelativePath(path, folderName);
                var dir = new DirectoryInfo(fullPath);
                if (!dir.Exists)
                {
                    dir.Create();
                    Utility.Debug.LogInfo("Path:" + path + "Folder is created ");
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
                    Utility.Text.StringBuilderCache.Append(relativePath[i] + "/");
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
                return Utility.IO.CombineRelativePath(relativePath) + fileFullName;
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
            /// <summary>
            /// 读取指定路径下某text类型文件的内容
            /// </summary>
            /// <param name="fullFilePath">文件的完整路径，包含文件名与扩展名</param>
            /// <returns>指定文件的包含的内容</returns>
            public static string ReadTextFileContent(string fullFilePath)
            {
                if (!File.Exists(fullFilePath))
                    Utility.Debug.LogError(new IOException("ReadTextFileContent path not exist !" + fullFilePath));
                Utility.Text.ClearStringBuilder();

                using (FileStream stream = File.Open(fullFilePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        Utility.Text.StringBuilderCache.Append(reader.ReadToEnd());
                        reader.Close();
                        stream.Close();
                    }
                }
                return Utility.Text.StringBuilderCache.ToString();
            }
            /// <summary>
            /// 读取指定路径下某text类型文件的内容
            /// </summary>
            /// <param name="folderPath">文件夹路径</param>
            /// <param name="fileName">文件名称，包含文件名与扩展名</param>
            /// <returns>指定文件的包含的内容</returns>
            public static string ReadTextFileContent(string folderPath, string fileName)
            {
                if (!Directory.Exists(folderPath))
                    Utility.Debug.LogError(new IOException("ReadTextFileContent folder path not exist !" + folderPath));
                return ReadTextFileContent(Utility.IO.CombineRelativeFilePath(fileName, folderPath));
            }
            /// <summary>
            /// 使用UTF8编码；
            /// 追加写入文件信息；
            /// 若文件为空，则自动创建；
            /// 此方法为text类型文件写入；
            /// </summary>
            /// <param name="relativePath">相对路径</param>
            /// <param name="fileName">文件名</param>
            /// <param name="info">写入的信息</param>
            public static void AppendWriteTextFile(string relativePath, string fileName, string info)
            {
                string absoluteFullpath = Utility.IO.CombineRelativeFilePath(relativePath);
                if (!Directory.Exists(absoluteFullpath))
                    Directory.CreateDirectory(absoluteFullpath);
                using (FileStream stream = new FileStream(Utility.IO.CombineRelativeFilePath(fileName, absoluteFullpath), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    stream.Position = stream.Length;
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                    {
                        writer.WriteLine(info);
                        writer.Close();
                        stream.Close();
                    }
                }
            }
            /// <summary>
            /// 写入二进制
            /// </summary>
            /// <param name="fileFullPath">完整文件路径</param>
            /// <param name="context">内容</param>
            /// <returns>是否写入成功</returns>
            public static bool WriterFormattedBinary(string fileFullPath, object context)
            {
                if (!File.Exists(fileFullPath))
                    return false;
                using (FileStream stream = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, context);
                    return true;
                }
            }
            /// <summary>
            /// 读取二进制
            /// </summary>
            /// <param name="fileFullPath">完整文件路径</param>
            /// <returns>内容</returns>
            public static object ReadFormattedBinary(string fileFullPath)
            {
                if (!File.Exists(fileFullPath))
                    return null;
                using (FileStream stream = new FileStream(fileFullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return formatter.Deserialize(stream);
                }
            }
            /// <summary>
            /// 清空text类型的文本
            /// </summary>
            /// <param name="fileFullPath">完整文件路径</param>
            /// <returns>是否写入成功</returns>
            public static bool ClearTextContext(string fileFullPath)
            {
                if (!File.Exists(fileFullPath))
                    return false;
                File.WriteAllText(fileFullPath, string.Empty);
                return true;
            }
        }
    }
}