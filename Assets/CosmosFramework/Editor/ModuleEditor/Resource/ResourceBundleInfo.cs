using Cosmos.Resource;
using System.Collections.Generic;

namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// 资源包信息
    /// </summary>
    public class ResourceBundleInfo
    {
        /// <summary>
        /// 资源包的名称，AsseBundleName；
        /// </summary>
        public string BundleName { get; set; }
        /// <summary>
        /// 包相对Asset目录下的地址；
        /// </summary>
        public string BundlePath { get; set; }
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
