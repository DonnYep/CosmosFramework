using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Audio
{
    public interface IAudioAssetHelper
    {
        IAudioObject LoadAudio(AudioAssetInfo assetInfo);
        Coroutine LoadAudioAsync(AudioAssetInfo assetInfo,Action<IAudioObject>callback);
    }
}
