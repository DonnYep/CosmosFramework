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
    public class QuarkConfig:MonoBehaviour
    {
        [SerializeField] string url;
        [SerializeField] string downloadPath;
        [SerializeField] QuarkAssetLoadMode quarkAssetLoadMode;
        [SerializeField] bool useAbsPath;
        public QuarkAssetLoadMode QuarkAssetLoadMode { get { return quarkAssetLoadMode; } }
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
                        instance= go.AddComponent<QuarkConfig>();
                    }
                }
                return instance;
            }
        }
         void Awake()
        {
            instance = this;
            QuarkManager.Instance.QuarkAssetLoadMode = quarkAssetLoadMode;
            switch (quarkAssetLoadMode)
            {
                case QuarkAssetLoadMode.AssetDatabase:
                    break;
                case QuarkAssetLoadMode.BuiltAssetBundle:
                    {
                        Utility.Text.IsStringValid(url, "URI is invalid !");
                        Utility.Text.IsStringValid(downloadPath, "DownloadPath is invalid !");
                        if (Utility.Net.PingURI(url))
                        {
                            if (!Directory.Exists(downloadPath))
                                Directory.CreateDirectory(downloadPath);
                            QuarkManager.Instance.Initiate(url, downloadPath);
                        }
                    }
                    break;
            }
        }
    }
}
