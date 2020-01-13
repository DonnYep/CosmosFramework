using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    [CreateAssetMenu(fileName = "NewEnviroAudioObject", menuName = "CosmosFramework/AudioObject/EnviroAudioObject")]
    public class EnviroAudioObject : AudioEventObject
    {
        [SerializeField]
         AudioClip[] audioClips;
        public override AudioClip AudioClip { get { return audioClips[ Utility.Random(0,audioClips.Length)]; } }
        public override void Reset()
        {
            audioName = "NewEnviroAudio";
            mute = false;
            playOnAwake = false;
            loop = false;
            volume = 1;
            spatialBlend = 0;
            speed = 1;
            audioClips = null;
        }
    }
}