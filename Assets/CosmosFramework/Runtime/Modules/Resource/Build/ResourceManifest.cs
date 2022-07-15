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
        Dictionary<string, ResourceBundle> bundleDict;
        /// <summary>
        /// 打包的版本；
        /// </summary>
        public string BuildVersion { get; set; }
        /// <summary>
        /// BundleName===ResourceBundle；
        /// </summary>
        public Dictionary<string, ResourceBundle> BundleDict
        {
            get
            {
                if (bundleDict == null)
                    bundleDict = new Dictionary<string, ResourceBundle>();
                return bundleDict;
            }
            set { bundleDict = value; }
        }
    }
}
