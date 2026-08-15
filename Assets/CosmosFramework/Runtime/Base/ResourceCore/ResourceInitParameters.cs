using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源系统初始化参数。
    /// </summary>
    [Serializable]
    public class ResourceInitParameters
    {
        /// <summary>
        /// 资源包裹名
        /// </summary>
        public string PackageName = CFResourceConstants.DEFAULT_PACKAGE_NAME;
        /// <summary>
        /// 运行模式
        /// </summary>
        public CFResourcePlayMode PlayMode;
        /// <summary>
        /// 文件清单路径。
        /// <para>支持绝对路径、相对StreamingAssets路径、http(s)地址。</para>
        /// </summary>
        public string ManifestPath;
        /// <summary>
        /// AssetBundle存放目录。
        /// <para>支持绝对路径、相对StreamingAssets路径、http(s)地址。</para>
        /// </summary>
        public string BundleDirectory;
        /// <summary>
        /// 文件清单加密密钥
        /// </summary>
        public string ManifestEncryptionKey;
        /// <summary>
        /// 自动释放未被引用的资源
        /// </summary>
        public bool AutoUnloadUnusedAssets = true;
    }
}
