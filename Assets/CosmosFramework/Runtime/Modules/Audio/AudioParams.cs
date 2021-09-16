
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Audio
{
    /// <summary>
    /// 声音播放时的参数；
    /// </summary>
    public struct AudioParams
    {
        /// <summary>
        /// Audio开始播放时间，默认从0秒开始；
        /// </summary>
        public float PlayTime { get; set; }
        /// <summary>
        /// 播放循环；
        /// </summary>
        public bool Loop { get; set; }
        /// <summary>
        /// 声音优先级；
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 音量；
        /// </summary>
        public float Volume { get; set; }
        /// <summary>
        /// 播放速度；
        /// </summary>
        public float Pitch { get; set; }
        /// <summary>
        /// 立体声左右声道；
        /// </summary>
        public float StereoPan { get; set; }
        /// <summary>
        /// 声音空间混合量。
        /// </summary>
        public float SpatialBlend { get; set; }
        /// <summary>
        /// 音频混响区域；
        /// </summary>
        public float ReverbZoneMix { get; set; }
        /// <summary>
        /// 多普勒等级。
        /// </summary>
        public float DopplerLevel { get; set; }
        /// <summary>
        /// 扩散；
        /// </summary>
        public int Spread { get; set; }
        /// <summary>
        /// 最大可听范围；
        /// </summary>
        public float MaxDistance { get; set; }
        public void Reset()
        {
            PlayTime = 0;
            Loop = AudioConstant.Loop;
            Priority = AudioConstant.Priority;
            Volume = AudioConstant.Volume;
            Pitch = AudioConstant.Pitch;
            StereoPan = AudioConstant.StereoPan;
            SpatialBlend = AudioConstant.SpatialBlend;
            ReverbZoneMix = AudioConstant.ReverbZoneMix;
            DopplerLevel = AudioConstant.DopplerLevel;
            Spread = AudioConstant.Spread;
            MaxDistance = AudioConstant.MaxDistance;
        }
        readonly static AudioParams m_Default = new AudioParams()
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
            MaxDistance = AudioConstant.MaxDistance
        };
        public  static AudioParams Default { get { return m_Default; } }
    }
}
