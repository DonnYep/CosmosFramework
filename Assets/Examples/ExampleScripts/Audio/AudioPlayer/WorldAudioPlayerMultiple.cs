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
        List<IAudio> audioList=new List<IAudio>();
        public IAudio[] AudioArgs
        {
            get
            {
                audioList.Clear();
                for (int i = 0; i < AudioDataSets.Length; i++)
                {
                    global::Cosmos.AudioObject audioVar = new global::Cosmos.AudioObject();
                    audioVar= SetAudioVariable(audioVar, AudioDataSets[i]);
                    audioList.Add(audioVar);
                }
                return audioList.ToArray();
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
            audioManager.PlayMultipleAudio(AudioAttachTarget, AudioArgs);
        }
        public override void StopAudio()
        {
            audioManager.StopMultipleAudio(AudioAttachTarget);
        }
        public override void PauseAudio()
        {
            audioManager.PauseMultipleAudio(AudioAttachTarget);
        }
        public override void UnpauseAudio()
        {
            audioManager.UnpauseMultipleAudio(AudioAttachTarget);
        }
    }
}

