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
        [Serializable]
        public class BundleManifest
        {
            public string BundleName { get; set; }
            /// <summary>
            /// AB打包出来之后生成的Hash码；
            /// </summary>
            public string Hash { get; set; }
            /// <summary>
            /// AB数据数据长度，用于验证数据完整性；
            /// </summary>
            public long ABFileSize { get; set; }
            /// <summary>
            /// 资源的依赖项；
            /// </summary>
            public IList<string> DependList { get; set; }
            /// <summary>
            /// AssetName===ResourceObject
            /// </summary>
            public Dictionary<string, ResourceObject> ResourceObjectDict;
        }
        /// <summary>
        /// 打包的版本；
        /// </summary>
        public string BuildVersion { get; set; }
        /// <summary>
        /// BundleName===BundleManifest；
        /// </summary>
        public Dictionary<string, BundleManifest> BundleManifestDict { get; set; }
    }
}
