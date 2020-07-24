using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
namespace Cosmos.Audio{
    public interface IAudioController
    {
        void PlayAudio();
        void StopAudio();
        void PauseAudio();
        void UnpauseAudio();
    }
}