using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源模块全局配置
    /// </summary>
    public class ResourceSettings : ScriptableObject, IDisposable
    {
        static ResourceSettings instance;
        public static ResourceSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UnityEngine.Resources.Load<ResourceSettings>("ResourceSettings");
                }
                return instance;
            }
        }
        /// <summary>
        /// 文件清单名称
        /// </summary>
        [SerializeField] string manifestFileName = "Manifest";
        /// <summary>
        /// 版本号
        /// </summary>
        [SerializeField] string version;
        /// <summary>
        /// 能够被识别的文件后缀名
        /// </summary>
        [SerializeField] List<string> availableExtenisonList;

        [SerializeField] ResourceLoadMode resourceLoadMode;
        public ResourceLoadMode ResourceLoadMode
        {
            get { return resourceLoadMode; }
            set { resourceLoadMode = value; }
        }

        public void Dispose()
        {
        }
    }
}
