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
        AudioVariable audioVariable = new AudioVariable();
        public override AudioVariable AudioVariable { get { return audioVariable; } }
        List<LogicEventArgs<AudioVariable>> audioArgs=new List<LogicEventArgs<AudioVariable>>();
        public LogicEventArgs<AudioVariable>[] AudioArgs
        {
            get
            {
                audioArgs.Clear();
                for (int i = 0; i < AudioDataSets.Length; i++)
                {
                    AudioVariable audioVar = new AudioVariable();
                    LogicEventArgs<AudioVariable> arg = new LogicEventArgs<AudioVariable>(audioVar);
                    arg.Data.AudioDataSet= AudioDataSets[i];
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
            Facade.PlayMultipleAudio(AudioAttachTarget, AudioArgs);
        }
        public override void StopAudio()
        {
            Facade.StopMultipleAudio(AudioAttachTarget);
        }
        public override void PauseAudio()
        {
            Facade.PauseMultipleAudio(AudioAttachTarget);
        }
        public override void UnpauseAudio()
        {
            Facade.UnpauseMultipleAudio(AudioAttachTarget);
        }
    }
}

