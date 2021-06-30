using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            /// 获取url根目录的文件列表；
            /// </summary>
            /// <param name="url">资源定位地址</param>
            /// <returns>目录字符串数组</returns>
            public static string[] GetUrlRootFiles(string url)
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
                                    uris.Add(match.Groups["name"].ToString());
                                }
                            }
                        }
                    }
                }
                return uris.ToArray();
            }
            //public static void FTPServer(string url)
            //{

            //}
        }
    }
}
