using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
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
                for (int i = 0; i < AudioEventObjects.Length; i++)
                {
                    AudioEventArgs arg = new AudioEventArgs();
                    arg.AudioEventObject = AudioEventObjects[i];
                    audioArgs.Add(arg);
                }
                return audioArgs.ToArray();
            }
        }
        [SerializeField] AudioEventObject[] audioEventObjects;
        public AudioEventObject[] AudioEventObjects
        {
            get
            {
                List<AudioEventObject> argObject = new List<AudioEventObject>();
                for (short i = 0; i < audioEventObjects.Length; i++)
                {
                    if (audioEventObjects[i] != null)
                    {
                        argObject.Add(audioEventObjects[i]);
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

