using System.Runtime.InteropServices;

namespace Cosmos.Resource.Verifiy
{
    /// <summary>
    /// Bundle用于校验时的数据；
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct ResourceBundleVerifiyInfo
    {
        /// <summary>
        /// 包的名称；
        /// </summary>
        public string BundleName;
        /// <summary>
        /// 包的加载标记；
        /// </summary>
        public string BundleKey;
        /// <summary>
        /// 包体的Hash
        /// </summary>
        public string BundleHash;
        /// <summary>
        /// AssetBundle包大小；
        /// </summary>
        public long BundleSize;
    }
}
