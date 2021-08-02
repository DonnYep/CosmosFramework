using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
namespace Cosmos.Quark
{
    /// <summary>
    /// Quark配置脚本，挂载到物体上配置即可；
    /// </summary>
    public class QuarkConfig : MonoBehaviour
    {
        public string Url;
        public string DownloadPath;
        public bool PingUrl;
        public QuarkAssetLoadMode QuarkAssetLoadMode;
        public QuarkAssetDataset QuarkAssetDataset;
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
                        Utility.Text.IsStringValid(Url, "URI is invalid !");
                        Utility.Text.IsStringValid(DownloadPath, "DownloadPath is invalid !");
                        if (PingUrl)
                        {
                            if (Utility.Net.PingURI(Url))
                            {
                                if (!Directory.Exists(DownloadPath))
                                    Directory.CreateDirectory(DownloadPath);
                                QuarkManager.Instance.Initiate(Url, DownloadPath);
                            }
                        }
                        else
                        {
                            if (!Directory.Exists(DownloadPath))
                                Directory.CreateDirectory(DownloadPath);
                            QuarkManager.Instance.Initiate(Url, DownloadPath);
                        }
                        QuarkManager.Instance.CheckForUpdates();
                    }
                    break;
            }
        }
    }
}
