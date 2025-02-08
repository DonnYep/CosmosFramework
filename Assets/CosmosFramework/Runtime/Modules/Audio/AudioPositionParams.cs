using System.Runtime.InteropServices;
using UnityEngine;

namespace Cosmos.Audio
{
    /// <summary>
    /// Transform与WorldPosition是互斥关系，存在绑定对象就无法进行位置(WorldPosition)播放。
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct AudioPositionParams
    {
        /// <summary>
        /// 播放的世界位置
        /// <para>注意，此参数与<see cref="BindParent"/>互斥</para>
        /// <para>当此<see cref="BindParent"/>不为空时，优先使用绑定位置播放</para>
        /// </summary>
        public Vector3 WorldPosition { get; set; }
        /// <summary>
        /// 绑定的父对象
        /// <para>注意，此参数与<see cref="WorldPosition"/>互斥</para>
        /// <para>当此参数不为空时，优先使用绑定位置播放</para>
        /// </summary>
        public Transform BindParent { get; set; }
        /// <summary>
        /// 默认参数
        /// </summary>
        public readonly static AudioPositionParams Default = new AudioPositionParams();
    }
}
