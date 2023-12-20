using System.Runtime.InteropServices;

namespace Cosmos.Audio
{
    /// <summary>
    /// 音效状态信息
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct AudioStateInfo
    {
        /// <summary>
        /// 声音资源信息
        /// </summary>
        public string AssetName { get; private set; }
        /// <summary>
        /// 声音组别；
        /// </summary>
        public string AudioGroupName { get; private set; }
        /// <summary>
        /// 播放状态
        /// </summary>
        public AudioPlayStatusType AudioPlayStatusType { get; private set; }
    }
}
