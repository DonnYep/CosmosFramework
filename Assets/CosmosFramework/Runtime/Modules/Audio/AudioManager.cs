using System.Collections.Generic;
using System;
using UnityEngine;

namespace Cosmos.Audio
{
    //================================================
    /*
     * 1、音效采用注册播放方式。若需要成功播放一个音效，须先注册音效资源。
     * 
     * 2、默认存在一个内置的音效组，不可移除。
     * 
     * 3、音效的播放、暂停、恢复、停止操作，都可传入过渡时间。
     * 
     * 4、相同音效支持多个实体播放。
     * 
     * 5、所有播放声音所返回的serialId都大于等于1。
     * 
     */
    //================================================
    [Module]
    internal sealed partial class AudioManager : Module, IAudioManager
    {
        /// <summary>
        /// AduioGroupName===AudioGroup ;
        /// </summary>
        Dictionary<string, AudioGroup> audioGroupDict;
        /// <summary>
        /// audioAssetName===AudioAssetEntity
        /// </summary>
        Dictionary<string, AudioAssetEntity> audioAssetEntityDict;
        /// <summary>
        /// 声音资源加载帮助体
        /// </summary>
        IAudioAssetHelper audioAssetHelper;
        /// <summary>
        /// 音效资源加载成功事件
        /// </summary>
        Action<AudioRegisterSuccessEventArgs> audioAssetRegisterSuccess;
        /// <summary>
        /// 音效资源加载失败事件
        /// </summary>
        Action<AudioRegisterFailureEventArgs> audioAssetRegisterFailure;
        /// <summary>
        /// 音效加载任务字典，audioAssetName===AudioAssetLoadTask
        /// </summary>
        Dictionary<string, AudioAssetLoadTask> audioAssetLoadTaskDict;
        int serialIndex;
        ///<inheritdoc/>
        public event Action<AudioRegisterFailureEventArgs> AudioAssetRegisterFailure
        {
            add { audioAssetRegisterFailure += value; }
            remove { audioAssetRegisterFailure -= value; }
        }
        ///<inheritdoc/>
        public event Action<AudioRegisterSuccessEventArgs> AudioAssetRegisterSuccess
        {
            add { audioAssetRegisterSuccess += value; }
            remove { audioAssetRegisterSuccess -= value; }
        }
        ///<inheritdoc/>
        public int AudioAssetCount { get { return audioAssetEntityDict.Count; } }
        /// <summary>
        /// 静音
        /// </summary>
        bool mute;
        ///<inheritdoc/>
        public bool Mute
        {
            get { return mute; }
            set
            {
                if (mute != value)
                {
                    mute = value;
                    foreach (var group in audioGroupDict.Values)
                    {
                        group.Mute = mute;
                    }
                }
            }
        }
        ///<inheritdoc/>
        public void SetAudioAssetHelper(IAudioAssetHelper helper)
        {
            if (helper == null)
                throw new NullReferenceException("IAudioAssetHelper is invalid !");
            this.audioAssetHelper = helper;
        }
        ///<inheritdoc/>
        public void RegisterAudioAsset(string audioAssetName)
        {
            Utility.Text.IsStringValid(audioAssetName, "audioAssetName is invalid !");

            if (!audioAssetLoadTaskDict.ContainsKey(audioAssetName))
            {
                var coroutine = audioAssetHelper.LoadAudioAsync(audioAssetName, OnRegisterAudioAssetSuccess, OnRegisterAudioAssetFailure);
                var loadTask = AudioAssetLoadTask.Create(audioAssetName, coroutine);
                audioAssetLoadTaskDict.Add(audioAssetName, loadTask);
            }
        }
        ///<inheritdoc/>
        public void DeregisterAudioAsset(string audioAssetName)
        {
            if (audioAssetLoadTaskDict.Remove(audioAssetName, out var loadTask))
            {
                AudioAssetLoadTask.Release(loadTask);
            }
            audioAssetEntityDict.Remove(audioAssetName);
            OnDeregisterAudioAsset(audioAssetName);
        }
        ///<inheritdoc/>
        public int PlayAudio(string audioAssetName)
        {
            return PlayAudio(audioAssetName, AudioConstant.DEFAULT_AUDIO_GROUP, AudioParams.Default, AudioPositionParams.Default);
        }
        ///<inheritdoc/>
        public int PlayAudio(string audioAssetName, AudioParams audioParams)
        {
            return PlayAudio(audioAssetName, AudioConstant.DEFAULT_AUDIO_GROUP, audioParams, AudioPositionParams.Default);
        }
        ///<inheritdoc/>
        public int PlayAudio(string audioAssetName, AudioParams audioParams, AudioPositionParams audioPositionParams)
        {
            return PlayAudio(audioAssetName, AudioConstant.DEFAULT_AUDIO_GROUP, audioParams, audioPositionParams);
        }
        ///<inheritdoc/>
        public int PlayAudio(string audioAssetName, string audioGroupName, AudioParams audioParams, AudioPositionParams audioPositionParams)
        {
            Utility.Text.IsStringValid(audioAssetName, "audioAssetName is invalid !");
            Utility.Text.IsStringValid(audioGroupName, "audioGroupName is invalid !");
            if (!audioAssetEntityDict.TryGetValue(audioAssetName, out var audioAssetEntity))
                return 0;
            if (!audioGroupDict.TryGetValue(audioGroupName, out var audioGroup))
                return 0;
            var serialId = GetAudioSerialId();
            audioGroup.PlayAudio(serialId, audioAssetEntity, audioParams, audioPositionParams);
            return serialId;
        }
        ///<inheritdoc/>
        public void ReplayAudio(int serialId, float fadeInSeconds)
        {
            foreach (var group in audioGroupDict.Values)
            {
                group.ReplayAudio(serialId, fadeInSeconds);
            }
        }
        ///<inheritdoc/>
        public void PauseAudio(int serialId, float fadeOutSeconds)
        {
            foreach (var group in audioGroupDict.Values)
            {
                group.PauseAudio(serialId, fadeOutSeconds);
            }
        }
        ///<inheritdoc/>
        public void PauseAudios(string audioAssetName, float fadeOutSeconds)
        {
            foreach (var group in audioGroupDict.Values)
            {
                group.PauseAudios(audioAssetName, fadeOutSeconds);
            }
        }
        ///<inheritdoc/>
        public void ResumeAudio(int serialId, float fadeInSeconds)
        {
            foreach (var group in audioGroupDict.Values)
            {
                group.ResumeAudio(serialId, fadeInSeconds);
            }
        }
        ///<inheritdoc/>
        public void ResumeAudios(string audioAssetName, float fadeInSeconds)
        {
            foreach (var group in audioGroupDict.Values)
            {
                group.ResumeAudios(audioAssetName, fadeInSeconds);
            }
        }
        ///<inheritdoc/>
        public void StopAudio(int serialId, float fadeOutSeconds)
        {
            foreach (var group in audioGroupDict.Values)
            {
                group.StopAudio(serialId, fadeOutSeconds);
            }
        }
        ///<inheritdoc/>
        public void StopAudios(string audioAssetName, float fadeOutSeconds)
        {
            foreach (var group in audioGroupDict.Values)
            {
                group.StopAudios(audioAssetName, fadeOutSeconds);
            }
        }
        ///<inheritdoc/>
        public bool HasAudio(int serialId)
        {
            bool hasAudio = false;
            foreach (var group in audioGroupDict.Values)
            {
                var has = group.HasAudio(serialId);
                if (has)
                {
                    hasAudio = has;
                }
            }
            return hasAudio;
        }
        ///<inheritdoc/>
        public bool GetAudioPlayInfo (int serialId, out AudioPlayInfo audioPlayInfo)
        {
            audioPlayInfo = AudioPlayInfo.Default;
            foreach (var group in audioGroupDict.Values)
            {
                var has = group.GetAudioPlayInfo(serialId,out audioPlayInfo);
                if (has)
                {
                    return true;
                }
            }
            return false;
        }
        ///<inheritdoc/>
        public bool IsAudioAssetRegistered(string audioAssetName)
        {
            Utility.Text.IsStringValid(audioAssetName, "audioAssetName is invalid !");
            return audioAssetEntityDict.ContainsKey(audioAssetName);
        }
        ///<inheritdoc/>
        public void SetAudioParams(int serialId, AudioParams audioParams)
        {
            foreach (var group in audioGroupDict.Values)
            {
                group.SetAudioParams(serialId, audioParams);
            }
        }
        ///<inheritdoc/>
        public void SetAudiosParams(string audioAssetName, AudioParams audioParams)
        {
            foreach (var group in audioGroupDict.Values)
            {
                group.SetAudiosParams(audioAssetName, audioParams);
            }
        }
        ///<inheritdoc/>
        public bool HasAudioGroup(string audioGroupName)
        {
            Utility.Text.IsStringValid(audioGroupName, "audioGroupName is invalid !");
            return audioGroupDict.ContainsKey(audioGroupName);
        }
        ///<inheritdoc/>
        public void RemoveAudioGroup(string audioGroupName)
        {
            Utility.Text.IsStringValid(audioGroupName, "audioGroupName is invalid !");
            if (audioGroupName == AudioConstant.DEFAULT_AUDIO_GROUP)
            {
                return;
            }
            if (audioGroupDict.TryRemove(audioGroupName, out var group))
            {
                AudioGroup.Release(group);
            }
        }
        ///<inheritdoc/>
        public IAudioGroup AddAudioGroup(string audioGroupName)
        {
            Utility.Text.IsStringValid(audioGroupName, "audioGroupName is invalid !");
            if (!audioGroupDict.TryGetValue(audioGroupName, out var group))
            {
                group = AudioGroup.Create(audioGroupName);
                audioGroupDict.Add(audioGroupName, group);
            }
            return group;
        }
        ///<inheritdoc/>
        public bool PeekAudioGroup(string audioGroupName, out IAudioGroup group)
        {
            var hasGroup = audioGroupDict.TryGetValue(audioGroupName, out var audioGroup);
            group = audioGroup;
            return hasGroup;
        }
        ///<inheritdoc/>
        public void PauseAllAudios(float fadeOutSecounds)
        {
            foreach (var audioGroup in audioGroupDict.Values)
            {
                audioGroup.PauseAllAudios(fadeOutSecounds);
            }
        }
        ///<inheritdoc/>
        public void StopAllAudios(float fadeOutSecounds)
        {
            foreach (var audioGroup in audioGroupDict.Values)
            {
                audioGroup.StopAllAudios(fadeOutSecounds);
            }
        }
        protected override void OnInitialization()
        {
            audioGroupDict = new Dictionary<string, AudioGroup>();
            audioAssetEntityDict = new Dictionary<string, AudioAssetEntity>();
            audioAssetLoadTaskDict = new Dictionary<string, AudioAssetLoadTask>();
            audioAssetHelper = new DefaultAudioAssetHelper();
            serialIndex = 0;
            var defaultGroup = AudioGroup.Create(AudioConstant.DEFAULT_AUDIO_GROUP);
            audioGroupDict.Add(AudioConstant.DEFAULT_AUDIO_GROUP, defaultGroup);
        }
        void OnRegisterAudioAssetSuccess(string audioAssetName, AudioClip audioClip)
        {
            audioAssetLoadTaskDict.Remove(audioAssetName);
            var currentAudioAssetEntity = AudioAssetEntity.Create(audioAssetName, audioClip);
            if (audioAssetEntityDict.TryRemove(audioAssetName, out var previousAudioAssetEntity))
            {
                AudioAssetEntity.Release(previousAudioAssetEntity);
            }
            audioAssetEntityDict.Add(audioAssetName, currentAudioAssetEntity);
            var eventArgs = AudioRegisterSuccessEventArgs.Create(audioAssetName, audioClip);
            audioAssetRegisterSuccess?.Invoke(eventArgs);
            AudioRegisterSuccessEventArgs.Release(eventArgs);
        }
        void OnRegisterAudioAssetFailure(string audioAssetName, string errorMessage)
        {
            audioAssetLoadTaskDict.Remove(audioAssetName);
            var eventArgs = AudioRegisterFailureEventArgs.Create(audioAssetName, errorMessage);
            audioAssetRegisterFailure?.Invoke(eventArgs);
            AudioRegisterFailureEventArgs.Release(eventArgs);
        }
        void OnDeregisterAudioAsset(string audioAssetName)
        {
            foreach (var group in audioGroupDict.Values)
            {
                group.ReleaseAudios(audioAssetName);
            }
        }
        /// <summary>
        /// 获取音效播放Id，所得的serialId大于等于1
        /// </summary>
        /// <returns>播放Id</returns>
        int GetAudioSerialId()
        {
            if (serialIndex == int.MaxValue)
                serialIndex = 0;
            return serialIndex++;
        }
    }
}