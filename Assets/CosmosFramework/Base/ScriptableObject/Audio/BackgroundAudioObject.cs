using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    [CreateAssetMenu(fileName ="NewBackgroundAduioObject",menuName ="CosmosFramework/AudioObject/BackgroundAudioObject")]
    public class BackgroundAudioObject : AudioEventObject
    {
        [SerializeField] AudioClip aduioCilp;
        public override AudioClip AudioClip { get { return aduioCilp; } }
        public override void Reset()
        {
            audioName = "NewBackgroundAudio";
            mute = false;
            playOnAwake = false;
            loop = false;
            volume = 1;
            spatialBlend = 0;
            speed = 1;
            //clips = null;
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