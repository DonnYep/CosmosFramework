using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// 原则上打包发布后时不允许修改的，因此所有继承此类的子类都只有只读属性
    /// </summary>
    public abstract class AudioDataSet : CFDataSet
    {
        [SerializeField]
        protected bool mute = false;
        public bool Mute { get { return mute; } }
        [SerializeField]
        protected bool playOnAwake = false;
        public bool PlayOnAwake { get { return playOnAwake; } }
        [SerializeField]
        protected bool loop = false;
        public bool Loop { get { return loop; } }
        [SerializeField]
        [Range(0, 1)]
        protected  float volume = 1;
        public float Volume { get { return volume; } }
        [SerializeField]
        [Range(0, 1)]
        protected float spatialBlend = 0;
        public float SpatialBlend { get { return spatialBlend; } }
        [SerializeField]
        [Range(-3, 3)]
        protected float speed = 1;
        public float Speed { get { return speed; } }
        public virtual AudioClip AudioClip { get; }
    }
}