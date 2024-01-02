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
        public string AudipAssetName { get; private set; }
        /// <summary>
        /// 音效序播放列号
        /// </summary>
        public int SerialId { get; private set; }
        /// <summary>
        /// 播放状态
        /// </summary>
        public AudioPlayStatusType AudioPlayStatusType { get; private set; }
    }
}
