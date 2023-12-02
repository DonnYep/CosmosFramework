using System.Runtime.InteropServices;
using UnityEngine;

namespace Cosmos.Audio
{
    /// <summary>
    /// Transform与WorldPosition是互斥关系，存在绑定对象就无法进行位置(WorldPosition)播放；
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct AudioPlayInfo
    {
        public Vector3 WorldPosition { get; set; }
        public Transform BindObject { get; set; }
        public static AudioPlayInfo Default { get { return new AudioPlayInfo() { WorldPosition = Vector3.zero }; } }
    }
}
