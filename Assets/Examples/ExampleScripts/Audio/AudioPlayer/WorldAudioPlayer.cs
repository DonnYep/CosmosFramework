using UnityEngine;
using System.Collections;
namespace Cosmos
{
    [DisallowMultipleComponent]
    public class WorldAudioPlayer : AudioPlayer
    {
        [SerializeField] GameObject audioAttachTarget;
        public GameObject AudioAttachTarget { get { return audioAttachTarget; } set { audioAttachTarget = value; }  }
        [SerializeField] AudioDataSet audioDataSet;
        public override AudioDataSet AudioDataSet { get { return audioDataSet; } }
       AudioObject audioObject = new AudioObject();
        public override AudioObject AudioObject { get { return audioObject; } }
        public override void PlayAudio()
        {
           var audio= SetAudioVariable(audioObject, AudioDataSet);
            audioManager.PlayWorldAudio(AudioAttachTarget, audio);
        }
       public override void StopAudio()
        {
            audioManager.StopWorldAudio(AudioAttachTarget);
        }
       public override void PauseAudio()
        {
            audioManager.PauseWorldAudio(AudioAttachTarget);
        }
      public override void UnpauseAudio()
        {
            audioManager.UnpauseWorldAudio(AudioAttachTarget);
        }
    }
}