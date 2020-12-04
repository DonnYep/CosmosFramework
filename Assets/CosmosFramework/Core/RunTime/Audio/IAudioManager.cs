using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    public interface IAudioManager :IModuleManager
    {
        bool Mute { get; set; }
        void PlayBackgroundAudio(IAudio audio);
        void PauseBackgroundAudio();
        void UnpauseBackgroundAudio();
        void StopBackgroundAudio();
        void PlayWorldAudio(GameObject attachTarget, IAudio audio);
        void PauseWorldAudio(GameObject attachTargset);
        void UnpauseWorldAudio(GameObject attachTargset);
        void StopWorldAudio(GameObject attachTargset);
        void StopAllWorldAudio();
        void PlayMultipleAudio(GameObject attachTargset, IAudio[] audios);
        void PauseMultipleAudio(GameObject attachTargset);
        void UnpauseMultipleAudio(GameObject attachTargset);
        void StopMultipleAudio(GameObject attachTargset);
    }
}