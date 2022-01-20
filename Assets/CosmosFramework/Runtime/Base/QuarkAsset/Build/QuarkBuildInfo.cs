﻿using System;
using System.Collections.Generic;

namespace Quark.Asset
{
    /// <summary>
    /// Quark asset bundle 打包信息；
    /// 含ab包之间的依赖，hash等信息；
    /// </summary>
    [Serializable]
    public class QuarkBuildInfo
    {
        [Serializable]
        public class AssetData
        {
            /// <summary>
            /// 索引序号；
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// 资源所在的AssetBundle名字；
            /// </summary>
            public string ABName { get; set; }
            /// <summary>
            /// 资源所在AB打包出来的Hash；
            /// </summary>
            public string ABHash { get; set; }
            /// <summary>
            /// 被引用的次数；
            /// </summary>
            public int ReferenceCount { get; set; } = 0;
            /// <summary>
            /// 资源的依赖项；
            /// </summary>
            public List<string> DependList { get; set; }
        }
        public QuarkBuildInfo()
        {
            AssetDataMaps = new Dictionary<string, AssetData>();
        }
        public string BuildTime { get; set; }
        /// <summary>
        /// Key:Path；Value:AssetData；
        /// </summary>
        public Dictionary<string, AssetData> AssetDataMaps { get; set; } 
        public void Dispose()
        {
            AssetDataMaps.Clear();
            BuildTime = null;
        }
    }
}
