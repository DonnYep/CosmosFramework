using System.Collections;
using System.Collections.Generic;
using System;

namespace Cosmos.Audio
{
    //================================================
    // 1、声音资源可设置组别，单体声音资源与组别的关系为一对多映射关系。
    // 2、注册的声音对象名都是唯一的，若重名，则覆写。命名时尽量安规则。
    //================================================
    [Module]
    internal sealed class AudioManager : Module//, IAudioManager
    {
        /// <summary>
        /// AduioGroupName===AudioGroup ;
        /// </summary>
        Dictionary<string, AudioGroup> audioGroupDict;
        /// <summary>
        /// AudioName===AudioObject ;
        /// </summary>
        Dictionary<string, IAudioObject> audioObjectDict;

        IAudioAssetHelper audioAssetHelper;

        IAudioPlayHelper audioPlayHelper;

        AudioGroupPool audioGroupPool;
        Action<AudioRegistSuccessEventArgs> audioRegisterSuccess;


        Action<AudioRegistFailureEventArgs> audioRegisterFailure;
        /// <summary>
        /// 声音注册失败事件；
        /// 回调中参数为失败的资源名称；
        /// </summary>
        public event Action<AudioRegistFailureEventArgs> AudioRegistFailure
        {
            add { audioRegisterFailure += value; }
            remove { audioRegisterFailure -= value; }
        }
        /// <summary>
        /// 声音注册成功事件；
        /// </summary>
        public event Action<AudioRegistSuccessEventArgs> AudioRegisterSuccess
        {
            add { audioRegisterSuccess += value; }
            remove { audioRegisterSuccess -= value; }
        }
        public int AudioCount { get { return audioObjectDict.Count; } }
        public bool Mute { get { return audioPlayHelper.Mute; } set { audioPlayHelper.Mute = value; } }
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
        /// <summary>
        ///注册声音；
        ///若声音原始存在，则更新，若不存在，则加载；
        /// </summary>
        public void RegistAudio(AudioAssetInfo audioAssetInfo)
        {
            Utility.Text.IsStringValid(audioAssetInfo.AudioName, "AudioName is invalid !");
            var audioName = audioAssetInfo.AudioName;
            var audioGroupName = audioAssetInfo.AudioGroupName;
            audioAssetHelper.LoadAudioAsync(audioAssetInfo, audioObj =>
             {
                 if (audioObj != null)
                     OnAudioRegistSuccess(audioObj);
                 else
                     OnAudioRegistFailure(audioAssetInfo.AudioName, audioAssetInfo.AudioGroupName);
             });
        }
        /// <summary>
        /// 注销声音；
        /// </summary>
        /// <param name="audioName">声音名</param>
        /// <returns>是否注销成功</returns>
        public bool DeregisterAudio(string audioName)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            if (audioObjectDict.Remove(audioName, out var audioObject))
            {
                var audioGroupName = audioObject.AudioGroupName;
                if (string.IsNullOrEmpty(audioGroupName))
                    return true;
                if (audioGroupDict.TryGetValue(audioGroupName, out var group))
                {
                    group.RemoveAudio(audioName);
                    if (group.AudioCount <= 0)
                    {
                        audioGroupDict.Remove(audioGroupName);
                        audioGroupPool.Despawn(group);
                    }
                }
                return true;
            }
            return false;
        }
        public void ClearAudioGroup(string audioGroupName)
        {
            Utility.Text.IsStringValid(audioGroupName, "AudioGroupName is invalid !");
            if (audioGroupDict.TryGetValue(audioGroupName, out var group))
            {
                group.RemoveAllAudios();
            }
        }
        public bool HasAudio(string audioName)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            return audioObjectDict.ContainsKey(audioName);
        }
        public bool HasAudioGroup(string audioGroupName)
        {
            Utility.Text.IsStringValid(audioGroupName, "AudioGroupName is invalid !");
            return audioGroupDict.ContainsKey(audioGroupName);
        }
        public void PalyAudioGroup(string audioGroupName)
        {
            Utility.Text.IsStringValid(audioGroupName, "AudioGroupName is invalid !");
            if (audioGroupDict.TryGetValue(audioGroupName, out var group))
            {
                var dict = group.AudioDict;
                foreach (var obj in dict)
                {
                    audioPlayHelper.PlayAudio(obj.Value);
                }
            }
            else
            {
                throw new NullReferenceException($"AudioGroup {audioGroupName} have not been registered ");
            }
        }
        public void PalyAudio(string audioName, AudioParams audioParams,UnityEngine.Vector3 worldPosition)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            if(audioObjectDict.TryGetValue(audioName,out var audioObject))
            {
                audioPlayHelper.PlayAudio(audioObject,audioParams, worldPosition);
            }
            else
            {
                throw new NullReferenceException("IAudioObject have not been registered ");
            }
        }
        public void PauseAudioGroup(string audioGroupName)
        {
            Utility.Text.IsStringValid(audioGroupName, "AudioGroupName is invalid !");
            if (audioGroupDict.TryGetValue(audioGroupName, out var group))
            {
                var dict = group.AudioDict;
                foreach (var obj in dict)
                {
                    audioPlayHelper.PauseAudio(obj.Value);
                }
            }
            else
            {
                throw new NullReferenceException($"AudioGroup {audioGroupName} have not been registered ");
            }
        }
        public void PauseAudio(string audioName)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            if (audioObjectDict.TryGetValue(audioName, out var audioObject))
            {
                audioPlayHelper.PauseAudio(audioObject);
            }
            else
            {
                throw new NullReferenceException("IAudioObject have not been registered ");
            }
        }
        public void UnPauseAudioGroup(string audioGroupName)
        {
            Utility.Text.IsStringValid(audioGroupName, "AudioGroupName is invalid !");
            if (audioGroupDict.TryGetValue(audioGroupName, out var group))
            {
                var dict = group.AudioDict;
                foreach (var obj in dict)
                {
                    audioPlayHelper.UnPauseAudio(obj.Value);
                }
            }
            else
            {
                throw new NullReferenceException($"AudioGroup {audioGroupName} have not been registered ");
            }
        }
        public void UnPauseAudio(string audioName)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            if (audioObjectDict.TryGetValue(audioName, out var audioObject))
            {
                audioPlayHelper.UnPauseAudio(audioObject);
            }
            else
            {
                throw new NullReferenceException("IAudioObject have not been registered ");
            }
        }
        public void StopAudioGroup(string audioGroupName)
        {
            Utility.Text.IsStringValid(audioGroupName, "AudioGroupName is invalid !");
            if(audioGroupDict.TryGetValue(audioGroupName,out var group))
            {
                var dict = group.AudioDict;
                foreach (var obj in dict)
                {
                    audioPlayHelper.StopAudio(obj.Value);
                }
            }
            else
            {
                throw new NullReferenceException($"AudioGroup {audioGroupName} have not been registered ");
            }
        }
        public void StopAudio(string audioName)
        {
            Utility.Text.IsStringValid(audioName, "AudioName is invalid !");
            if (audioObjectDict.TryGetValue(audioName, out var audioObject))
            {
                audioPlayHelper.StopAudio(audioObject);
            }
            else
            {
                throw new NullReferenceException("IAudioObject have not been registered ");
            }
        }
        public void StopAllAudios()
        {
            foreach (var ao in audioObjectDict)
            {
                audioPlayHelper.StopAudio(ao.Value);
            }
        }
        public void PauseAllAudios()
        {
            foreach (var ao in audioObjectDict)
            {
                audioPlayHelper.PauseAudio(ao.Value);
            }
        }
        protected override void OnInitialization()
        {
            audioGroupDict = new Dictionary<string, AudioGroup>();
            audioObjectDict = new Dictionary<string, IAudioObject>();
            audioGroupPool = new AudioGroupPool();
            audioAssetHelper = new DefaultAudioAssetHelper();
            audioPlayHelper = new DefaultAudioPlayHepler();
        }
        //[TickRefresh]
        void TickRefresh()
        {
            
        }
        void OnAudioRegistFailure(string audioName, string audioGroupName)
        {
            var eventArgs = AudioRegistFailureEventArgs.Create(audioName, audioGroupName);
            audioRegisterFailure?.Invoke(eventArgs);
            AudioRegistFailureEventArgs.Release(eventArgs);
        }
        void OnAudioRegistSuccess(IAudioObject audioObject)
        {
            var audioName = audioObject.AudioName;
            var audioGroupName = audioObject.AudioGroupName;
            audioObjectDict.Add(audioName, audioObject);
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