﻿using System;
using System.Collections.Generic;

namespace Quark.Asset
{
    /// <summary>
    /// Quark ab文件的清单；
    /// 含每个ab包中所包含的文件信息，ab的hash等；
    /// </summary>
    [Serializable]
    public class QuarkManifest : IQuarkLoaderData
    {
        [Serializable]
        public class ManifestItem
        {
            public string ABName { get; set; }
            /// <summary>
            /// Manifest AssetFileHash
            /// AB打包出来之后生成的Hash码；
            /// </summary>
            public string Hash { get; set; }
            /// <summary>
            /// AB数据数据长度，用于验证数据完整性；
            /// </summary>
            public long ABFileSize { get; set; }
            /// <summary>
            /// Assets根目录下的相对路径数组；
            /// AssetPath===[AssetName(case sensitivity)===AssetExtension]
            /// </summary>
            public Dictionary<string, QuarkAssetObject> Assets { get; set; }
        }
        /// <summary>
        /// Key:ABName;Value:Manifest;
        /// </summary>
        public Dictionary<string, ManifestItem> ManifestDict { get; set; }

        public QuarkManifest()
        {
            ManifestDict = new Dictionary<string, ManifestItem>();
        }
    }
}
