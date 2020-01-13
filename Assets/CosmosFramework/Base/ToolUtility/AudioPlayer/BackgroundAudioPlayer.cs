using UnityEngine;
using System.Collections;
using Cosmos.Audio;
//using Cosmos.Audio;
namespace Cosmos {
    public class BackgroundAudioPlayer :AudioPlayer
    {
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
            Facade.Instance.UnPauseBackgroundAudio();
        }
    }
}