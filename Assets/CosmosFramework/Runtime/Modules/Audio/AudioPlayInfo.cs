using UnityEngine;
using System.Runtime.InteropServices;

namespace Cosmos.Audio
{
    /// <summary>
    /// Transform与WorldPosition是互斥关系，存在绑定对象就无法进行位置(WorldPosition)播放；
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct AudioPlayInfo
    {
        float fadeInTime;
        float fadeOutTime;
        /// <summary>
        /// 声音渐入时间；
        /// </summary>
        public float FadeInTime
        {
            get { return fadeInTime; }
            set
            {
                if (value <= 0) fadeInTime = 0;
                else fadeInTime = value;
            }
        }
        /// <summary>
        /// 声音渐出时间；
        /// </summary>
        public float FadeOutTime
        {
            get { return fadeOutTime; }
            set
            {
                if (value <= 0) fadeOutTime = 0;
                else fadeOutTime = value;
            }
        }
        public Vector3 WorldPosition { get; set; }
        public Transform BindObject { get; set; }
        public static AudioPlayInfo Default { get { return new AudioPlayInfo() { WorldPosition = Vector3.zero }; } }
    }
}
