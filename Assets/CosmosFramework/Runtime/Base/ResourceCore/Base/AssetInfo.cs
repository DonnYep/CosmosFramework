using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源信息。一个资源所拥有的信息类型，仅作查看信息用。
    /// </summary>
    public class AssetInfo
    {
        /// <summary>
        /// 资源信息对应的资产对象
        /// </summary>
        readonly PackageAsset packageAsset;
        /// <summary>
        /// 资产的唯一识别码
        /// </summary>
        string uid;
        /// <summary>
        /// 资源所属包体
        /// </summary>
        public string PackageName { get; private set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Invalid
        {
            get { return packageAsset == null; }
        }
        /// <summary>
        /// 资产地址
        /// </summary>
        public string AssetPath
        {
            get { return packageAsset.AssetPath; }
        }
        public string Name
        {
            get { return packageAsset.Name; }
        }
        public string Extension
        {
            get { return packageAsset.Extension; }
        }
        /// <summary>
        /// 所属bundle名称
        /// </summary>
        public string BundleName
        {
            get { return packageAsset.BundleName; }
        }
        /// <summary>
        /// bundle内的唯一识别编码
        /// </summary>
        public string UID
        {
            get
            {
                if (string.IsNullOrEmpty(uid))
                {
                    //暂时使用资产地址进行识别
                    uid = $"<{AssetPath}>";
                }
                return uid;
            }
        }
        internal AssetInfo(string packageName, PackageAsset packageAsset)
        {
            PackageName = packageName;
            this.packageAsset = packageAsset;
        }
        private AssetInfo() { }
    }
}
