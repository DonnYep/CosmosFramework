using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源包
    /// </summary>
    internal class PackageBundle: IEquatable<PackageBundle>
    {
        /// <summary>
        /// 资源包名
        /// </summary>
        public string BundleName;
        /// <summary>
        /// unity生成的CRC
        /// </summary>
        public uint CRC;
        /// <summary>
        /// 文件hash值
        /// </summary>
        public string FileHash;
        /// <summary>
        /// 文件字节数
        /// </summary>
        public long FileSize;
        /// <summary>
        /// 依赖的资源包Hash集合
        /// </summary>
        public int[] DependHashs;

        /// <summary>
        /// 所属的包裹名称
        /// </summary>
        public string PackageName { private set; get; }
        /// <summary>
        /// 解析资源包
        /// </summary>
        public void ParseBundle(PackageManifest manifest)
        {
            PackageName = manifest.PackageName;

        }
        public bool Equals(PackageBundle other)
        {
            return other.FileHash == this.FileHash;
        }
    }
}
