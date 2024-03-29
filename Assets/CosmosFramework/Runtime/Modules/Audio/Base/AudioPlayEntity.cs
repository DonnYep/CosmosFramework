﻿using System.Collections;
using UnityEngine;

namespace Cosmos.Audio
{
    /// <summary>
    /// 音效播放实体
    /// </summary>
    internal class AudioPlayEntity : IReference
    {
        /// <summary>
        /// unity的声音播放器
        /// </summary>
        public AudioSource AudioSource { get; set; }
        /// <summary>
        /// 音效资源名
        /// </summary>
        public string AudioAssetName { get; set; }
        /// <summary>
        /// 音效序列号
        /// </summary>
        public int SerialId { get; set; }
        /// <summary>
        /// 静音
        /// </summary>
        public bool Mute
        {
            get
            {
                if (AudioSource == null)
                    return true;
                return AudioSource.mute;
            }
            set
            {
                if (AudioSource != null)
                    AudioSource.mute = value;
            }
        }
        /// <summary>
        /// 声音播放状态
        /// </summary>
        public AudioPlayStatusType AudioPlayStatusType { get; set; }
        /// <summary>
        /// 协程对象
        /// </summary>
        Coroutine coroutine;
        /// <summary>
        /// 播放参数缓存
        /// </summary>
        AudioParams audioParams;
        /// <summary>
        /// 声音播放位置参数
        /// </summary>
        AudioPositionParams audioPositionParams;
        public void OnPlay(AudioAssetEntity audioAssetEntity, AudioParams audioParams, AudioPositionParams audioPositionParams)
        {
            var audioClip = audioAssetEntity.AudioClip;
            this.audioPositionParams = audioPositionParams;
            this.audioParams = audioParams;
            AudioSource = AudioSourcePool.Spawn();
            AudioSource.clip = audioClip;
            AudioSource.loop = audioParams.Loop;
            AudioSource.priority = audioParams.Priority;
            AudioSource.volume = audioParams.Volume;
            AudioSource.pitch = audioParams.Pitch;
            AudioSource.panStereo = audioParams.StereoPan;
            AudioSource.spatialBlend = audioParams.SpatialBlend;
            AudioSource.reverbZoneMix = audioParams.ReverbZoneMix;
            AudioSource.dopplerLevel = audioParams.DopplerLevel;
            AudioSource.spread = audioParams.Spread;
            AudioSource.maxDistance = audioParams.MaxDistance;
            AudioSource.name = AudioConstant.PREFIX + audioAssetEntity.AudioAssetName;
            if (audioPositionParams.BindParent == null)
            {
                AudioSource.transform.SetParent(CosmosEntry.AudioManager.InstanceObject().transform);
                AudioSource.transform.position = audioPositionParams.WorldPosition;
            }
            else
            {
                AudioSource.transform.SetParent(audioPositionParams.BindParent);
            }
            if (coroutine != null)
                Utility.Unity.StopCoroutine(coroutine);
            coroutine = Utility.Unity.StartCoroutine(EnumPlay(audioParams.FadeInSeconds));
            AudioPlayStatusType = AudioPlayStatusType.Play;
        }
        public void OnReplay(float fadeInSecounds)
        {
            if (coroutine != null)
                Utility.Unity.StopCoroutine(coroutine);
            if (AudioSource == null)
            {
                //当AudioSource空引用时，可能作为某个物体的子物体被销毁了。
                //这里直接标记为停止状态，等待下一个播放音效进行回收。
                AudioPlayStatusType = AudioPlayStatusType.Stop;
            }
            else
            {
                if (audioPositionParams.BindParent == null)
                {
                    AudioSource.transform.SetParent(CosmosEntry.AudioManager.InstanceObject().transform);
                    AudioSource.transform.position = audioPositionParams.WorldPosition;
                }
                else
                {
                    AudioSource.transform.SetParent(audioPositionParams.BindParent);
                }
                AudioSource.time = 0;
                coroutine = Utility.Unity.StartCoroutine(EnumPlay(fadeInSecounds));
            }
        }
        public void OnStop(float fadeOutSecounds)
        {
            if (coroutine != null)
                Utility.Unity.StopCoroutine(coroutine);
            if (AudioSource == null)
            {
                //当AudioSource空引用时，可能作为某个物体的子物体被销毁了。
                //这里直接标记为停止状态，等待下一个播放音效进行回收。
                AudioPlayStatusType = AudioPlayStatusType.Stop;
            }
            else
            {
                coroutine = Utility.Unity.StartCoroutine(EnumStop(fadeOutSecounds));
            }
        }
        public void OnPause(float fadeOutSecounds)
        {
            if (coroutine != null)
                Utility.Unity.StopCoroutine(coroutine);
            if (AudioSource == null)
            {
                AudioPlayStatusType = AudioPlayStatusType.Stop;
            }
            else
            {
                coroutine = Utility.Unity.StartCoroutine(EnumPause(fadeOutSecounds));
            }
        }
        public void OnResume(float fadeInSecounds)
        {
            if (coroutine != null)
                Utility.Unity.StopCoroutine(coroutine);
            if (AudioSource == null)
            {
                AudioPlayStatusType = AudioPlayStatusType.Stop;
            }
            else
            {
                coroutine = Utility.Unity.StartCoroutine(EnumResume(fadeInSecounds));
            }
        }
        public void SetAudioParams(AudioParams audioParams)
        {
            this.audioParams = audioParams;
            AudioSource.loop = audioParams.Loop;
            AudioSource.priority = audioParams.Priority;
            AudioSource.volume = audioParams.Volume;
            AudioSource.pitch = audioParams.Pitch;
            AudioSource.panStereo = audioParams.StereoPan;
            AudioSource.spatialBlend = audioParams.SpatialBlend;
            AudioSource.reverbZoneMix = audioParams.ReverbZoneMix;
            AudioSource.dopplerLevel = audioParams.DopplerLevel;
            AudioSource.spread = audioParams.Spread;
            AudioSource.maxDistance = audioParams.MaxDistance;
        }
        public void Release()
        {
            if (coroutine != null)
                Utility.Unity.StopCoroutine(coroutine);
            AudioSourcePool.Despawn(AudioSource);
            AudioSource = null;
            audioParams = AudioParams.Default;
            SerialId = 0;
            AudioPlayStatusType = AudioPlayStatusType.None;
            coroutine = null;
        }
        IEnumerator EnumPlay(float seconds)
        {
            if (seconds <= 0)
            {
                AudioSource.Play();
                yield break;
            }
            AudioSource.volume = 0;
            AudioSource.Play();
            while (AudioSource.volume < audioParams.Volume)
            {
                AudioSource.volume += Time.deltaTime / seconds;
                yield return null;
            }
            AudioSource.volume = audioParams.Volume;
            if (!audioParams.Loop)
            {
                yield return new WaitUntil(() => { return !AudioSource.isPlaying; });
                AudioPlayStatusType = AudioPlayStatusType.Stop;

            }
        }
        IEnumerator EnumResume(float seconds)
        {
            if (seconds <= 0)
            {
                AudioSource.UnPause();
                yield break;
            }
            AudioSource.volume = 0;
            AudioSource.UnPause();
            while (AudioSource.volume < audioParams.Volume)
            {
                AudioSource.volume += Time.deltaTime / seconds;
                yield return null;
            }
            AudioSource.volume = audioParams.Volume;
            AudioPlayStatusType = AudioPlayStatusType.Play;
            if (!audioParams.Loop)
            {
                yield return new WaitUntil(() => { return !AudioSource.isPlaying; });
                AudioPlayStatusType = AudioPlayStatusType.Stop;
            }
        }
        IEnumerator EnumPause(float seconds)
        {
            if (seconds <= 0)
            {
                AudioSource.Pause();
                yield break;
            }
            float startVolume = AudioSource.volume;
            while (AudioSource.volume > 0)
            {
                AudioSource.volume -= Time.deltaTime / seconds;
                yield return null;
            }
            AudioSource.Pause();
            AudioSource.volume = startVolume;
            AudioPlayStatusType = AudioPlayStatusType.Pause;
        }
        IEnumerator EnumStop(float seconds)
        {
            if (seconds <= 0)
            {
                AudioSource.Stop();
                yield break;
            }
            float startVolume = AudioSource.volume;
            while (AudioSource.volume > 0)
            {
                AudioSource.volume -= Time.deltaTime / seconds;
                yield return null;
            }
            AudioSource.Stop();
            AudioSource.volume = startVolume;
            AudioPlayStatusType = AudioPlayStatusType.Stop;
        }
    }
}
