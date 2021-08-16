using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos.Audio
{
    public interface IAudioPlayHelper
    {
        bool Mute { get; set; }
        void PlayAudio(IAudioObject audioObject,AudioParams audioParams, Vector3 wordPosition);
        void PlayAudio(IAudioObject audioObject);
        void StopAudio(IAudioObject audioObject);
        void PauseAudio(IAudioObject audioObject);
        void UnPauseAudio(IAudioObject audioObject);
    }
}
