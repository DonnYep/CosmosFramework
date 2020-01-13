using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public class AudioPlayer: MonoBehaviour {

       Audio. AudioEventArgs args=new Audio.AudioEventArgs();
        [SerializeField] AudioEventObject audioEventObject;
        public  void PlayBackgroundAudio()
        {
           args.AudioEventObject= audioEventObject;
            Facade.Instance.PlayBackgroundAudio(args);
        }
        public void StopBackgroundAudio()
        {
            Facade.Instance.StopBackgroundAudio();
        }
        public void PauseBackgroundAudio()
        {
            Facade.Instance.PauseBackgroundAudio();
        }
        public void UnPauseBackgroundAudio()
        {
            Facade.Instance.UnPauseBackgroundAudio();
        }
        public void PlayWorldAudio(GameObject attachTarget)
        {
            args.AudioEventObject = audioEventObject;
            Facade.Instance.PlayWorldAudio(attachTarget, args);
        }
        public void StopWorldAudio(GameObject attachTarget)
        {
            Facade.Instance.StopWorldAudio(attachTarget);
        }
        public void PauseWorldAudio(GameObject attachTarget)
        {
            Facade.Instance.PauseWorldAudio(attachTarget);
        }
        public void UnPauseWorldAudio(GameObject attachTarget)
        {
            Facade.Instance.UnPauseWorldAudio(attachTarget);
        }
    }
}