namespace Cosmos.Resource
{
    /// <summary>
    /// 一个AssetBundle所拥有的信息类型，仅作查看信息用。
    /// </summary>
    public class BundleInfo
    {
        /// <summary>
        /// 包名称
        /// </summary>
        public string BundleName;
        public string MD5;
        /// <summary>
        /// 用于校验bundle是否更新
        /// </summary>
        public string Hash;
        /// <summary>
        /// 资源包长度
        /// </summary>
        public long Length;
    }
}
