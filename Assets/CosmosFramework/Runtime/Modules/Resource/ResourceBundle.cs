﻿using System.Collections.Generic;
using System;
namespace Cosmos.Resource
{
    /// <summary>
    /// 资源包，AssetBundle；
    /// 存储AssetBundle信息；
    /// </summary>
    [Serializable]
    public class ResourceBundle
    {
        /// <summary>
        /// 资源包的名称，AsseBundleName；
        /// </summary>
        public string BundleName { get; set; }
        /// <summary>
        /// 资源的依赖项；
        /// </summary>
        public IList<string> DependList { get; set; }
        /// <summary>
        /// 资源对象列表；
        /// </summary>
        public IList<ResourceObject> ResourceObjectList { get; set; }
    }
}