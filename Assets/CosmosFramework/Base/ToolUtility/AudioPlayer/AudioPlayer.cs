using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public abstract class AudioPlayer: MonoBehaviour
    {
        protected Audio.AudioEventArgs audioArgs = new Audio.AudioEventArgs();

        [SerializeField] AudioEventObject audioEventObject;
        public  AudioEventObject AudioEventObject { get { return audioEventObject; }  }
        public abstract void PlayAudio();
        public abstract void PauseAudio();
        public abstract void UnpauseAudio();
        public abstract void StopAudio();
    }
}