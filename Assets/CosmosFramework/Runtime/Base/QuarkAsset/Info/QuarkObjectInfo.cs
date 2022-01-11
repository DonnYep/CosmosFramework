namespace Quark.Asset
{
    public struct QuarkObjectInfo
    {
        /// <summary>
        /// 资源名称；
        /// </summary>
        public string AssetName { get; private set; }
        /// <summary>
        /// 资源所属AB包名；
        /// </summary>
        public string AssetBundleName { get; private set; }
        /// <summary>
        /// 引用计数；
        /// </summary>
        public int ReferenceCount { get; private set; }
        /// <summary>
        /// 源文件的后缀名；
        /// </summary>
        public string AssetExtension { get; private set; }
        internal int ABObjectHash { get; set; }
        public QuarkObjectInfo Colne()
        {
            var info= Create(this.AssetName, this.AssetBundleName, this.AssetExtension, this.ReferenceCount);
            info.ABObjectHash = this.ABObjectHash;
            return info;
        }
        public static QuarkObjectInfo None { get { return new QuarkObjectInfo(); } }
        public static QuarkObjectInfo operator --(QuarkObjectInfo quarkObjectInfo)
        {
            var latesetReferenceCount = quarkObjectInfo.ReferenceCount;
            var newInfo = QuarkObjectInfo.Create(quarkObjectInfo.AssetName, quarkObjectInfo.AssetBundleName, quarkObjectInfo.AssetExtension, latesetReferenceCount--);
            return newInfo;
        }
        public static QuarkObjectInfo operator ++(QuarkObjectInfo quarkObjectInfo)
        {
            var latesetReferenceCount = quarkObjectInfo.ReferenceCount;
            var newInfo = QuarkObjectInfo.Create(quarkObjectInfo.AssetName, quarkObjectInfo.AssetBundleName, quarkObjectInfo.AssetExtension, latesetReferenceCount++);
            return newInfo;
        }
        public static QuarkObjectInfo Create(string assetName, string assetBundleName, string assetExtension, int referenceCount)
        {
            QuarkObjectInfo info = new QuarkObjectInfo();
            info.AssetName = assetName;
            info.AssetBundleName = assetBundleName;
            info.ReferenceCount = referenceCount;
            info.AssetExtension = assetExtension;
            return info;
        }
    }
}
