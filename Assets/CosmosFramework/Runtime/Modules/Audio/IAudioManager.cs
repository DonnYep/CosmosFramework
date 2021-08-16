using Cosmos.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    public interface IAudioManager :IModuleManager
    {
        /// <summary>
        /// 声音注册失败事件；
        /// 回调中参数为失败的资源名称；
        /// </summary>
        event Action<AudioRegistFailureEventArgs> AudioRegistFailure;
        /// <summary>
        /// 声音注册成功事件；
        /// </summary>
        event Action<AudioRegistSuccessEventArgs> AudioRegisterSuccess;
        int AudioCount { get; }
        bool Mute { get; set ;  }
        bool SetAuidoGroup(string audioName, string audioGroupName);
        /// <summary>
        ///注册声音；
        ///若声音原始存在，则更新，若不存在，则加载；
        /// </summary>
        void RegistAudio(AudioAssetInfo audioAssetInfo);
        /// <summary>
        /// 注销声音；
        /// </summary>
        /// <param name="audioName">声音名</param>
        /// <returns>是否注销成功</returns>
        bool DeregisterAudio(string audioName);
        void ClearAudioGroup(string audioGroupName);
        bool HasAudio(string audioName);
        bool HasAudioGroup(string audioGroupName);
        void PalyAudioGroup(string audioGroupName);
        void PalyAudio(string audioName, AudioParams audioParams, UnityEngine.Vector3 worldPosition);
        void PauseAudioGroup(string audioGroupName);
        void PauseAudio(string audioName);
        void UnPauseAudioGroup(string audioGroupName);
        void UnPauseAudio(string audioName);
        void StopAudioGroup(string audioGroupName);
        void StopAudio(string audioName);
        void StopAllAudios();
        void PauseAllAudios();
    }
}