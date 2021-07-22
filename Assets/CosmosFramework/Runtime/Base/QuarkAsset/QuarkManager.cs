using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using UnityEngine;

namespace Cosmos.Quark
{
    //================================================
    //1、QuarkAsset是一款unity资源管理的解决方案。摒弃了Resources原生的
    // 加载模式，主要针对AssetBundle在Editor模式与Runtime模式加载方式的
    //统一。
    //
    // 2、Editor模式下，加载方式主要依靠unity生成的gid进行资源寻址。通过
    // gid可以忽略由于更改文件夹地址导致的加载失败问题。
    //
    // 3、加载资源可直接通过资源名进行加载，无需通过相对地址或者完整路径
    //名。若文件资源相同，则可通过后缀名、相对于unity的assetType、以及完整
    //路径规避。
    //4、Quark设计方向是插件化，即插即用，因此内置了很多常用工具函数；
    //
    //5、使用流程：1>先初始化调用Initiate函数;
    //                        2>比较远端与本地的文件清单，调用CompareManifest；
    //                        3>下载差异文件，调用LaunchDownload；
    //================================================
    public class QuarkManager : Singleton<QuarkManager>
    {
        public QuarkAssetLoadMode QuarkAssetLoadMode { get; set; }

        QuarkDownloader quarkDownloader;
        QuarkComparator quarkComparator;
        QuarkLoader quarkLoader;
        /// <summary>
        /// 本地缓存的地址；
        /// </summary>
        public string DownloadPath { get; private set; }
        /// <summary>
        /// 远端存储的地址；
        /// </summary>
        public string URL { get; private set; }
        QuarkAssetDataset quarkAssetData;
        public QuarkAssetDataset QuarkAssetData { get { return quarkAssetData; } }
        /// <summary>
        /// 当检测到最新的；
        /// </summary>
        Action<long> onDetectedSuccess;
        /// <summary>
        /// 当检测失败；
        /// </summary>
        Action<string> onDetectedFailure;
        /// <summary>
        /// 当检测到最新的；
        /// </summary>
        public event Action<long> OnDetectedSuccess
        {
            add { onDetectedSuccess += value; }
            remove { onDetectedSuccess -= value; }
        }
        /// <summary>
        /// 当检测失败；
        /// </summary>
        public event Action<string> OnDetectedFailure
        {
            add { onDetectedFailure += value; }
            remove { onDetectedFailure -= value; }
        }
        /// <summary>
        /// URL---DownloadPath
        /// </summary>
        public event Action<string, string> OnDownloadStart
        {
            add { quarkDownloader.OnDownloadStart += value; }
            remove { quarkDownloader.OnDownloadStart -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---Data
        /// </summary>
        public event Action<string, string, byte[]> OnDownloadSuccess
        {
            add { quarkDownloader.OnDownloadSuccess += value; }
            remove { quarkDownloader.OnDownloadSuccess -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---ErrorMessage
        /// </summary>
        public event Action<string, string, string> OnDownloadFailure
        {
            add { quarkDownloader.OnDownloadFailure += value; }
            remove { quarkDownloader.OnDownloadFailure -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---OverallProgress(0~100%)---IndividualProgress(0~100%)
        /// </summary>
        public event Action<string, string, float, float> OnDownloadOverall
        {
            add { quarkDownloader.OnDownloadOverall += value; }
            remove { quarkDownloader.OnDownloadOverall -= value; }
        }
        /// <summary>
        /// SuccessURIs---FailureURIs---TimeSpan
        /// </summary>
        public event Action<string[], string[], TimeSpan> OnDownloadFinish
        {
            add { quarkDownloader.OnDownloadFinish += value; }
            remove { quarkDownloader.OnDownloadFinish -= value; }
        }

        /// <summary>
        /// 初始化，传入资源定位符与本地持久化路径；
        /// </summary>
        /// <param name="url">统一资源定位符</param>
        /// <param name="localPath">本地持久化地址</param>
        public void Initiate(string url, string localPath)
        {
            this.URL = url;
            this.DownloadPath = localPath;
            quarkComparator = new QuarkComparator(url, localPath);
            quarkDownloader = new QuarkDownloader();
            quarkLoader = new QuarkLoader(localPath);
            quarkDownloader.InitDownloader(url, localPath);
            quarkComparator.Initiate(OnCompareSuccess, OnCompareFailure);
        }
        /// <summary>
        /// 比较远程与本地的文件清单；
        /// </summary>
        public void CompareManifest()
        {
            quarkComparator.CompareLocalAndRemoteManifest();
        }
        /// <summary>
        /// 启动下载；
        /// </summary>
        public void LaunchDownload()
        {
            quarkDownloader.LaunchDownload();
        }
        /// <summary>
        /// 用于Built assetbundle模式；
        /// 对Manifest进行编码；
        /// </summary>
        /// <param name="manifest">unityWebRequest获取的Manifest文件对象</param>
        public void SetBuiltAssetBundleModeData(QuarkManifest manifest)
        {
            quarkLoader.SetBuiltAssetBundleModeData(manifest);
        }
        /// <summary>
        /// 用于Editor开发模式；
        /// 对QuarkAssetDataset进行编码
        /// </summary>
        /// <param name="assetData">QuarkAssetDataset对象</param>
        public void SetAssetDatabaseModeData(QuarkAssetDataset assetData)
        {
            quarkLoader.SetAssetDatabaseModeData(assetData);
        }
        public void UnLoadAsset()
        {
        }

        #region BuiltAssetBundle
        #endregion
        public Coroutine LoadAllAssetAsync<T>(string assetBundleName, Action<T[]> callback) where T : UnityEngine.Object
        {
            return quarkLoader.LoadAllAssetAsync<T>(assetBundleName, callback);
        }
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, bool instantiate = false)
where T : UnityEngine.Object
        {
            return quarkLoader.LoadAssetAsync<T>(assetName, callback, instantiate);
        }
        public T LoadAsset<T>(string assetName, string assetExtension = null)
    where T : UnityEngine.Object
        {
            return quarkLoader.LoadAsset<T>(assetName, assetExtension);
        }
        /// <summary>
        /// 获取比较manifest成功；
        /// </summary>
        /// <param name="latest">最新的</param>
        /// <param name="expired">过期的</param>
        /// <param name="size">整体文件大小</param>
        void OnCompareSuccess(string[] latest, string[] expired, long size)
        {
            var length = expired.Length;
            for (int i = 0; i < length; i++)
            {
                try
                {
                    var expiredPath = Utility.IO.PathCombine(DownloadPath, expired[i]);
                    Utility.IO.DeleteFile(expiredPath);
                }
                catch { }
            }
            quarkDownloader.AddDownloadFiles(latest);
            onDetectedSuccess?.Invoke(size);
        }
        /// <summary>
        /// 当比较失败；
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        void OnCompareFailure(string errorMessage)
        {
            onDetectedFailure?.Invoke(errorMessage);
        }
    }
}
