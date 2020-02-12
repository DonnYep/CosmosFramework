using UnityEngine;
using System.Collections;
namespace Cosmos {
    [DisallowMultipleComponent]
    public class BackgroundAudioPlayer :AudioPlayer
    {
        AudioEventArgs audioArgs = new AudioEventArgs();
        [SerializeField] AudioDataSet audioDataSet;
        public override AudioDataSet AudioDataSet { get { return audioDataSet; } }
        public override void PlayAudio()
        {
            audioArgs.AudioDataSet = AudioDataSet;
            Facade.Instance.PlayBackgroundAudio(audioArgs);
        }
         public override void StopAudio()
        {
            Facade.Instance.StopBackgroundAudio();
        }
         public override void PauseAudio()
        {
            Facade.Instance.PauseBackgroundAudio();
        }
         public override void UnpauseAudio()
        {
            Facade.Instance.UnpauseBackgroundAudio();
        }
    }
}