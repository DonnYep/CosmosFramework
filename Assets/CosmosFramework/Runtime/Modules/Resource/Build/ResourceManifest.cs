using System.Collections.Generic;
using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// 文件清单，记录AB包体与资源的包含关系
    /// </summary>
    [Serializable]
    public class ResourceManifest
    {
        /// <summary>
        /// BundleName===ResourceBundle；
        /// </summary>
        public Dictionary<string, ResourceBundle> ResourceBundleDict { get; set; }
    }
}
