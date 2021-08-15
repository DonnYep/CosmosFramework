using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Cosmos.Audio
{
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

        IAudioEffectHelper audioEffectHelper;

        IAudioPlayHelper audioPlayHelper;

        AudioGroupPool audioGroupPool;

        public bool Mute { get; set; }

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
                return group.AddAudio(audioName, audio);
            }
            return false;
        }
        public Coroutine RegisterAudioAsync(AudioAssetInfo audioAssetInfo, Action<IAudioObject> callback)
        {
            return audioAssetHelper.LoadAudioAsync(audioAssetInfo, callback);
        }
        public IAudioObject RegisterAudio(AudioAssetInfo audioAssetInfo)
        {
            Utility.Text.IsStringValid(audioAssetInfo.AudioName, "AudioName is invalid !");
            var audioName = audioAssetInfo.AudioName;
            var audioGroupName = audioAssetInfo.AudioGroupName;
            if (audioObjectDict.TryGetValue(audioName, out var audioObject))
                return audioObject;
            audioObject = audioAssetHelper.LoadAudio(audioAssetInfo);
            if (audioObject != null)
            {
                audioObjectDict.Add(audioName, audioObject);
                if (!string.IsNullOrEmpty(audioGroupName))
                {
                    if(!audioGroupDict.TryGetValue(audioGroupName,out var group))
                    {
                        group = audioGroupPool.Spawn();
                        group.AudioGroupName = audioGroupName;
                        audioGroupDict.Add(audioGroupName, group);
                    }
                    group.AddAudio(audioName, audioObject);
                }
            }
            return audioObject;
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
        public void PalyAudioGroup(string audioGroup)
        {

        }
        public void PalyAudio(string audioName, AudioParams audioParams)
        {

        }
        public void PalyAudio(string audioName, string audioGroup, AudioParams audioParams)
        {

        }
        public void PauseAudioGroup(string audioGroup)
        {

        }
        public void PauseAudio(string audioName, AudioParams audioParams)
        {

        }
        public void PauseAudio(string audioName, string audioGroup, AudioParams audioParams)
        {

        }
        public void ResumeAudioGroup(string audioGroup)
        {

        }
        public void ResumeAudio(string audioName, AudioParams audioParams)
        {

        }
        public void ResumeAudio(string audioName, string audioGroup, AudioParams audioParams)
        {

        }
        public void StopAudioGroup(string audioGroup)
        {

        }
        public void StopAudio(string audioName)
        {

        }
        public void StopAllAudios()
        {

        }
        protected override void OnInitialization()
        {
            audioGroupDict = new Dictionary<string, AudioGroup>();
            audioObjectDict = new Dictionary<string, IAudioObject>();
            audioGroupPool = new AudioGroupPool();
            audioAssetHelper = new DefaultAudioAssetHelper();
        }
        [TickRefresh]
        void TickRefresh()
        {
        }
    }
}