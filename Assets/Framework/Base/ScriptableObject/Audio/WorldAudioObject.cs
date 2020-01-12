using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    [CreateAssetMenu(fileName = "NewWorldAduioObject", menuName = "CosmosFramework/AudioObject/WorldAudioObject")]
    public class WorldAudioObject : AudioEventObject
    {
        [SerializeField]
         AudioClip[] audioClips;
        public override AudioClip AudioClip { get { return audioClips[ Utility.Random(0,audioClips.Length)]; } }
        public override void Reset()
        {
            audioName = "NewWorldAudio";
            mute = false;
            playOnAwake = false;
            loop = false;
            volume = 1;
            spatialBlend = 0;
            speed = 1;
            audioClips = null;
        }
        public override void MuteAudio()
        {
        }
        public override void PauseAudio()
        {
        }
        public override void PlayAudio()
        {
        }
        public override void StopAudio()
        {
        }
        public override void UnPauseAudio()
        {
        }
    }
}