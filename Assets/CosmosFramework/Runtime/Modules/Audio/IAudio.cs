using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    /// <summary>
    /// 声音对象接口
    /// </summary>
    public interface IAudio : IReference
    {
        bool Mute { get; }
        bool PlayOnAwake { get; }
        bool Loop { get;  }
        float StereoPan { get; }
        int Priority { get; }
        float Volume { get; }
        float SpatialBlend { get; }
        float Speed { get; }
        AudioClip AudioClip { get; }
        GameObject MountObject { get; }
    }
}