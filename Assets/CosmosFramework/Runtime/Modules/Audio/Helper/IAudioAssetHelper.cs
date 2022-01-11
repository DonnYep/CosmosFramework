using System;
using UnityEngine;
namespace Cosmos.Audio
{
    public interface IAudioAssetHelper
    {
        AudioObject LoadAudio(AudioAssetInfo assetInfo);
        Coroutine LoadAudioAsync(AudioAssetInfo assetInfo,Action<AudioObject>callback);
    }
}
