using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源信息。一个资源所拥有的信息类型，仅作查看信息用。
    /// </summary>
    public class AssetInfo
    {
        /// <summary>
        /// 资源地址。
        /// </summary>
        public string AssetPath { get; private set; }
        /// <summary>
        /// 资源类型。
        /// </summary>
        public System.Type AssetType { get; private set; }
        internal AssetInfo(string assetPath, Type assetType)
        {
            AssetPath = assetPath;
            AssetType = assetType;
        }
        private AssetInfo() { }
    }
}
