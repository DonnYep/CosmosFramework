using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    [CreateAssetMenu(fileName = "New MultipleAuidoObject", menuName = "CosmosFramework/AudioObject/ MultipleAuidoObject")]
    public class MultipleAuidoObject : AudioEventObject
    {
        [SerializeField]
        AudioClip[] audioClips;
        public AudioClip[] AudioClips { get { return audioClips; } }
        public override void Reset()
        {
            objectName = "New MultipleAudio";
            mute = false;
            playOnAwake = false;
            loop = false;
            volume = 1;
            spatialBlend = 0;
            speed = 1;
            audioClips = new AudioClip[0];
        }
    }
}