using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    //================================================
    /*
     * 1、WebRequest用于加载AssetBundle资源。资源状态可以是Remote的，
    *  也可以是Local下persistentDataPath的；
     * 
     * 2、内置已经实现了一个默认的WebRequest帮助类对象；模块初始化时会
    * 自动加载并将默认的helper设置为此模块的默认加载helper；
    * 
    */
    //================================================
    public interface IWebRequestManager : IModuleManager, IModuleInstance
    {
        /// <summary>
        /// 任务数量
        /// </summary>
        int TaskCount { get; }
        /// <summary>
        /// 正在执行下载或上传的任意操作
        /// </summary>
        bool Running { get; }
        /// <summary>
        /// 开始回调；
        /// </summary>
        event Action<WebRequestStartEventArgs> OnStartCallback;
        /// <summary>
        /// 进度回调；
        /// </summary>
        event Action<WebRequestUpdateEventArgs> OnUpdateCallback;
        /// <summary>
        /// 成功回调；
        /// </summary>
        event Action<WebRequestSuccessEventArgs> OnSuccessCallback;
        /// <summary>
        /// 失败回调；
        /// </summary>
        event Action<WebRequestFailureEventArgs> OnFailureCallback;
        /// <summary>
        /// 获取文件长度失败回调；
        /// </summary>
        event Action<WebRequestGetContentLengthFailureEventArgs> OnGetContentLengthFailureCallback;
        /// <summary>
        /// 获取文件长度成功回调；
        /// </summary>
        event Action<WebRequestGetContentLengthSuccessEventArgs> OnGetContentLengthSuccessCallback;
        /// <summary>
        /// 所有任务完成回调；
        /// </summary>
        event Action<WebRequestAllTaskCompleteEventArgs> OnAllTaskCompleteCallback;
        /// <summary>
        /// 添加下载AssetBundle任务；
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns>任务id</returns>
        long AddDownloadAssetBundleTask(string url);
        /// <summary>
        ///  添加下载Audio任务；
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="audioType">声音类型</param>
        /// <returns>任务id</returns>
        long AddDownloadAudioTask(string url, AudioType audioType);
        /// <summary>
        /// 添加下载Text任务；
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns>任务id</returns>
        long AddDownloadTextTask(string url);
        /// <summary>
        /// 添加下载Texture任务；
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns>任务id</returns>
        long AddDownloadTextureTask(string url);
        /// <summary>
        /// 添加下载请求任务；
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns>任务id</returns>
        long AddDownloadRequestTask(string url);
        /// <summary>
        /// 添加上传请求任务；
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">数据</param>
        /// <param name="uploadType">上传类型</param>
        /// <returns>任务id</returns>
        long AddUploadRequestTask(string url, byte[] data, WebRequestUploadType uploadType);
        /// <summary>
        /// 添加上传请求任务；
        /// </summary>
        /// <param name="webRequest">请求</param>
        /// <returns>任务id</returns>
        long AddUploadRequestTask(UnityWebRequest webRequest);
        /// <summary>
        /// 添加下载请求任务；
        /// </summary>
        /// <param name="webRequest">请求</param>
        /// <returns>任务id</returns>
        long AddDownloadRequestTask(UnityWebRequest webRequest);
        /// <summary>
        /// 添加获取文件大小的请求；
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns>任务id</returns>
        long AddGetContentLengthTask(string url);
        /// <summary>
        /// 移除任务；
        /// </summary>
        /// <param name="taskId">任务id</param>
        /// <returns>移除结果</returns>
        bool RemoveTask(long taskId);
        /// <summary>
        /// 是否存在任务；
        /// </summary>
        /// <param name="taskId">任务id</param>
        /// <returns>存在结果</returns>
        bool HasTask(long taskId);
        /// <summary>
        /// 开始执行请求任务；
        /// </summary>
        void StartRequestTasks();
        /// <summary>
        /// 停止但不清空任务，效果与暂停相似；
        /// </summary>
        void StopRequestTasks();
        /// <summary>
        /// 清空并终止所有请求任务；
        /// </summary>
        void AbortRequestTasks();
    }
}
