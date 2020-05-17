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
        AudioVariable audioVariable = new AudioVariable();
        public override AudioVariable AudioVariable { get { return audioVariable; } }
        LogicEventArgs<AudioVariable> audioArgs = new LogicEventArgs<AudioVariable>();
        public override void PlayAudio()
        {
            audioVariable.AudioDataSet = AudioDataSet;
            audioArgs.SetData(audioVariable);
            Facade.PlayWorldAudio(AudioAttachTarget, audioArgs);
        }
       public override void StopAudio()
        {
            Facade.StopWorldAudio(AudioAttachTarget);
        }
       public override void PauseAudio()
        {
            Facade.PauseWorldAudio(AudioAttachTarget);
        }
      public override void UnpauseAudio()
        {
            Facade.UnpauseWorldAudio(AudioAttachTarget);
        }
    }
}