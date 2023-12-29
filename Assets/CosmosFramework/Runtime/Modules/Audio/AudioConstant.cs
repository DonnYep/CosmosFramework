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
