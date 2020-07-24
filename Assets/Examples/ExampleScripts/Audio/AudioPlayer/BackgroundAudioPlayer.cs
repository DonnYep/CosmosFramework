using UnityEngine;
using System.Collections;
using Cosmos.Audio;
namespace Cosmos {
    [DisallowMultipleComponent]
    public class BackgroundAudioPlayer :AudioPlayer
    {
        LogicEventArgs<IAudio> audioArgs = new LogicEventArgs<IAudio>();
        [SerializeField] AudioDataSet audioDataSet;
        public override AudioDataSet AudioDataSet { get { return audioDataSet; } }
        AudioObject audioObject = new AudioObject();
        public override AudioObject AudioObject { get { return audioObject; } }
        public override void PlayAudio()
        {
            var audio = SetAudioVariable(audioObject, audioDataSet);
            audioArgs.SetData(audio);
            Facade.PlayBackgroundAudio(audioArgs);
        }
         public override void StopAudio()
        {
            Facade.StopBackgroundAudio();
        }
         public override void PauseAudio()
        {
            Facade.PauseBackgroundAudio();
        }
         public override void UnpauseAudio()
        {
            Facade.UnpauseBackgroundAudio();
        }
    }
}