using Cosmos.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public abstract class AudioPlayer: MonoBehaviour
    {
        protected IAudioManager audioManager;
        public virtual AudioDataSet AudioDataSet { get; }
        public virtual AudioObject AudioObject { get; }
        public abstract void PlayAudio();
        public abstract void PauseAudio();
        public abstract void UnpauseAudio();
        public abstract void StopAudio();
       protected AudioObject SetAudioVariable(AudioObject audioVariable, AudioDataSet DataSet)
        {
            audioVariable.Mute = DataSet.Mute;
            audioVariable.Volume = DataSet.Volume;
            audioVariable.Speed = DataSet.Speed;
            audioVariable.PlayOnAwake = DataSet.PlayOnAwake;
            audioVariable.SpatialBlend = DataSet.SpatialBlend;
            audioVariable.Priority = 128;
            audioVariable.Loop = DataSet.Loop;
            audioVariable.AudioClip = DataSet.AudioClip;
            return audioVariable;
        }
       protected virtual void Start()
        {
            audioManager = GameManager.GetModule<IAudioManager>();
        }
    }
}