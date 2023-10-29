using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    public class WebRequestSuccessEventArgs : GameEventArgs
    {
        public long TaskId { get; private set; }
        public string URL { get; private set; }
        public byte[] Data { get; private set; }
        public UnityWebRequest WebRequest { get; private set; }
        public TimeSpan TimeSpan { get; private set; }
        public AudioClip GetAudioClip()
        {
            if (WebRequest == null)
                return null;
            return DownloadHandlerAudioClip.GetContent(WebRequest);
        }
        public AssetBundle GetAssetBundle()
        {
            if (WebRequest == null)
                return null;
            return DownloadHandlerAssetBundle.GetContent(WebRequest);
        }
        public Texture2D GetTexture2D()
        {
            if (WebRequest == null)
                return null;
            return DownloadHandlerTexture.GetContent(WebRequest);
        }
        public string GetText()
        {
            if (WebRequest == null)
                return null;
            return WebRequest.downloadHandler.text;
        }
        public override void Release()
        {
            TaskId = 0;
            URL = string.Empty;
            Data = null;
            WebRequest = null;
            TimeSpan = TimeSpan.Zero;
        }
        internal static WebRequestSuccessEventArgs Create(long taskId, string url, byte[] data, UnityWebRequest webRequest, TimeSpan timeSpan)
        {
            var eventArgs = ReferencePool.Acquire<WebRequestSuccessEventArgs>();
            eventArgs.TaskId = taskId;
            eventArgs.URL = url;
            eventArgs.Data = data;
            eventArgs.WebRequest = webRequest;
            eventArgs.TimeSpan = timeSpan;
            return eventArgs;
        }
        internal static void Release(WebRequestSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
