﻿using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Cosmos
{
    public static partial class Utility
    {
        /// <summary>
        /// 文件压缩工具类
        /// </summary>
        public static class Compression
        {
            /// <summary>
            /// 压缩文本；
            /// </summary>
            /// <param name="context">文本内容</param>
            /// <returns>压缩后的文本</returns>
            public static string CompressString(string context)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(context);
                var memoryStream = new MemoryStream();
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }
                memoryStream.Position = 0;
                var compressedData = new byte[memoryStream.Length];
                memoryStream.Read(compressedData, 0, compressedData.Length);
                var gZipBuffer = new byte[compressedData.Length + 4];
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                return Convert.ToBase64String(gZipBuffer);
            }
            /// <summary>
            /// 解压文本；
            /// </summary>
            /// <param name="compressedContext">压缩过的文本</param>
            /// <returns>解压后的文本</returns>
            public static string DecompressString(string compressedContext)
            {
                byte[] gZipBuffer = Convert.FromBase64String(compressedContext);
                using (var memoryStream = new MemoryStream())
                {
                    int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                    memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
                    var buffer = new byte[dataLength];
                    memoryStream.Position = 0;
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        gZipStream.Read(buffer, 0, buffer.Length);
                    }
                    return Encoding.UTF8.GetString(buffer);
                }
            }
            /// <summary>
            /// 压缩bytes
            /// </summary>
            /// <param name="bytes">需要压缩的bytes</param>
            /// <returns>压缩后的bytes</returns>
            public static byte[] CompressBytes(byte[] bytes)
            {
                byte[] buffer = bytes;
                var memoryStream = new MemoryStream();
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }
                memoryStream.Position = 0;
                var compressedData = new byte[memoryStream.Length];
                memoryStream.Read(compressedData, 0, compressedData.Length);
                var gZipBuffer = new byte[compressedData.Length + 4];
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                return gZipBuffer;
            }
            /// <summary>
            /// 解压bytes；
            /// </summary>
            /// <param name="compressedBytes">压缩过的bytes</param>
            /// <returns>解压后的bytes</returns>
            public static byte[] DecompressBytes(byte[] compressedBytes)
            {
                byte[] gZipBuffer = compressedBytes;
                using (var memoryStream = new MemoryStream())
                {
                    int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                    memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
                    var buffer = new byte[dataLength];
                    memoryStream.Position = 0;
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        gZipStream.Read(buffer, 0, buffer.Length);
                    }
                    return buffer;
                }
            }
        }
    }
}
