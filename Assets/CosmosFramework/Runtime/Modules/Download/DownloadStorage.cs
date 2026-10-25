using System;
using System.IO;

namespace Cosmos.Download
{
    /// <summary>
    /// 下载文件落盘策略：
    /// <para>1、下载中的数据一律写入临时文件（目标路径 + 后缀），只有校验通过才移动到目标路径；</para>
    /// <para>2、临时文件在中断后保留，长度即断点偏移，因此"跨进程"的断点续传天然可用；</para>
    /// <para>3、移动前先删除目标文件，避免 File.Move 在目标存在时抛异常。</para>
    /// </summary>
    internal static class DownloadStorage
    {
        /// <summary>
        /// 计算临时文件路径
        /// </summary>
        /// <param name="savePath">目标路径</param>
        /// <param name="extension">临时文件后缀</param>
        /// <returns>临时文件路径</returns>
        public static string GetTempPath(string savePath, string extension)
        {
            return savePath + extension;
        }
        /// <summary>
        /// 取文件长度
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>字节数，文件不存在或读取失败时为0</returns>
        public static long GetLength(string path)
        {
            if (string.IsNullOrEmpty(path))
                return 0L;
            try
            {
                var fileInfo = new FileInfo(path);
                return fileInfo.Exists ? fileInfo.Length : 0L;
            }
            catch (Exception)
            {
                return 0L;
            }
        }
        /// <summary>
        /// 确保文件所在目录存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void EnsureDirectory(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
        /// <summary>
        /// 删除文件（不存在时忽略）
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>是否删除了文件</returns>
        public static bool Delete(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            try
            {
                if (!File.Exists(path))
                    return false;
                File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 把临时文件提交到目标路径（覆盖同名文件）
        /// </summary>
        /// <param name="tempPath">临时文件</param>
        /// <param name="savePath">目标文件</param>
        public static void Commit(string tempPath, string savePath)
        {
            EnsureDirectory(savePath);
            if (File.Exists(savePath))
                File.Delete(savePath);
            File.Move(tempPath, savePath);
        }
    }
}
