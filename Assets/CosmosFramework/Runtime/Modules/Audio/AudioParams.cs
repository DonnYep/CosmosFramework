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
    public sealed class AudioParams : IReference
    {
        public float PlayTime { get; set; }
        //public bool Mute { get; set; }
        //public bool PlayOnAwake { get; set; }
        public bool Loop { get; set; }
        /// <summary>
        /// 声音优先级；
        /// </summary>
        public int Priority { get; set; }
        public float Volume { get; set; }
        /// <summary>
        /// 播放速度；
        /// </summary>
        public int Pitch  { get; set; }
        /// <summary>
        /// 立体声左右声道；
        /// </summary>
        public float StereoPan { get; set; }
        /// <summary>
        /// 声音空间混合量。
        /// </summary>
        public float SpatialBlend { get; set; }
        public float ReverbZoneMix{ get; set; }
        public float DopplerLevel { get; set; }
        public void Release()
        {
        }
    }
}
