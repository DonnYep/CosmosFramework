using System;
namespace Cosmos.Audio
{
    /// <summary>
    /// 声音播放时的参数；
    /// </summary>
    public struct AudioParams : IEquatable<AudioParams>
    {
        /// <summary>
        /// Audio开始播放时间，默认从0秒开始
        /// </summary>
        public float PlayTime { get; set; }
        /// <summary>
        /// 播放循环
        /// </summary>
        public bool Loop { get; set; }
        /// <summary>
        /// 声音优先级
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 音量
        /// </summary>
        public float Volume { get; set; }
        /// <summary>
        /// 播放速度
        /// </summary>
        public float Pitch { get; set; }
        /// <summary>
        /// 立体声左右声道
        /// </summary>
        public float StereoPan { get; set; }
        /// <summary>
        /// 声音空间混合量
        /// </summary>
        public float SpatialBlend { get; set; }
        /// <summary>
        /// 音频混响区域
        /// </summary>
        public float ReverbZoneMix { get; set; }
        /// <summary>
        /// 多普勒等级
        /// </summary>
        public float DopplerLevel { get; set; }
        /// <summary>
        /// 扩散
        /// </summary>
        public int Spread { get; set; }
        /// <summary>
        /// 最大可听范围
        /// </summary>
        public float MaxDistance { get; set; }
        /// <summary>
        /// 渐入时间，单位秒
        /// </summary>
        public float FadeInSeconds { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public readonly static AudioParams Default = new AudioParams()
        {
            PlayTime = 0,
            Loop = AudioConstant.Loop,
            Priority = AudioConstant.Priority,
            Volume = AudioConstant.Volume,
            Pitch = AudioConstant.Pitch,
            StereoPan = AudioConstant.StereoPan,
            SpatialBlend = AudioConstant.SpatialBlend,
            ReverbZoneMix = AudioConstant.ReverbZoneMix,
            DopplerLevel = AudioConstant.DopplerLevel,
            Spread = AudioConstant.Spread,
            MaxDistance = AudioConstant.MaxDistance,
            FadeInSeconds = AudioConstant.DEFAULT_FADEIN_SECONDS
        };
        public bool Equals(AudioParams other)
        {
            return other.PlayTime == PlayTime &&
                other.Loop == Loop &&
                other.Priority == Priority &&
                other.Volume == Volume &&
                other.Pitch == Pitch &&
                other.StereoPan == StereoPan &&
                other.SpatialBlend == SpatialBlend &&
                other.ReverbZoneMix == ReverbZoneMix &&
                other.DopplerLevel == DopplerLevel &&
                other.Spread == Spread &&
                other.MaxDistance == MaxDistance;
        }
    }
}
