using System;
using UnityEngine;
namespace Cosmos.Audio
{
    public interface IAudioAssetHelper
    {
        IAudioObject LoadAudio(AudioAssetInfo assetInfo);
        Coroutine LoadAudioAsync(AudioAssetInfo assetInfo,Action<IAudioObject>callback);
    }
}
