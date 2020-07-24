using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    [CreateAssetMenu(fileName = "NewEnviroAudioDataSet", menuName = "CosmosFramework/AudioDataSet/EnviroAudio")]
    public class EnviroAudioDataSet : AudioDataSet
    {
        [SerializeField]
         AudioClip[] audioClips;
        public override AudioClip AudioClip { get { return audioClips[ Utility.Unity.Random(0,audioClips.Length)]; } }
        public override void Reset()
        {
            objectName= "NewEnviroAudio";
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