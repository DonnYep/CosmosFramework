using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System;
using System.Security.Cryptography;

namespace Cosmos
{
    public static partial class Utility
    {
        public static class IO
        {
            /// <summary>
            /// 标准的UTF-8是不含BOM的；
            /// 构造的UTF8Encoding，排除掉UTF8-BOM的影响；
            /// </summary>
            static UTF8Encoding utf8Encoding = new UTF8Encoding(false);

            /// <summary>
            /// 获取文件夹中的文件数量；
            /// </summary>
            /// <param name="folderPath">文件夹路径</param>
            /// <returns>文件数量</returns>
            public static int FolderFileCount(string folderPath)
            {
                int count = 0;
                var files = Directory.GetFiles(folderPath); //String数组类型
                count += files.Length;
                var dirs = Directory.GetDirectories(folderPath);
                foreach (var dir in dirs)
                {
                    count += FolderFileCount(dir);
                }
                return count;
            }
            /// <summary>
            /// 遍历文件夹下的所有文件地址；
            /// </summary>
            /// <param name="folderPath">文件夹路径</param>
            /// <param name="handler">遍历到一个文件时的处理的函数</param>
            /// <exception cref="IOException">
            /// Folder path is invalid
            /// </exception>
            public static void TraverseFolderFilePath(string folderPath, Action<string> handler)
            {
                if (!Directory.Exists(folderPath))
                    throw new IOException("Folder path is invalid ! ");
                if (handler == null)
                    throw new ArgumentNullException("Handler is invalid !");
                var fileDirs = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
                foreach (var dir in fileDirs)
                {
                    handler.Invoke(dir);
                }
            }
            /// <summary>
            /// 遍历文件夹下的文件；
            /// </summary>
            /// <param name="folderPath">文件夹路径</param>
            /// <param name="handler">遍历到一个文件时的处理的函数</param>
            /// <exception cref="IOException">
            /// Folder path is invalid
            /// </exception>
            public static void TraverseFolderFile(string folderPath, Action<FileSystemInfo> handler)
            {
                DirectoryInfo d = new DirectoryInfo(folderPath);
                FileSystemInfo[] fsInfoArr = d.GetFileSystemInfos();
                foreach (FileSystemInfo fsInfo in fsInfoArr)
                {
                    if (fsInfo is DirectoryInfo)     //判断是否为文件夹
                    {
                        TraverseFolderFile(fsInfo.FullName, handler);//递归调用
                    }
                    else
                    {
                        handler(fsInfo);
                    }
                }
            }
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
                var fullPath = PathCombine(path, folderName);
                var dir = new DirectoryInfo(fullPath);
                if (!dir.Exists)
                {
                    dir.Create();
                    Utility.Debug.LogInfo("Path:" + path + "Folder is created ");
                }
            }
            /// <summary>
            /// 获取共同的路径；
            /// </summary>
            /// <param name="paths">传入的路径合集</param>
            /// <returns>共同的路径</returns>
            public static string CommonPath(string[] paths)
            {
                var firstPath = paths[0];
                bool isSame = true;
                int index = 0;
                string commonPath = string.Empty;
                while (isSame && index < firstPath.Length)
                {
                    for (int i = 1; i < paths.Length && isSame; i++)
                    {
                        isSame = firstPath[index] == paths[i][index];
                    }
                    if (isSame)
                        commonPath += firstPath[index];
                    index++;
                }
                return commonPath;
            }
            /// <summary>
            /// 标准 Windows 文件路径地址合并；
            /// 返回结果示例：Resources\JsonData\
            /// </summary>
            /// <param name="paths">路径params</param>
            /// <returns>合并的路径</returns>
            public static string PathCombine(params string[] paths)
            {
                var resultPath = Path.Combine(paths);
                resultPath = resultPath.Replace("/", "\\");
                return resultPath;
            }
            /// <summary>
            /// 合并路径；
            /// web类型地址合并， 获得的路径将以 / 作为分割符；
            /// 返回结果示例：github.com/DonnYep/CosmosFramework
            /// </summary>
            /// <param name="paths">路径</param>
            /// <returns>合并的路径</returns>
            public static string WebPathCombine(params string[] paths)
            {
                var pathResult = Path.Combine(paths);
                pathResult = pathResult.Replace("\\", "/");
                return pathResult;
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
            /// 拷贝文件夹；
            /// </summary>
            /// <param name="sourceDirName">原始文件夹</param>
            /// <param name="destDirName">目标文件夹</param>
            public static void DirectoryCopy(string sourceDirName, string destDirName)
            {
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);
                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceDirName);
                }
                DirectoryInfo[] dirs = dir.GetDirectories();
                Directory.CreateDirectory(destDirName);
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(tempPath, false);
                }
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath);
                }
            }
            public static void DeleteFile(string fileFullPath)
            {
                if (File.Exists(fileFullPath))
                {
                    File.Delete(fileFullPath);
                }
            }
            /// <summary>
            /// 重命名文件；
            /// 第一个参数需要：盘符+地址+文件名+后缀；
            /// 第二个参数仅需文件名+后缀名；
            /// </summary>
            /// <param name="oldFileFullPath">旧文件的完整路径，需要带后缀名</param>
            /// <param name="newFileNamewithExtension">新的文件名，仅需文件名+后缀名</param>
            public static void RenameFile(string oldFileFullPath, string newFileNamewithExtension)
            {
                if (!File.Exists(oldFileFullPath))
                {
                    using (FileStream fs = File.Create(oldFileFullPath)) { }
                }
                var dirPath = Path.GetDirectoryName(oldFileFullPath);
                var newFileName = Path.Combine(dirPath, newFileNamewithExtension);
                if (File.Exists(newFileName))
                    File.Delete(newFileName);
                File.Move(oldFileFullPath, newFileName);
            }
            /// <summary>
            /// 读取文件内容到byte数组，不作binary或者text转换；
            /// </summary>
            /// <param name="fileFullPath">文件的完整路径，包括后缀名等</param>
            /// <returns>读取到的文件byte数组</returns>
            public static byte[] ReadFile(string fileFullPath)
            {
                if (!File.Exists(fileFullPath))
                    throw new IOException("File full path is invalid ! ");
                return File.ReadAllBytes(fileFullPath);
            }
            /// <summary>
            /// 不适用Text类型！；
            /// 读取二进制文件，返回byte array；
            /// </summary>
            /// <param name="fileFullPath">文件的完整路径</param>
            /// <returns>文件被读取的二进制</returns>
            public static byte[] ReadBinaryFile(string fileFullPath)
            {
                if (!File.Exists(fileFullPath))
                    throw new IOException("ReadBinaryFile path not exist !" + fileFullPath);
                using (FileStream stream = File.Open(fileFullPath, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream, utf8Encoding))
                    {
                        return reader.ReadBytes((int)stream.Length);
                    }
                }
            }
            /// <summary>
            /// 读取指定路径下某text类型文件的内容
            /// </summary>
            /// <param name="fileFullPath">文件的完整路径，包含文件名与扩展名</param>
            /// <returns>指定文件的包含的内容</returns>
            public static string ReadTextFileContent(string fileFullPath)
            {
                if (!File.Exists(fileFullPath))
                    throw new IOException("ReadTextFileContent path not exist !" + fileFullPath);
                Utility.Text.ClearStringBuilder();
                using (FileStream stream = File.Open(fileFullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream, utf8Encoding))
                    {
                        Utility.Text.StringBuilderCache.Append(reader.ReadToEnd());
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
                    throw new IOException("ReadTextFileContent folder path not exist !" + folderPath);
                return ReadTextFileContent(Path.Combine(folderPath, fileName));
            }
            /// <summary>
            /// 使用UTF8编码；
            /// 追加写入文件信息；
            /// 若文件为空，则自动创建；
            /// 此方法为text类型文件写入；
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <param name="fileName">文件名</param>
            /// <param name="context">写入的信息</param>
            public static void AppendWriteTextFile(string filePath, string fileName, string context)
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                using (FileStream stream = new FileStream(Path.Combine(filePath, fileName), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    stream.Position = stream.Length;
                    using (StreamWriter writer = new StreamWriter(stream, utf8Encoding))
                    {
                        writer.WriteLine(context);
                        writer.Flush();
                    }
                }
            }
            /// <summary>
            /// 使用UTF8编码；
            /// 追加写入文件信息；
            /// 若文件为空，则自动创建；
            /// 此方法为text类型文件写入
            /// </summary>
            /// <param name="fileFullPath">文件完整路径</param>
            /// <param name="context">写入的信息</param>
            public static void AppendWriteTextFile(string fileFullPath, string context)
            {
                using (FileStream stream = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    stream.Position = stream.Length;
                    using (StreamWriter writer = new StreamWriter(stream, utf8Encoding))
                    {
                        writer.WriteLine(context);
                        writer.Flush();
                    }
                }
            }
            /// <summary>
            /// 使用UTF8编码；
            /// 写入文件信息；
            /// 若文件为空，则自动创建；
            /// 此方法为text类型文件写入；
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <param name="fileName">文件名</param>
            /// <param name="context">写入的信息</param>
            /// <param name="append">是否追加</param>
            public static void WriteTextFile(string filePath, string fileName, string context, bool append = false)
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                using (FileStream stream = File.Open(Path.Combine(filePath, fileName), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    if (append)
                        stream.Position = stream.Length;
                    using (StreamWriter writer = new StreamWriter(stream, utf8Encoding))
                    {
                        writer.WriteLine(context);
                        writer.Flush();
                    }
                }
            }
            /// <summary>
            /// 使用UTF8编码；
            /// 写入文件信息；
            /// 若文件为空，则自动创建；
            /// 此方法为text类型文件写入；
            /// </summary>
            /// <param name="fileFullPath">文件完整路径</param>
            /// <param name="context">写入的信息</param>
            /// <param name="append">是否追加</param>
            public static void WriteTextFile(string fileFullPath, string context, bool append = false)
            {
                using (FileStream stream = File.Open(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    if (append)
                        stream.Position = stream.Length;
                    using (StreamWriter writer = new StreamWriter(stream, utf8Encoding))
                    {
                        writer.WriteLine(context);
                        writer.Flush();
                    }
                }
            }
            /// <summary>
            /// 不适用Text类型！；
            /// 写入二进制类型文件；
            /// </summary>
            /// <param name="context">文件内容</param>
            /// <param name="fileFullPath">文件完整路径，带后缀名</param>
            public static void WriteBinaryFile(byte[] context, string fileFullPath)
            {
                using (FileStream stream = File.Open(fileFullPath, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(context);
                        writer.Flush();
                    }
                }
            }
            /// <summary>
            /// 将byte数组写成文件，不作binary或者text转换；
            /// 支持覆写；
            /// </summary>
            /// <param name="context">需要写入的数据byte数组</param>
            /// <param name="fileFullPath">文件的完整路径，包括后缀名等</param>
            public static void WriteFile(byte[] context, string fileFullPath)
            {
                File.WriteAllBytes(fileFullPath, context);
            }
            /// <summary>
            /// 完全覆写；
            ///  使用UTF8编码；
            /// </summary>
            /// <param name="filePath">w文件路径</param>
            /// <param name="fileName">文件名</param>
            /// <param name="context">写入的信息</param>
            public static void OverwriteTextFile(string filePath, string fileName, string context)
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                var fileFullPath = Path.Combine(filePath, fileName);
                using (FileStream stream = File.Open(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.SetLength(0);
                    using (StreamWriter writer = new StreamWriter(stream, utf8Encoding))
                    {
                        writer.WriteLine(context);
                        writer.Flush();
                    }
                }
            }
            /// <summary>
            /// 完全覆写；
            ///  使用UTF8编码；
            /// </summary>
            /// <param name="fileFullPath">文件完整路径</param>
            /// <param name="context">写入的信息</param>
            public static void OverwriteTextFile(string fileFullPath, string context)
            {
                using (FileStream stream = File.Open(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.SetLength(0);
                    using (StreamWriter writer = new StreamWriter(stream, utf8Encoding))
                    {
                        writer.WriteLine(context);
                        writer.Flush();
                    }
                }
            }
            /// <summary>
            /// 写入二进制
            /// </summary>
            /// <param name="fileFullPath">完整文件路径，带后缀名</param>
            /// <param name="context">内容</param>
            /// <returns>是否写入成功</returns>
            public static bool WriterFormattedBinary(string fileFullPath, object context)
            {
                if (!File.Exists(fileFullPath))
                    File.Create(fileFullPath);
                using (FileStream stream = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, context);
                    return true;
                }
            }
            /// <summary>
            /// 写入二进制；
            /// 传入的路径必为 ：{ Asset\Core\ } 格式
            /// </summary>
            /// <param name="filePath">文件夹路径</param>
            /// <param name="fileName">带后缀的文件名</param>
            /// <param name="context">内容</param>
            /// <returns>是否写入成功</returns>
            public static bool WriterFormattedBinary(string filePath, string fileName, object context)
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                var fullFilePath = Path.Combine(filePath, fileName);
                using (FileStream stream = new FileStream(fullFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
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