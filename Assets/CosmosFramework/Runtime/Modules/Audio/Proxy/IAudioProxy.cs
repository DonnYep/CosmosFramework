using System;
using UnityEngine;
namespace Cosmos.Audio
{
    public interface IAudioProxy:IDisposable
    {
        AudioSource AudioSource { get; set; }
        bool IsFading { get; }
        void OnPlay(float fadeTime, AudioParams audioParams);
        void OnUnPause(float fadeTime);
        void OnPause(float fadeTime);
        void OnStop(float fadeTime);
    }
}
