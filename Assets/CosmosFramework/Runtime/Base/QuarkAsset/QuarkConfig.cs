using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using Quark.Asset;
using Cosmos;

namespace Quark
{
    /// <summary>
    /// Quark配置脚本，挂载到物体上配置即可；
    /// </summary>
    public class QuarkConfig : MonoBehaviour
    {
        /// <summary>
        /// 资源所在URI；
        /// </summary>
        public string Url;
        /// <summary>
        /// 是否去ping uri地址；
        /// </summary>
        public bool PingUrl;
        /// <summary>
        /// 加载模式，分别为Editor与Build；
        /// </summary>
        public QuarkAssetLoadMode QuarkAssetLoadMode;
        /// <summary>
        /// QuarkAssetLoadMode 下AssetDatabase模式所需的寻址数据；
        /// <see cref="Quark.QuarkAssetLoadMode"/>
        /// </summary>
        public QuarkAssetDataset QuarkAssetDataset;
        /// <summary>
        /// 持久化路径类型；
        /// </summary>
        public QuarkPersistentPathType QuarkPersistentPathType;
        /// <summary>
        /// 使用持久化的相对路径；
        /// </summary>
        public bool UsePersistentRelativePath;
        /// <summary>
        /// 持久化路径下的相对地址；
        /// </summary>
        public string PersistentRelativePath;
        /// <summary>
        /// 对称加密密钥；
        /// </summary>
        public string AESEncryptionKey;
        /// <summary>
        /// QuarkPersistentPathType 枚举下的自定义持久化路径；
        /// <see cref=" Quark.QuarkPersistentPathType"/>
        /// </summary>
        public string CustomePersistentPath;


        /// <summary>
        /// 配置的路径都整合到此字段；
        /// </summary>
        string downloadPath;

        static QuarkConfig instance;
        public static QuarkConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<QuarkConfig>();
                    if (instance == null)
                    {
                        var go = new GameObject(typeof(QuarkConfig).Name);
                        instance = go.AddComponent<QuarkConfig>();
                    }
                }
                return instance;
            }
        }
        void Awake()
        {
            instance = this;
            QuarkManager.Instance.QuarkAssetLoadMode = QuarkAssetLoadMode;
            QuarkManager.Instance.AESEncryptionKey = AESEncryptionKey;
            switch (QuarkAssetLoadMode)
            {
                case QuarkAssetLoadMode.AssetDatabase:
                    {
                        if (QuarkAssetDataset != null)
                            QuarkManager.Instance.SetAssetDatabaseModeData(QuarkAssetDataset);
                    }
                    break;
                case QuarkAssetLoadMode.BuiltAssetBundle:
                    {
                        //var downloadPath =  Path.Combine( Application.persistentDataPath,"DownloadPath");
                        Utility.Text.IsStringValid(Url, "URI is invalid !");
                        if (PingUrl)
                        {
                            if (!Utility.Net.PingURI(Url))
                                return;
                        }
                        if (QuarkPersistentPathType != QuarkPersistentPathType.CustomePersistentPath)
                        {
                            switch (QuarkPersistentPathType)
                            {
                                case QuarkPersistentPathType.PersistentDataPath:
                                    downloadPath = Application.persistentDataPath;
                                    break;
                                case QuarkPersistentPathType.StreamingAssets:
                                    downloadPath = Application.streamingAssetsPath;
                                    break;
                            }
                            if (UsePersistentRelativePath)
                            {
                                downloadPath = Path.Combine(downloadPath, PersistentRelativePath);
                            }
                        }
                        else
                        {
                            downloadPath = CustomePersistentPath;
                        }
                        Utility.Text.IsStringValid(downloadPath, "DownloadPath is invalid !");
                        if (!Directory.Exists(downloadPath))
                            Directory.CreateDirectory(downloadPath);
                        QuarkManager.Instance.Initiate(Url, downloadPath);
                        QuarkManager.Instance.CheckForUpdates();
                    }
                    break;
            }
        }
    }
}
