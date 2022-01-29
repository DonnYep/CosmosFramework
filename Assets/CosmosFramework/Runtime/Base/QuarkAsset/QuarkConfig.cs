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
        /// 
        /// </summary>
        public QuarkBuildPath QuarkBuildPath;
        /// <summary>
        ///启用 ab build 的相对路径；
        /// </summary>
        public bool EnableRelativeBuildPath;
        /// <summary>
        /// ab Build 的相对地址；
        /// </summary>
        public string RelativeBuildPath;


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
        public QuarkLoadPath QuarkLoadPath;
        /// <summary>
        /// 使用持久化的相对路径；
        /// </summary>
        public bool EnableRelativeLoadPath;
        /// <summary>
        /// 持久化路径下的相对地址；
        /// </summary>
        public string RelativeLoadPath;
        /// <summary>
        /// 对称加密密钥；
        /// </summary>
        public string AESEncryptionKey;
        /// <summary>
        /// QuarkPersistentPathType 枚举下的自定义持久化路径；
        /// <see cref=" Quark.QuarkLoadPath"/>
        /// </summary>
        public string CustomeLoadPath;

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
                        switch (QuarkBuildPath)
                        {
                            case QuarkBuildPath.StreamingAssets:
                                StreamingAssetsTab();
                                break;
                            case QuarkBuildPath.URL:
                                URLTab();
                                break;
                        }
                    }
                    break;
            }
        }
        void URLTab()
        {
            Utility.Text.IsStringValid(Url, "URI is invalid !");
            if (PingUrl)
            {
                if (!Utility.Net.PingURI(Url))
                    return;
            }
            if (QuarkLoadPath != QuarkLoadPath.Custome)
            {
                switch (QuarkLoadPath)
                {
                    case QuarkLoadPath.PersistentDataPath:
                        downloadPath = Application.persistentDataPath;
                        break;
                    case QuarkLoadPath.StreamingAssets:
                        downloadPath = Application.streamingAssetsPath;
                        break;
                }
                if (EnableRelativeLoadPath)
                {
                    downloadPath = Path.Combine(downloadPath, RelativeLoadPath);
                }
            }
            else
            {
                downloadPath = CustomeLoadPath;
            }
            Utility.Text.IsStringValid(downloadPath, "DownloadPath is invalid !");
            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);
            QuarkManager.Instance.Initiate(Url, downloadPath);
            QuarkManager.Instance.CheckForUpdates();
        }
        void StreamingAssetsTab()
        {
            string streamingAssetPath = string.Empty;
            if (EnableRelativeBuildPath)
                streamingAssetPath = Path.Combine(Application.streamingAssetsPath, RelativeBuildPath);
            else
                streamingAssetPath = Application.streamingAssetsPath;
            QuarkManager.Instance.Initiate(streamingAssetPath, streamingAssetPath);
            QuarkManager.Instance.LoadFromStreamingAssets();
        }
        private void OnDestroy()
        {
            QuarkManager.Instance.StopDownload();
        }
    }
}
