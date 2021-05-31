using Cosmos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// 声音对象
    /// </summary>
    public class AudioObject : IAudio
    {
        public bool Mute { get; set; }
        public bool PlayOnAwake { get; set; }
        public bool Loop { get; set; }
        public int Priority { get; set; }
        public float Volume { get; set; }
        public float StereoPan { get; set; }
        public float SpatialBlend { get; set; }
        public float Speed { get; set; }
        public virtual AudioClip AudioClip { get; set; }
        public GameObject MountObject { get; set; }
        public void Release()
        {
            Mute = false;
            PlayOnAwake = false;
            Loop = false;
            Priority = 128;
            Volume = 1;
            StereoPan = 0;
            SpatialBlend = 0;
            Speed = 1;
            AudioClip = null;
            MountObject = null;
        }
    }
}