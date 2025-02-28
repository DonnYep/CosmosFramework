using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cosmos.Resource
{
    public class ResourcePackageSO : ScriptableObject
    {
        /// <summary>
        /// 资源包裹名
        /// </summary>
        public string PackageName;
        /// <summary>
        /// 下属的PackageBundleSO文件hash值
        /// </summary>
        public string[] BundleHashs;
    }
}
