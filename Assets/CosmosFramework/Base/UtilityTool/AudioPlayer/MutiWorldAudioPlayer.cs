using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    public class MutiWorldAudioPlayer : AudioPlayer
    {
       new AudioEventArgs[] audioArgs;
        [SerializeField] AudioEventObject[] audioEventObjects;
        [SerializeField] GameObject audioAttachTarget;
        public GameObject AudioAttachTarget { get { return audioAttachTarget; } set { audioAttachTarget = value; } }
        public override void PauseAudio()
        {
        }
        public override void PlayAudio()
        {
        }
        public override void StopAudio()
        {
        }
        public override void UnpauseAudio()
        {
        }
    }
}

