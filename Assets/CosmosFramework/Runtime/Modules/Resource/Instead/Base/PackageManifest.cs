using System.Collections.Generic;

namespace Cosmos.Resource
{
    /// <summary>
    /// 文件清单
    /// </summary>
    internal class PackageManifest
    {
        /// <summary>
        /// 资源包裹名称
        /// </summary>
        public string PackageName;
        /// <summary>
        /// 版本信息
        /// </summary>
        public string PackageVersion;
        /// <summary>
        /// 资源列表
        /// </summary>
        public List<PackageAsset> AssetList = new List<PackageAsset>();
        /// <summary>
        /// 资源包列表
        /// </summary>
        public List<PackageBundle> BundleList = new List<PackageBundle>();

    }
}
