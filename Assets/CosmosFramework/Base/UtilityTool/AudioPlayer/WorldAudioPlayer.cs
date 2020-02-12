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
        AudioEventArgs audioArgs = new AudioEventArgs();
        public override void PlayAudio()
        {
            audioArgs.AudioDataSet = AudioDataSet;
            Facade.Instance.PlayWorldAudio(AudioAttachTarget, audioArgs);
        }
       public override void StopAudio()
        {
            Facade.Instance.StopWorldAudio(AudioAttachTarget);
        }
       public override void PauseAudio()
        {
            Facade.Instance.PauseWorldAudio(AudioAttachTarget);
        }
      public override void UnpauseAudio()
        {
            Facade.Instance.UnpauseWorldAudio(AudioAttachTarget);
        }
    }
}