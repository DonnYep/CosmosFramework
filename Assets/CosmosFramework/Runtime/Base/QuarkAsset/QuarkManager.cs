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
    //================================================
    public class QuarkManager : Singleton<QuarkManager>
    {
        QuarkManifest quarkManifest;
        /// <summary>
        /// AssetDataBase模式下资源的映射字典；
        /// Key : AssetName---Value :  Lnk [QuarkAssetObject]
        /// </summary>
        Dictionary<string, LinkedList<QuarkAssetDatabaseObject>> assetDatabaseMap;
        /// <summary>
        /// BuiltAssetBundle 模式下资源的映射；
        /// Key : ABName---Value :  Lnk [QuarkAssetABObject]
        /// </summary>` 
        Dictionary<string, LinkedList<QuarkAssetBundleObject>> builtAssetBundleMap;
        /// <summary>
        /// Key:[ABName] ; Value : [ABHash]
        /// </summary>
        Dictionary<string, string> assetBundleHashDict;

        /// <summary>
        /// Key : [ABName] ; Value : [AssetBundle]
        /// </summary>
        Dictionary<string, AssetBundle> assetBundleDict;
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
        Action<long> onDetectedLatest;
        /// <summary>
        /// 当检测失败；
        /// </summary>
        Action<string> onDetectedFailure;
        /// <summary>
        /// 当检测到最新的；
        /// </summary>
        public event Action<long> OnDetectedLatest
        {
            add { onDetectedLatest += value; }
            remove { onDetectedLatest -= value; }
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
        public event Action<string, string> DownloadStart
        {
            add { quarkDownloader.OnDownloadStart += value; }
            remove { quarkDownloader.OnDownloadStart -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---Data
        /// </summary>
        public event Action<string, string, byte[]> DownloadSuccess
        {
            add { quarkDownloader.OnDownloadSuccess += value; }
            remove { quarkDownloader.OnDownloadSuccess -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---ErrorMessage
        /// </summary>
        public event Action<string, string, string> DownloadFailure
        {
            add { quarkDownloader.OnDownloadFailure += value; }
            remove { quarkDownloader.OnDownloadFailure -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---OverallProgress(0~100%)---IndividualProgress(0~100%)
        /// </summary>
        public event Action<string, string, float, float> DownloadOverall
        {
            add { quarkDownloader.OnDownloadOverall += value; }
            remove { quarkDownloader.OnDownloadOverall -= value; }
        }
        /// <summary>
        /// SuccessURIs---FailureURIs---TimeSpan
        /// </summary>
        public event Action<string[], string[], TimeSpan> DownloadFinish
        {
            add { quarkDownloader.OnDownloadFinish += value; }
            remove { quarkDownloader.OnDownloadFinish -= value; }
        }

        public void Initiate(string uri, string localPath)
        {
            this.URL = uri;
            this.DownloadPath = localPath;
            quarkComparator = new QuarkComparator(uri, localPath);
            quarkDownloader.InitDownloader(uri, localPath);
            quarkComparator.Initiate(OnCompareSuccess, OnCompareFailure);
        }
        public void CompareManifest()
        {
            quarkComparator.CompareLocalAndRemoteManifest();
        }
        public void LaunchDownload()
        {
            quarkDownloader.LaunchDownload();
        }
        /// <summary>
        /// 对Manifest进行编码；
        /// </summary>
        /// <param name="manifest">unityWebRequest获取的Manifest文件对象</param>
        public void SetBuiltAssetBundleModeData(QuarkManifest manifest)
        {
            var lnkDict = new Dictionary<string, LinkedList<QuarkAssetBundleObject>>();
            foreach (var mf in manifest.ManifestDict)
            {
                var assetPaths = mf.Value.Assets;
                var length = assetPaths.Length;
                for (int i = 0; i < length; i++)
                {
                    var qab = GetAssetBundleObject(mf.Value.ABName, assetPaths[i]);
                    if (!lnkDict.TryGetValue(qab.AssetName, out var lnkList))
                    {
                        lnkList = new LinkedList<QuarkAssetBundleObject>();
                        lnkList.AddLast(qab);
                        lnkDict.Add(qab.AssetName, lnkList);
                    }
                    else
                    {
                        lnkList.AddLast(qab);
                    }
                }
            }
            builtAssetBundleMap = lnkDict;
        }
        /// <summary>
        /// 对QuarkAssetDataset进行编码
        /// </summary>
        /// <param name="assetData">QuarkAssetDataset对象</param>
        public void SetAssetDatabaseModeData(QuarkAssetDataset assetData)
        {
            var lnkDict = new Dictionary<string, LinkedList<QuarkAssetDatabaseObject>>();
            var length = assetData.QuarkAssetObjectList.Count;
            for (int i = 0; i < length; i++)
            {
                var name = assetData.QuarkAssetObjectList[i].AssetName;
                if (!lnkDict.TryGetValue(name, out var lnkList))
                {
                    var lnk = new LinkedList<QuarkAssetDatabaseObject>();
                    lnk.AddLast(assetData.QuarkAssetObjectList[i]);
                    lnkDict.Add(name, lnk);
                }
                else
                {
                    lnkList.AddLast(assetData.QuarkAssetObjectList[i]);
                }
            }
            quarkAssetData = assetData;
            assetDatabaseMap = lnkDict;
        }

        public void UnLoadAsset()
        {
        }

        #region BuiltAssetBundle
        #endregion
        public T[] LoadAllAsset<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
        {
            return quarkLoader.LoadAllAsset<T>(assetBundleName, assetName);
        }
        public T LoadAsset<T>(string assetName, string assetExtension = null)
    where T : UnityEngine.Object
        {
            return quarkLoader.LoadAsset<T>(assetName, assetExtension);
        }
        QuarkAssetBundleObject GetAssetBundleObject(string abName, string assetName)
        {
            var qab = new QuarkAssetBundleObject()
            {
                AssetBundleName = abName,
                AssetPath = assetName,
            };
            var strs = Utility.Text.StringSplit(assetName, new string[] { "/" });
            var nameWithExt = strs[strs.Length - 1];
            var splits = Utility.Text.StringSplit(nameWithExt, new string[] { "." });
            qab.AssetExtension = Utility.Text.Combine(".", splits[splits.Length - 1]);
            qab.AssetName = nameWithExt.Replace(qab.AssetExtension, "");
            return qab;
        }
        /// <summary>
        /// 获取比较manifest成功；
        /// </summary>
        /// <param name="latest">最新的</param>
        /// <param name="expired">过期的</param>
        /// <param name="size">整体文件大小</param>
        void OnCompareSuccess(string[] latest, string[] expired, long size)
        {
            quarkDownloader.AddDownloadFiles(latest);
            onDetectedLatest?.Invoke(size);
            var length = expired.Length;
            for (int i = 0; i < length; i++)
            {
                try
                {
                    Utility.IO.DeleteFile(Utility.IO.PathCombine(DownloadPath, expired[i]));
                }
                catch { }
            }
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
