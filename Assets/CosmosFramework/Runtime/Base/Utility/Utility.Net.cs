using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cosmos
{
    public static partial class Utility
    {
        public static class Net
        {
            /// <summary>
            /// 远程资源尽量使用英文字母命名；
            /// 使用HttpWebRequest Ping获取url根目录的文件列表；
            /// </summary>
            /// <param name="url">资源定位地址</param>
            /// <returns>目录字符串数组</returns>
            public static string[] PingUrlRootFiles(string url)
            {
                if (string.IsNullOrEmpty(url))
                    throw new ArgumentNullException("URL is invalid !");
                List<string> uris = new List<string>();
                HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string html = reader.ReadToEnd();
                        Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
                        MatchCollection matches = regex.Matches(html);
                        if (matches.Count > 0)
                        {
                            foreach (Match match in matches)
                            {
                                if (match.Success)
                                {
                                    var remoteUri = match.Groups["name"].ToString();
                                    if (!remoteUri.EndsWith("../"))
                                    {
                                        uris.Add(remoteUri);
                                    }
                                }
                            }
                        }
                    }
                }
                return uris.ToArray();
            }
            /// <summary>
            ///远程资源尽量使用英文字母命名；
            ///返回时只带File地址，不包含Folder；
            ///使用HttpWebRequest Ping并遍历url的文件列表；
            /// </summary>
            /// <param name="url">资源定位地址</param>
            /// <param name="fileList">返回的文件列表</param>
            public static void PingUrlFileList(string url, List<string> fileList)
            {
                if (string.IsNullOrEmpty(url))
                    throw new ArgumentNullException("URL is invalid !");
                if (fileList == null)
                    throw new ArgumentNullException("FileList is invalid !");
                HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string html = reader.ReadToEnd();
                        Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
                        MatchCollection matches = regex.Matches(html);
                        if (matches.Count > 0)
                        {
                            foreach (Match match in matches)
                            {
                                if (match.Success)
                                {
                                    var remoteUri = match.Groups["name"].ToString();
                                    if (!remoteUri.EndsWith("../"))
                                    {
                                        var uriListPath = Utility.IO.WebPathCombine(url, remoteUri);
                                        if (remoteUri.EndsWith("/"))
                                        {
                                            PingUrlFileList(uriListPath, fileList);
                                        }
                                        else
                                        {
                                            fileList.Add(uriListPath);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Ping URL是否存在；
            /// Ping的过程本身是阻塞的，谨慎使用！；
            /// </summary>
            /// <param name="url">资源地址</param>
            /// <returns>是否存在</returns>
            public static bool PingURI(string url)
            {
                using (HttpClient client = new HttpClient())
                {
                    var response =  client.GetAsync(url).Result;
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
        }
    }
}
