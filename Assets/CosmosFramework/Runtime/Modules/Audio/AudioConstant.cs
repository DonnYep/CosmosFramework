namespace Cosmos.Audio
{
    /// <summary>
    /// 声音模块常量
    /// </summary>
    public class AudioConstant
    {
        internal const bool PalyOnAwake = true;
        internal const bool Loop = false;
        internal const int Priority = 128;
        internal const float Volume = 1;
        internal const float Pitch = 1;
        internal const float StereoPan = 0;
        internal const float SpatialBlend = 0;
        internal const float ReverbZoneMix = 1;
        internal const float DopplerLevel = 1;
        internal const int Spread = 0;
        internal const float MaxDistance = 500;

        /// <summary>
        /// 检查播放间隔，5秒
        /// </summary>
        internal const int CheckPlayingIntervalSecond = 5;
        /// <summary>
        /// 播放速度下限；pitch小于等于0时播放位置不会推进，会出现"永久在播却静音"的幽灵音效
        /// </summary>
        internal const float MIN_PITCH = 0.01f;
        /// <summary>
        /// 声音组默认音量
        /// </summary>
        internal const float DEFAULT_AUDIO_GROUP_VOLUME = 1f;
        /// <summary>
        /// 声音组默认最大同时播放数，0表示不限制
        /// </summary>
        internal const int DEFAULT_MAX_CONCURRENT_AUDIOS = 0;
        /// <summary>
        /// 单帧最大推进时间；用于卡顿、断点、长时间挂起后避免过渡在一帧内跳完
        /// </summary>
        internal const float MAX_TICK_DELTA_TIME = 0.1f;
        /// <summary>
        /// 声音常量
        /// </summary>
        internal const string PREFIX = "SND-";
        /// <summary>
        /// 默认音效组名
        /// </summary>
        public const string DEFAULT_AUDIO_GROUP = "<DEFAULT_AUDIO_GROUP>";

        /// <summary>
        /// 默认音效淡出时间，单位秒
        /// </summary>
        public const float DEFAULT_FADEOUT_SECONDS = 0;
        /// <summary>
        /// 默认音效淡入时间，单位秒
        /// </summary>
        public const float DEFAULT_FADEIN_SECONDS = 0;
    }
}
