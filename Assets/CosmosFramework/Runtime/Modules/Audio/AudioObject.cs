using UnityEngine;
using System;
namespace Cosmos.Audio
{
    /// <summary>
    /// 声音对象
    /// </summary>
    public class AudioObject : IEquatable<AudioObject>
    {
        public string AudioName { get; private set; }
        public AudioClip AudioClip { get; private set; }
        public string AudioGroupName { get; private set; }
        public AudioObject(string audioName, AudioClip audioClip, string audioGroupName)
        {
            AudioName = audioName;
            AudioClip = audioClip;
            AudioGroupName = audioGroupName;
        }
        public AudioObject(string audioName, AudioClip audioClip)
        {
            AudioName = audioName;
            AudioClip = audioClip;
            AudioGroupName = string.Empty;
        }
        public bool Equals(AudioObject other)
        {
            if (AudioClip == null || other.AudioClip == null)
                return false;
            return this.AudioClip == other.AudioClip && this.AudioName == other.AudioName
                && this.AudioGroupName == other.AudioGroupName;
        }
        public override bool Equals(object obj)
        {
            return obj is AudioObject && Equals((AudioObject)obj);
        }
        public override int GetHashCode()
        {
            var hashStr = $"{AudioName}{AudioClip.name}{AudioGroupName}";
            return hashStr.GetHashCode();
        }
        public static bool operator ==(AudioObject a, AudioObject b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(AudioObject a, AudioObject b)
        {
            return !a.Equals(b);
        }
        internal void SetGroup(string audioGroupName)
        {
            this.AudioGroupName = audioGroupName;
        }
    }
}