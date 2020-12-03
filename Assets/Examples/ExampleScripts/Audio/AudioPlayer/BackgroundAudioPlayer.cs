using UnityEngine;
using System.Collections;
using Cosmos.Audio;
namespace Cosmos {
    [DisallowMultipleComponent]
    public class BackgroundAudioPlayer :AudioPlayer
    {
        LogicEventArgs<IAudio> audioArgs = new LogicEventArgs<IAudio>();
        [SerializeField] AudioDataset audioDataSet;
        public override AudioDataset AudioDataSet { get { return audioDataSet; } }
        AudioObject audioObject = new AudioObject();
        public override AudioObject AudioObject { get { return audioObject; } }
        public override void PlayAudio()
        {
            var audio = SetAudioVariable(audioObject, audioDataSet);
            audioArgs.SetData(audio);
            audioManager.PlayBackgroundAudio(audio);
        }
         public override void StopAudio()
        {
            audioManager.StopBackgroundAudio();
        }
         public override void PauseAudio()
        {
            audioManager.PauseBackgroundAudio();
        }
         public override void UnpauseAudio()
        {
            audioManager.UnpauseBackgroundAudio();
        }
    }
}