using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    [DisallowMultipleComponent]
    public class WorldAudioPlayerMultiple : AudioPlayer
    {
        [SerializeField] GameObject audioAttachTarget;
        public GameObject AudioAttachTarget { get { return audioAttachTarget; } set { audioAttachTarget = value; } }

        //AudioEventArgs[] audioArgs;

        List<AudioEventArgs> audioArgs=new List<AudioEventArgs>();
        public AudioEventArgs[] AudioArgs
        {
            get
            {
                audioArgs.Clear();
                for (int i = 0; i < AudioDataSets.Length; i++)
                {
                    AudioEventArgs arg = new AudioEventArgs();
                    arg.AudioDataSet = AudioDataSets[i];
                    audioArgs.Add(arg);
                }
                return audioArgs.ToArray();
            }
        }
        [SerializeField] AudioDataSet[] audioDataSets;
        public AudioDataSet[] AudioDataSets
        {
            get
            {
                List<AudioDataSet> argObject = new List<AudioDataSet>();
                for (short i = 0; i < audioDataSets.Length; i++)
                {
                    if (audioDataSets[i] != null)
                    {
                        argObject.Add(audioDataSets[i]);
                    }
                }
                return argObject.ToArray();
                //return audioEventObjects;
            }
        }
        public override void PlayAudio()
        {
            Facade.Instance.PlayMultipleAudio(AudioAttachTarget, AudioArgs);
        }
        public override void StopAudio()
        {
            Facade.Instance.StopMultipleAudio(AudioAttachTarget);
        }
        public override void PauseAudio()
        {
            Facade.Instance.PauseMultipleAudio(AudioAttachTarget);
        }
        public override void UnpauseAudio()
        {
            Facade.Instance.UnpauseMultipleAudio(AudioAttachTarget);
        }
    }
}

