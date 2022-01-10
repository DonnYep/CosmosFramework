using UnityEngine;
namespace Cosmos.Audio
{
    /// <summary>
    /// 声音对象；
    /// </summary>
    internal class AudioObject : IAudioObject,IReference
    {
        public string AudioName { get; set; }
        public virtual AudioClip AudioClip { get; set; }
        public string AudioGroupName { get; set; }

        public void Release()
        {
            AudioClip = null;
            AudioName = string.Empty;
        }
    }
}