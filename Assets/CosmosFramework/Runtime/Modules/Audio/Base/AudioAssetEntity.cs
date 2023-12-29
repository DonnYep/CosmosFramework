using UnityEngine;
using System;
namespace Cosmos.Audio
{
    /// <summary>
    /// 音效资源实体
    /// </summary>
    internal class AudioAssetEntity : IReference, IEquatable<AudioAssetEntity>
    {
        /// <summary>
        /// 声音资源信息
        /// </summary>
        public string AudioAssetName { get; private set; }
        /// <summary>
        /// 音效资源
        /// </summary>
        public AudioClip AudioClip { get; private set; }
        /// <inheritdoc/>
        public void Release()
        {
            AudioAssetName = string.Empty;
            AudioClip = null;
        }
        /// <summary>
        /// 创建音效实体
        /// </summary>
        /// <param name="audioAssetName">声音资源信息</param>
        /// <param name="audioClip">音效资源</param>
        /// <returns>创建的音效实体</returns>
        public static AudioAssetEntity Create(string audioAssetName, AudioClip audioClip)
        {
            var audioEntity = ReferencePool.Acquire<AudioAssetEntity>();
            audioEntity.AudioAssetName = audioAssetName;
            audioEntity.AudioClip = audioClip;
            return audioEntity;
        }
        /// <summary>
        /// 释放音效资源实体
        /// </summary>
        /// <param name="audioAssetEntity">音效资源实体</param>
        public static void Release(AudioAssetEntity audioAssetEntity)
        {
            ReferencePool.Release(audioAssetEntity);
        }
        public bool Equals(AudioAssetEntity other)
        {
            return other.AudioAssetName == this.AudioAssetName &&
                other.AudioClip == this.AudioClip;
        }
    }
}
