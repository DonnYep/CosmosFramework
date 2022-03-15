using System;
using UnityEngine;
namespace Cosmos.Audio
{
    /// <summary>
    /// 声音资源帮助体；
    /// 可通过实现此接口以自定义声音资源的加载方案；
    /// </summary>
    public interface IAudioAssetHelper
    {
        AudioObject LoadAudio(AudioAssetInfo assetInfo);
        Coroutine LoadAudioAsync(AudioAssetInfo assetInfo, Action<AudioObject> loadSuccess, Action loadFailure);
    }
}
