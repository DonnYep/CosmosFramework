using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Cosmos
{
    public static partial class Utility
    {
        public static class Net
        {
            /// <summary>
            /// 使用HttpWebRequest Ping获取url根目录的文件列表。
            /// <para> 远程资源尽量使用拉丁字母，此方法对于中文文件名解析会产生异常！</para>
            /// </summary>
            /// <param name="url">资源定位地址</param>
            /// <returns>目录字符串数组</returns>
            public static string[] PingUrlRootFiles(string url)
            {
                if (string.IsNullOrEmpty(url))
                    throw new ArgumentNullException("URL is invalid !");
                List<string> files = new List<string>();
                HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string html = reader.ReadToEnd();
                        GetUrlFiles(html, ref files);
                    }
                }
                return files.ToArray();
            }
            /// <summary>
            ///远程资源尽量使用英文字母命名。
            ///<para> 返回时只带File地址，不包含Folder</para>
            /// <para> 使用HttpWebRequest Ping并遍历url的文件列表 </para>
            /// </summary>
            /// <param name="url">资源定位地址</param>
            /// <param name="uris">返回的文件地址数组</param>
            public static void PingUrlFileList(string url, ref List<string> uris)
            {
                if (string.IsNullOrEmpty(url))
                    throw new ArgumentNullException("URL is invalid !");
                if (uris == null)
                    throw new ArgumentNullException("FileList is invalid !");
                HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string html = reader.ReadToEnd();
                        Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
                        MatchCollection matches = regex.Matches(html);
                        if (matches.Count <= 0)
                        {
                            return;
                        }
                        foreach (Match match in matches)
                        {
                            if (!match.Success)
                            {
                                continue;
                            }
                            var remoteUri = match.Groups["name"].ToString();
                            if (!remoteUri.EndsWith("../"))
                            {
                                var uriListPath = Utility.IO.CombineURL(url, remoteUri);
                                if (remoteUri.EndsWith("/"))
                                {
                                    PingUrlFileList(uriListPath, ref uris);
                                }
                                else
                                {
                                    uris.Add(uriListPath);
                                }
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Ping URL是否存在。
            /// <para> Ping的过程本身是阻塞的，谨慎使用！</para> 
            /// </summary>
            /// <param name="url">资源地址</param>
            /// <returns>是否存在</returns>
            public static bool PingURI(string url)
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            /// <summary>
            /// Get the  public IP Address of the computer
            /// </summary>
            /// <param name="timeoutMilliseconds">timeout</param>
            /// <returns>public IP Address</returns>
            public static string GetPublicIPAddress(int timeoutMilliseconds = 5000)
            {
                var client = new WebClient();
                var task = client.DownloadStringTaskAsync("http://icanhazip.com");
                if (task.Wait(timeoutMilliseconds) == false)
                    return string.Empty;
                return task.Result.Trim();
            }
            /// <summary>
            /// Get the IPv4 IP Address of the local computer
            /// </summary>
            /// <returns>IP Address</returns>
            public static string GetLocalIPv4Address()
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }
            /// <summary>
            /// Get the IPv6 IP Address of the local computer
            /// </summary>
            /// <returns>IP Address</returns>
            public static string GetILocalIPv6Address()
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        return ip.ToString();
                    }
                }
                throw new Exception("No network adapters with an IPv6 address in the system!");
            }
            /// <summary>
            /// 从web页面信息解析根目录下的文件信息。
            /// <para>若根目录下存在子目录，则子目录下的文件无法被解析</para>
            /// </summary>
            /// <param name="html">请求到的页面信息</param>
            /// <param name="files">解析到的首层文件信息</param>
            public static void GetUrlFiles(string html, ref List<string> files)
            {
                Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
                MatchCollection matches = regex.Matches(html);
                if (matches.Count <= 0)
                {
                    return;
                }
                foreach (Match match in matches)
                {
                    if (!match.Success)
                        continue;
                    var assetName = match.Groups["name"].ToString();
                    if (!assetName.EndsWith("../"))
                    {
                        files.Add(assetName);
                    }
                }
            }
            /// <summary>
            /// 是否是remote地址
            /// </summary>
            /// <param name="path">被检查的地址</param>
            /// <returns>是否是remote地址</returns>
            public static bool IsPathRemote(string path)
            {
                return path != null && path.StartsWith("http", StringComparison.Ordinal);
            }

            #region UnityWebRequest 公共判定（WebRequest / Download 共用）

            /// <summary>
            /// 请求是否成功（兼容 Unity 2020 前后两套 API）。
            /// <para>2020+ 用 <c>result == Success</c>：除了连接/协议错误，数据解析失败也算失败。</para>
            /// </summary>
            /// <param name="request">请求对象</param>
            /// <returns>是否成功</returns>
            public static bool IsRequestSucceeded(UnityEngine.Networking.UnityWebRequest request)
            {
                if (request == null)
                    return false;
#if UNITY_2020_1_OR_NEWER
                return request.result == UnityEngine.Networking.UnityWebRequest.Result.Success;
#else
                return !request.isNetworkError && !request.isHttpError;
#endif
            }
            /// <summary>
            /// 读取响应头 Content-Length。
            /// </summary>
            /// <param name="request">请求对象</param>
            /// <returns>字节数，缺失或非法返回 -1</returns>
            public static long GetContentLength(UnityEngine.Networking.UnityWebRequest request)
            {
                if (request == null)
                    return -1L;
                long length;
                return TryParseContentLength(request.GetResponseHeader("Content-Length"), out length) ? length : -1L;
            }
            /// <summary>
            /// 解析 Content-Length 头。
            /// </summary>
            /// <param name="header">头部值</param>
            /// <param name="length">字节数</param>
            /// <returns>是否解析成功</returns>
            public static bool TryParseContentLength(string header, out long length)
            {
                length = -1L;
                if (string.IsNullOrEmpty(header))
                    return false;
                long parsed;
                if (!long.TryParse(header.Trim(), out parsed) || parsed < 0L)
                    return false;
                length = parsed;
                return true;
            }
            /// <summary>
            /// 解析 Content-Range 头，形如 <c>bytes 100-999/1000</c>（总长可能是 <c>*</c>）。
            /// </summary>
            /// <param name="header">头部值</param>
            /// <param name="start">起始字节</param>
            /// <param name="end">结束字节</param>
            /// <param name="total">文件总长，未知为 -1</param>
            /// <returns>是否解析成功</returns>
            public static bool TryParseContentRange(string header, out long start, out long end, out long total)
            {
                start = -1L;
                end = -1L;
                total = -1L;
                if (string.IsNullOrEmpty(header))
                    return false;
                var text = header.Trim();
                if (text.StartsWith("bytes", StringComparison.OrdinalIgnoreCase))
                    text = text.Substring("bytes".Length).Trim();
                else
                    return false;
                var slashIndex = text.IndexOf('/');
                var rangePart = slashIndex >= 0 ? text.Substring(0, slashIndex) : text;
                if (slashIndex >= 0)
                {
                    var totalText = text.Substring(slashIndex + 1).Trim();
                    if (totalText != "*" && !(long.TryParse(totalText, out total) && total >= 0L))
                        total = -1L;
                }
                var dashIndex = rangePart.IndexOf('-');
                if (dashIndex < 0)
                    return false;
                if (!long.TryParse(rangePart.Substring(0, dashIndex).Trim(), out start) || start < 0L)
                {
                    start = -1L;
                    return false;
                }
                if (!long.TryParse(rangePart.Substring(dashIndex + 1).Trim(), out end) || end < start)
                {
                    end = -1L;
                    return false;
                }
                return true;
            }
            /// <summary>
            /// 本次响应体字节数：优先 Content-Length；缺失时用 Content-Range 推导（206 分段响应）。
            /// </summary>
            /// <param name="request">请求对象</param>
            /// <returns>字节数，未知返回 -1</returns>
            public static long GetTransferLength(UnityEngine.Networking.UnityWebRequest request)
            {
                var length = GetContentLength(request);
                if (length >= 0L)
                    return length;
                long start, end, total;
                if (request != null && TryParseContentRange(request.GetResponseHeader("Content-Range"), out start, out end, out total))
                    return end - start + 1L;
                return -1L;
            }
            #endregion
        }
    }
}
