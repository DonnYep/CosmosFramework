using System;
using UnityEngine;
namespace Cosmos.Audio
{
    /// <summary>
    /// 声音资源帮助体，可通过实现此接口以自定义声音资源的加载方案。
    /// </summary>
    public interface IAudioAssetHelper
    {
        /// <summary>
        /// 异步加载音效资产
        /// </summary>
        /// <param name="audioAssetName">音效资产名</param>
        /// <param name="loadSuccess">成功回调</param>
        /// <param name="loadFailure">失败回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadAudioAsync(string audioAssetName , Action<AudioObject> loadSuccess, Action loadFailure);
        /// <summary>
        /// 卸载音效
        /// </summary>
        /// <param name="audioAssetName">音效资产名</param>
        void UnloadAudio(string  audioAssetName);
    }
}
