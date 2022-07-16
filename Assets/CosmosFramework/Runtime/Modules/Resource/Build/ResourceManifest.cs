using System.Collections.Generic;
using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// AssetBundle 模式下资源寻址数据对象
    /// 文件清单，记录AB包体与资源的包含关系
    /// </summary>
    [Serializable]
    public class ResourceManifest
    {
        /// <summary>
        /// 包体打包后的信息
        /// </summary>
        [Serializable]
        public class ResourceBundleBuildInfo
        {
            /// <summary>
            /// 打包后的包大小；
            /// </summary>
            public long BundleSize;
            /// <summary>
            /// 包体的Hash
            /// </summary>
            public string BundleHash;
            /// <summary>
            /// 包体数据；
            /// </summary>
            public ResourceBundle ResourceBundle;
        }
        Dictionary<string, ResourceBundleBuildInfo> resourceBundleBuildInfoDict;
        /// <summary>
        /// 打包的版本；
        /// </summary>
        public string BuildVersion { get; set; }
        /// <summary>
        /// BundleName===ResourceBundleBuildInfo；
        /// </summary>
        public Dictionary<string, ResourceBundleBuildInfo> ResourceBundleBuildInfoDict
        {
            get
            {
                if (resourceBundleBuildInfoDict == null)
                    resourceBundleBuildInfoDict = new Dictionary<string, ResourceBundleBuildInfo>();
                return resourceBundleBuildInfoDict;
            }
            set { resourceBundleBuildInfoDict = value; }
        }
    }
}
