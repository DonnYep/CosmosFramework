using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    [CreateAssetMenu(fileName = "NewMultipleAuidoDataSet", menuName = "CosmosFramework/AudioDataSet/MultipleAuido")]
    public class MultipleAuidoDataset : AudioDataset
    {
        [SerializeField]
        AudioClip[] audioClips;
        public AudioClip[] AudioClips { get { return audioClips; } }
        public override void Dispose()
        {
            objectName = "NewMultipleAudio";
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