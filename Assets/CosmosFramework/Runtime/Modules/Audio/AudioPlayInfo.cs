using System.Runtime.InteropServices;

namespace Cosmos.Audio
{
    /// <summary>
    /// 声音播放状态
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct AudioPlayInfo
    {
        /// <summary>
        /// 声音资源名称
        /// </summary>
        public string AudioAssetName;
        /// <summary>
        /// 声音组名称
        /// </summary>
        public string AudioGroupName;
        /// <summary>
        /// 音效序播放列号
        /// </summary>
        public int SerialId;
        /// <summary>
        /// 播放状态
        /// </summary>
        public AudioPlayStatusType AudioPlayStatusType;
        public AudioPlayInfo(string audioAssetName, string audioGroupName, int serialId, AudioPlayStatusType audioPlayStatusType)
        {
            AudioAssetName = audioAssetName;
            AudioGroupName = audioGroupName;
            SerialId = serialId;
            AudioPlayStatusType = audioPlayStatusType;
        }
        public static readonly AudioPlayInfo Default = new AudioPlayInfo(string.Empty, string.Empty, 0, AudioPlayStatusType.None); 
    }
}
