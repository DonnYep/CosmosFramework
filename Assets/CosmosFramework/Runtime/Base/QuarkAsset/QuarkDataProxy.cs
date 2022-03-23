namespace Quark.Asset
{
    /// <summary>
    /// 数据代理类；
    /// </summary>
    public class QuarkDataProxy
    {
        /// <summary>
        /// 远端存储的地址；
        /// </summary>
        public static string URL { get; set; }
        /// <summary>
        /// 本地持久化路径；
        /// </summary>
        public static string PersistentPath { get; set; }
        /// <summary>
        /// 存储ab包之间的引用关系；
        /// </summary>
        public static QuarkBuildInfo QuarkBuildInfo { get; set; }
        /// <summary>
        /// 存储ab包中包含的资源信息；
        /// </summary>
        public static QuarkManifest QuarkManifest { get; set; }
        /// <summary>
        /// 加密偏移量；
        /// </summary>
        public static ulong QuarkEncryptionOffset { get; set; }
    }
}
