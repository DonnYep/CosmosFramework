using System.Collections.Generic;
using System;
namespace Cosmos.Audio
{
    //================================================
    /*
     * 1、声音资源可设置组别，单体声音资源与组别的关系为一对多映射关系。
     * 
     * 2、注册的声音对象名都是唯一的，若重名，则覆写。命名时尽量安规则。
     * 
     * 3、播放声音时传入的AudioPlayInfo拥有两个公共属性字段。若BindObject
     * 不为空，则有限绑定，否则是WorldPosition；
     * 
     * 4、播放声音前需要先对声音资源进行注册，API为  RegistAudioAsync 。
     * 通过监听AudioRegistFailure与AudioRegisterSuccess事件查看注册结果。
     * 注册成功后就可对声音进行播放，暂停，停止等操作。
     */
    //================================================
    [Module]
    internal sealed partial class AudioManager : Module, IAudioManager
    {
        /// <summary>
        /// AduioGroupName===AudioGroup ;
        /// </summary>
        Dictionary<string, IAudioGroup> audioGroupDict;
        /// <summary>
        /// AudioName===AudioObject ;
        /// </summary>
        Dictionary<string, AudioObject> audioObjectDict;
        /// <summary>
        /// 声音资源加载帮助体；
        /// </summary>
        IAudioAssetHelper audioAssetHelper;
        /// <summary>
        /// 声音播放帮助体；
        /// </summary>
        IAudioPlayHelper audioPlayHelper;
        /// <summary>
        /// 声音组池；
        /// </summary>
        AudioGroupPool audioGroupPool;
        Action<AudioRegistSuccessEventArgs> audioRegisterSuccess;
        Action<AudioRegistFailureEventArgs> audioRegisterFailure;
        ///<inheritdoc/>
        public event Action<AudioRegistFailureEventArgs> AudioRegistFailure
        {
            add { audioRegisterFailure += value; }
            remove { audioRegisterFailure -= value; }
        }
        ///<inheritdoc/>
        public event Action<AudioRegistSuccessEventArgs> AudioRegisterSuccess
        {
            add { audioRegisterSuccess += value; }
            remove { audioRegisterSuccess -= value; }
        }
        ///<inheritdoc/>
        public int AudioCount { get { return audioObjectDict.Count; } }
        ///<inheritdoc/>
        public bool Mute { get { return audioPlayHelper.Mute; } set { audioPlayHelper.Mute = value; } }
        ///<inheritdoc/>
        public void SetAudioAssetHelper(IAudioAssetHelper helper)
        {
            if (helper == null)
                throw new NullReferenceException("IAudioAssetHelper is invalid !");
            this.audioAssetHelper = helper;
        }
        ///<inheritdoc/>
        public void SetAudioPlayHelper(IAudioPlayHelper helper)
        {
            if (helper == null)
                throw new NullReferenceException("IAudioPlayHelper  is invalid !");
            this.audioPlayHelper = helper;
        }
        ///<inheritdoc/>
        public void RegistAudioAsync(AudioAssetInfo audioAssetInfo)
        {
            Utility.Text.IsStringValid(audioAssetInfo.AssetName, "AudioName is invalid !");
            audioAssetHelper.LoadAudioAsync(audioAssetInfo, audioObj =>
            {
                OnAudioRegistSuccess(audioObj);
            }, () =>
            {
                OnAudioRegistFailure(audioAssetInfo.AssetName, audioAssetInfo.AudioGroupName);
            });
        }
        ///<inheritdoc/>
        public void DeregisterAudio(string audioName)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            if (audioObjectDict.Remove(audioName, out var audioObject))
            {
                var audioGroupName = audioObject.AudioGroupName;
                if (string.IsNullOrEmpty(audioGroupName))
                    return;
                if (audioGroupDict.TryGetValue(audioGroupName, out var group))
                {
                    group.RemoveAudio(audioName);
                    if (group.AudioCount <= 0)
                    {
                        audioGroupDict.Remove(audioGroupName);
                        audioGroupPool.Despawn(group);
                    }
                }
            }
            else
            {
                throw new ArgumentNullException($"IAudioObject {audioName} is unregistered ");
            }
        }

