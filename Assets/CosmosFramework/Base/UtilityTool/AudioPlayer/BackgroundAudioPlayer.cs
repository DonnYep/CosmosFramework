using UnityEngine;
using System.Collections;
namespace Cosmos {
    public class BackgroundAudioPlayer :AudioPlayer
    {
        AudioEventArgs audioArgs = new AudioEventArgs();
        [SerializeField] AudioEventObject audioEventObject;
        public override AudioEventObject AudioEventObject { get { return audioEventObject; } }
        public override void PlayAudio()
        {
            audioArgs.AudioEventObject = AudioEventObject;
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