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
        global::Cosmos.AudioObject audioObject = new global::Cosmos.AudioObject();
        public override AudioObject AudioObject { get { return audioObject; } }
        List<LogicEventArgs<IAudio>> audioArgs=new List<LogicEventArgs<IAudio>>();
        public LogicEventArgs<IAudio>[] AudioArgs
        {
            get
            {
                audioArgs.Clear();
                for (int i = 0; i < AudioDataSets.Length; i++)
                {
                    global::Cosmos.AudioObject audioVar = new global::Cosmos.AudioObject();
                    LogicEventArgs<IAudio> args = new LogicEventArgs<IAudio>(audioVar);
                    audioVar= SetAudioVariable(audioVar, AudioDataSets[i]);
                    args.SetData(audioVar);
                    audioArgs.Add(args);
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

