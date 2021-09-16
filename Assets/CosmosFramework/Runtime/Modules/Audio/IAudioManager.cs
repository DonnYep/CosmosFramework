using Cosmos.Audio;
using System;
using System.Collections.Generic;
namespace Cosmos
{
    //================================================
    /*
     * 1、声音资源可设置组别，单体声音资源与组别的关系为一对多映射关系。
     * 
     * 2、注册的声音对象名都是唯一的，若重名，则覆写。命名时尽量安规则。
     * 
     * 3、播放声音时传入的AudioPlayInfo拥有两个公共属性字段。若BindObject
     * 不为空，则有限绑定，否则是WorldPosition；
     */
    //================================================
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
        /// <summary>
        /// 可播放的声音数量；
        /// </summary>
        int AudioCount { get; }
        /// <summary>
        /// 静音；
        /// </summary>
        bool Mute { get; set ;  }
        void SetAudioAssetHelper(IAudioAssetHelper helper);
        void SetAudioPlayHelper(IAudioPlayHelper helper);
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
        void DeregisterAudio(string audioName);
        bool HasAudio(string audioName);
        bool HasAudioGroup(string audioGroupName);
        /// <summary>
        /// 播放声音；
        /// </summary>
        /// <param name="audioName">注册过的声音名</param>
        /// <param name="audioParams">声音具体参数</param>
        /// <param name="audioPlayInfo">声音播放时候的位置信息以及绑定对象等</param>
        void PalyAudio(string audioName, AudioParams audioParams, AudioPlayInfo audioPlayInfo);
        void PauseAudioGroup(string audioGroupName);
        void PauseAudio(string audioName);
        void UnPauseAudioGroup(string audioGroupName);
        void UnPauseAudio(string audioName);
        void StopAudioGroup(string audioGroupName);
        void StopAudio(string audioName);
        void StopAllAudios();
        void PauseAllAudios();
        void ClearAudioGroup(string audioGroupName);
    }
}