        #region IndividualAudio
        ///<inheritdoc/>
        public void PlayAudio(string audioName, AudioParams audioParams, AudioPlayInfo audioPlayInfo)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            if (audioObjectDict.TryGetValue(audioName, out var audioObject))
            {
                audioPlayHelper.PlayAudio(audioObject, audioParams, audioPlayInfo);
            }
            else
            {
                throw new ArgumentNullException($"Audio {audioName} is unregistered ");
            }
        }
        ///<inheritdoc/>
        public void PauseAudio(string audioName, float fadeTime = 0)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            if (audioObjectDict.TryGetValue(audioName, out var audioObject))
            {
                audioPlayHelper.PauseAudio(audioObject, fadeTime);
            }
            else
            {
                throw new ArgumentNullException($"Audio {audioName} is unregistered ");
            }
        }
        ///<inheritdoc/>
        public void UnPauseAudio(string audioName, float fadeTime = 0)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            if (audioObjectDict.TryGetValue(audioName, out var audioObject))
            {
                audioPlayHelper.UnPauseAudio(audioObject, fadeTime);
            }
            else
            {
                throw new NullReferenceException("IAudioObject is unregistered ");
            }
        }
        ///<inheritdoc/>
        public void StopAudio(string audioName, float fadeTime = 0)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            if (audioObjectDict.TryGetValue(audioName, out var audioObject))
            {
                audioPlayHelper.StopAudio(audioObject, fadeTime);
            }
            else
            {
                throw new ArgumentNullException($"Audio {audioName} is unregistered ");
            }
        }
        ///<inheritdoc/>
        public bool HasAudio(string audioName)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            return audioObjectDict.ContainsKey(audioName);
        }
        ///<inheritdoc/>
        public void SetAudioParam(string audioName, AudioParams audioParams)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            if (audioObjectDict.TryGetValue(audioName, out var audioObject))
            {
                audioPlayHelper.SetAudioParam(audioObject, audioParams);
            }
            else
            {
                throw new ArgumentNullException($"Audio {audioName} is unregistered ");
            }
        }
        #endregion

        #region AudioGroup
        ///<inheritdoc/>
        public bool SetAuidoGroup(string audioName, string audioGroupName)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            Utility.Text.IsStringValid(audioGroupName, "AudioGroupName is invalid !");
            if (audioObjectDict.TryGetValue(audioName, out var audio))
            {
                if (!audioGroupDict.TryGetValue(audioGroupName, out var group))
                {
                    group = new AudioGroup();
                    group.AudioGroupName = audioGroupName;
                    audioGroupDict.Add(audioGroupName, group);
                }
                group.AddOrUpdateAudio(audioName, audio);
                return true;
            }
            return false;
        }
        ///<inheritdoc/>
        public void PauseAudioGroup(string audioGroupName, float fadeTime = 0)
        {
            Utility.Text.IsStringValid(audioGroupName, "AudioGroupName is invalid !");
            if (audioGroupDict.TryGetValue(audioGroupName, out var group))
            {
                var dict = group.AudioDict;
                foreach (var obj in dict)
                {
                    audioPlayHelper.PauseAudio(obj.Value, fadeTime);
                }
            }
            else
            {
                throw new ArgumentNullException($"AudioGroup {audioGroupName} is unregistered ");
            }
        }
        ///<inheritdoc/>
        public void UnPauseAudioGroup(string audioGroupName, float fadeTime = 0)
        {
            Utility.Text.IsStringValid(audioGroupName, "AudioGroupName is invalid !");
            if (audioGroupDict.TryGetValue(audioGroupName, out var group))
            {
                var dict = group.AudioDict;
                foreach (var obj in dict)
                {
                    audioPlayHelper.UnPauseAudio(obj.Value, fadeTime);
                }
            }
            else
            {
                throw new ArgumentNullException($"AudioGroup {audioGroupName} is unregistered ");
            }
        }
        ///<inheritdoc/>
        public void StopAudioGroup(string audioGroupName, float fadeTime = 0)
        {
            Utility.Text.IsStringValid(audioGroupName, "AudioGroupName is invalid !");
            if (audioGroupDict.TryGetValue(audioGroupName, out var group))
            {
                var dict = group.AudioDict;
                foreach (var obj in dict)
                {
                    audioPlayHelper.StopAudio(obj.Value, fadeTime);
                }
            }
            else
            {
                throw new ArgumentNullException($"AudioGroup {audioGroupName} is unregistered ");
            }
        }
        ///<inheritdoc/>
        public bool HasAudioGroup(string audioGroupName)
        {
            Utility.Text.IsStringValid(audioGroupName, "AudioGroupName is invalid !");
            return audioGroupDict.ContainsKey(audioGroupName);
        }
        ///<inheritdoc/>
        public void ClearAudioGroup(string audioGroupName)
        {
            Utility.Text.IsStringValid(audioGroupName, "AudioGroupName is invalid !");
            if (audioGroupDict.TryGetValue(audioGroupName, out var group))
            {
                group.RemoveAllAudios();
            }
        }
        #endregion

        ///<inheritdoc/>
        public void PauseAllAudios()
        {
            foreach (var ao in audioObjectDict)
            {
                audioPlayHelper.PauseAudio(ao.Value, 0);
            }
        }
        ///<inheritdoc/>
        public void StopAllAudios()
        {
            foreach (var ao in audioObjectDict)
            {
                audioPlayHelper.StopAudio(ao.Value, 0);
            }
        }
        ///<inheritdoc/>
        public void DeregisterAllAudios()
        {
            audioObjectDict.Clear();
            audioGroupDict.Clear();
            audioGroupPool.Clear();
            audioPlayHelper.ClearAllAudio();
        }
        protected override void OnInitialization()
        {
            audioGroupDict = new Dictionary<string, IAudioGroup>();
            audioObjectDict = new Dictionary<string, AudioObject>();
            audioGroupPool = new AudioGroupPool();
            audioAssetHelper = new DefaultAudioAssetHelper();
            audioPlayHelper = new DefaultAudioPlayHelper();
        }
        [TickRefresh]
        void TickRefresh()
        {
            audioPlayHelper?.TickRefresh();
        }
        void OnAudioRegistFailure(string audioName, string audioGroupName)
        {
            var eventArgs = AudioRegistFailureEventArgs.Create(audioName, audioGroupName);
            audioRegisterFailure?.Invoke(eventArgs);
            AudioRegistFailureEventArgs.Release(eventArgs);
        }
        void OnAudioRegistSuccess(AudioObject audioObject)
        {
            var audioName = audioObject.AudioName;
            var audioGroupName = audioObject.AudioGroupName;
            audioObjectDict[audioName] = audioObject;
            if (!string.IsNullOrEmpty(audioGroupName))
            {
                if (!audioGroupDict.TryGetValue(audioGroupName, out var group))
                {
                    group = audioGroupPool.Spawn();
                    group.AudioGroupName = audioGroupName;
                    audioGroupDict.Add(audioGroupName, group);
                }
                group.AddOrUpdateAudio(audioName, audioObject);
            }
            var eventArgs = AudioRegistSuccessEventArgs.Create(audioName, audioGroupName, audioObject.AudioClip);
            audioRegisterSuccess?.Invoke(eventArgs);
            AudioRegistSuccessEventArgs.Release(eventArgs);
        }
    }
}