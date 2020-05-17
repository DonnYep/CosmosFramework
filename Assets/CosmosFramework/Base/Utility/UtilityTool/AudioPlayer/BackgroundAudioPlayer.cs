using UnityEngine;
using System.Collections;
namespace Cosmos {
    [DisallowMultipleComponent]
    public class BackgroundAudioPlayer :AudioPlayer
    {
        LogicEventArgs<AudioVariable> audioArgs = new LogicEventArgs<AudioVariable>();
        [SerializeField] AudioDataSet audioDataSet;
        public override AudioDataSet AudioDataSet { get { return audioDataSet; } }
        AudioVariable audioVariable = new AudioVariable();
        public override AudioVariable AudioVariable { get { return audioVariable; } }
        public override void PlayAudio()
        {
            audioVariable.AudioDataSet = AudioDataSet;
            audioArgs.SetData(audioVariable);
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