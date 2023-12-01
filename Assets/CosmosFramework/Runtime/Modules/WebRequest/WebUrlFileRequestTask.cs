using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityWebRequest = UnityEngine.Networking.UnityWebRequest;
namespace Cosmos.WebRequest
{
    internal class WebUrlFileRequestTask : IReference
    {
        readonly List<WebRequestUrlFileInfo> urlFileInfoList = new List<WebRequestUrlFileInfo>();
        /// <summary>
        /// 由于是递归查询，因此需要一个错误列表
        /// </summary>
        readonly List<string> errorMessageList = new List<string>();
        bool requestFailure;
        Action<long> onSuccess;
        Action<long> onFailure;
        public List<WebRequestUrlFileInfo> UrlFileInfoList
        {
            get { return urlFileInfoList; }
        }
        public List<string> ErrorMessageList
        {
            get { return errorMessageList; }
        }
        public WebRequestTask WebRequestTask { get; private set; }
        public TimeSpan TimeSpan { get; private set; }
        public Coroutine Coroutine { get; private set; }
        int urlStringLength = 0;
        public void StartRequestUrl(WebRequestTask webRequestTask, Action<long> onSuccessCallback, Action<long> onFailureCalback)
        {
            this.WebRequestTask = webRequestTask;
            Coroutine = Utility.Unity.StartCoroutine(RunRequestUrl());
            onSuccess = onSuccessCallback;
            onFailure = onFailureCalback;
            requestFailure = false;
            urlStringLength = Utility.Text.GetUTF8Length(WebRequestTask.URL);
        }
        IEnumerator RunRequestUrl()
        {
            var url = WebRequestTask.URL;
            var startTime = DateTime.Now;
            yield return RunRequest(url);
            var endTime = DateTime.Now;
            TimeSpan = endTime - startTime;
            if (requestFailure)
            {
                onFailure?.Invoke(WebRequestTask.TaskId);
            }
            else
            {
                onSuccess?.Invoke(WebRequestTask.TaskId);
            }
        }
        IEnumerator RunRequest(string dstUrl)
        {
            UnityWebRequest request = UnityWebRequest.Get(dstUrl);
            yield return request.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
#elif UNITY_2018_1_OR_NEWER
            if (!request.isNetworkError && !request.isHttpError)
#endif
            {
                var html = request.downloadHandler.text;
                Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
                MatchCollection matches = regex.Matches(html);
                if (matches.Count <= 0)
                {
                    requestFailure = true;
                    yield break;
                }
                var formattedUrl = dstUrl;
                if (!formattedUrl.EndsWith("/"))
                {
                    formattedUrl += "/";
                }

                var relativeUrl = formattedUrl.Remove(0,urlStringLength);

                foreach (Match match in matches)
                {
                    if (!match.Success)
                        continue;
                    var fileName = match.Groups["name"].ToString();
                    if (!fileName.EndsWith("../"))
                    {
                        var fileUrl = Utility.Text.Combine(formattedUrl, fileName);
                        UnityWebRequest folderCheckRequest = UnityWebRequest.Head(fileUrl);
                        yield return folderCheckRequest.SendWebRequest();
                        bool isFile = folderCheckRequest.GetResponseHeaders().ContainsKey("Content-Length");
                        if (isFile)
                        {
                            relativeUrl.Replace(fileName, "");
                            var urlFileinfo = new WebRequestUrlFileInfo()
                            {
                                FileName = fileName,
                                URL = fileUrl,
                                RelativeUrl = relativeUrl
                            };
                            urlFileInfoList.Add(urlFileinfo);
                        }
                        else
                        {
                            yield return RunRequest(fileUrl);
                        }
                    }
                }
            }
            else
            {
                errorMessageList.Add(request.error);
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Release()
        {
            if (Coroutine != null)
                Utility.Unity.StopCoroutine(Coroutine);
            Coroutine = null;
            urlFileInfoList.Clear();
            errorMessageList.Clear();
            requestFailure = false;
            onSuccess = null;
            onFailure = null;
            TimeSpan = TimeSpan.Zero;
            WebRequestTask = null;
            urlStringLength = 0;
        }
    }
}
