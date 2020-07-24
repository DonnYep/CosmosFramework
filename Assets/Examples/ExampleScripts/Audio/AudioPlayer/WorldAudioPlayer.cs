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
        LogicEventArgs<IAudio> audioArgs = new LogicEventArgs<IAudio>();
        public override void PlayAudio()
        {
           var audio= SetAudioVariable(audioObject, AudioDataSet);
            audioArgs.SetData(audio);
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