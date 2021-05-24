using UnityEngine;
using System.Collections;
namespace Cosmos
{
    [DisallowMultipleComponent]
    public class WorldAudioPlayer : AudioPlayer
    {
        [SerializeField] GameObject audioAttachTarget;
        public GameObject AudioAttachTarget { get { return audioAttachTarget; } set { audioAttachTarget = value; }  }
        [SerializeField] AudioDataset audioDataSet;
        public override AudioDataset AudioDataSet { get { return audioDataSet; } }
       AudioObject audioObject = new AudioObject();
        public override AudioObject AudioObject { get { return audioObject; } }
        public override void PlayAudio()
        {
           var audio= SetAudioVariable(audioObject, AudioDataSet);
            AudioManager.PlayWorldAudio(AudioAttachTarget, audio);
        }
       public override void StopAudio()
        {
            AudioManager.StopWorldAudio(AudioAttachTarget);
        }
       public override void PauseAudio()
        {
            AudioManager.PauseWorldAudio(AudioAttachTarget);
        }
      public override void UnpauseAudio()
        {
            AudioManager.UnpauseWorldAudio(AudioAttachTarget);
        }
    }
}