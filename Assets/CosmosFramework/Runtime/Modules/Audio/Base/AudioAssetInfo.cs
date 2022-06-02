using System.Runtime.InteropServices;
namespace Cosmos.Audio
{
    [StructLayout(LayoutKind.Auto)]
    public struct AudioAssetInfo 
    {
        /// <summary>
        /// 声音资源信息
        /// </summary>
        public string AssetName { get; private set; }
        /// <summary>
        /// 声音组别；
        /// </summary>
        public string AudioGroupName { get; private set; }
        public AudioAssetInfo(string assetName) : this(assetName, string.Empty) { }
        public AudioAssetInfo(string assetName, string audioGroupName)
        {
            AssetName = assetName;
            AudioGroupName = audioGroupName;
        }
    }
}
