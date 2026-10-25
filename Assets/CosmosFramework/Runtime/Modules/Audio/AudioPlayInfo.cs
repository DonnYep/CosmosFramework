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
        /// <summary>
        /// 当前实际音量（已含组音量与淡入淡出），0~1
        /// </summary>
        public float Volume;
        /// <summary>
        /// 当前播放位置，单位秒
        /// </summary>
        public float Time;
        public AudioPlayInfo(string audioAssetName, string audioGroupName, int serialId, AudioPlayStatusType audioPlayStatusType)
            : this(audioAssetName, audioGroupName, serialId, audioPlayStatusType, 0f, 0f)
        {
        }
        public AudioPlayInfo(string audioAssetName, string audioGroupName, int serialId, AudioPlayStatusType audioPlayStatusType, float volume, float time)
        {
            AudioAssetName = audioAssetName;
            AudioGroupName = audioGroupName;
            SerialId = serialId;
            AudioPlayStatusType = audioPlayStatusType;
            Volume = volume;
            Time = time;
        }
        public static readonly AudioPlayInfo Default = new AudioPlayInfo(string.Empty, string.Empty, 0, AudioPlayStatusType.None); 
    }
}
