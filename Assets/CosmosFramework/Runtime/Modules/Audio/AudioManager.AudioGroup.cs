using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Audio
{
    internal sealed partial class AudioManager
    {
        /// <summary>
        /// 声音组；
        /// 这里声音组是一个容器，同时也是组内的"推进者"：不再等到下一次播放才顺带清理，
        /// 而是由管理器每帧 <see cref="Tick"/>：推进过渡 → 判定终止 → 同帧回收播放器。
        /// </summary>
        private class AudioGroup : IAudioGroup, IReference
        {
            /// <summary>
            /// 播放实体列表缓存
            /// </summary>
            readonly List<AudioPlayEntity> audioPlayEntityCache;
            /// <summary>
            /// 播放实体列表，serialId===AudioPlayEntity
            /// </summary>
            readonly Dictionary<int, AudioPlayEntity> audioPlayEntityDict;
            /// <summary>
            /// 组音量，与实体参数音量相乘
            /// </summary>
            float volume;
            /// <summary>
            /// 组内最大同时播放数，0表示不限制
            /// </summary>
            int maxConcurrentAudios;
            /// <summary>
            /// 是否静音
            /// </summary>
            bool mute;
            public AudioGroup()
            {
                audioPlayEntityDict = new Dictionary<int, AudioPlayEntity>();
                audioPlayEntityCache = new List<AudioPlayEntity>();
                volume = AudioConstant.DEFAULT_AUDIO_GROUP_VOLUME;
                maxConcurrentAudios = AudioConstant.DEFAULT_MAX_CONCURRENT_AUDIOS;
                mute = false;
            }
            /// <inheritdoc/>
            public string AudioGroupName { get; private set; }
            /// <inheritdoc/>
            public int AudioPlayEntityCount { get { return audioPlayEntityCache.Count; } }
            /// <inheritdoc/>
            public bool Mute
            {
                get { return mute; }
                set
                {
                    if (mute == value)
                        return;
                    mute = value;
                    var length = audioPlayEntityCache.Count;
                    for (int i = 0; i < length; i++)
                    {
                        audioPlayEntityCache[i].Mute = value;
                    }
                }
            }
            /// <inheritdoc/>
            public float Volume
            {
                get { return volume; }
                set
                {
                    var target = Mathf.Clamp01(value);
                    if (volume == target)
                        return;
                    volume = target;
                    //音量作用在包络之外，因此过渡中的音效也是平滑改音量，不会跳变
                    var length = audioPlayEntityCache.Count;
                    for (int i = 0; i < length; i++)
                    {
                        audioPlayEntityCache[i].ApplyGroupVolume(volume);
                    }
                }
            }
            /// <inheritdoc/>
            public int MaxConcurrentAudios
            {
                get { return maxConcurrentAudios; }
                set { maxConcurrentAudios = Mathf.Max(0, value); }
            }
            /// <summary>
            /// 每帧推进组内所有播放实体，并在同一帧回收已终止的实体
            /// </summary>
            /// <param name="deltaTime">推进时间，单位秒</param>
            public void Tick(float deltaTime)
            {
                var length = audioPlayEntityCache.Count;
                if (length == 0)
                    return;
                var writeIndex = 0;
                for (int i = 0; i < length; i++)
                {
                    var audioPlayEntity = audioPlayEntityCache[i];
                    if (audioPlayEntity.Tick(deltaTime, volume))
                    {
                        audioPlayEntityDict.Remove(audioPlayEntity.SerialId);
                        ReferencePool.Release(audioPlayEntity);
                    }
                    else
                    {
                        //就地压缩，避免 RemoveAt 的多次搬移
                        audioPlayEntityCache[writeIndex++] = audioPlayEntity;
                    }
                }
                if (writeIndex < length)
                    audioPlayEntityCache.RemoveRange(writeIndex, length - writeIndex);
            }
            /// <summary>
            /// 播放
            /// </summary>
            /// <returns>是否成功播放；并发上限已满且优先级不足时返回false</returns>
            public bool PlayAudio(int serialId, AudioAssetEntity audioAssetEntity, AudioParams audioParams, AudioPositionParams audioPositionParams)
            {
                if (maxConcurrentAudios > 0 && audioPlayEntityCache.Count >= maxConcurrentAudios)
                {
                    var victim = FindStealVictim(audioParams.Priority);
                    if (victim == null)
                        return false;
                    victim.StopImmediate();
                    RemoveAudioPlayEntity(victim);
                }
                var newAudioPlayEntity = ReferencePool.Acquire<AudioPlayEntity>();
                newAudioPlayEntity.SerialId = serialId;
                newAudioPlayEntity.OnPlay(audioAssetEntity, audioParams, audioPositionParams, mute, volume);
                if (audioPlayEntityDict.TryGetValue(serialId, out var previousAudioPlayEntity))
                {
                    //序列号回绕的极端情况：先淘汰旧实体，避免出现无人回收的孤儿
                    RemoveAudioPlayEntity(previousAudioPlayEntity);
                }
                audioPlayEntityDict[serialId] = newAudioPlayEntity;
                audioPlayEntityCache.Add(newAudioPlayEntity);
                return true;
            }
            /// <inheritdoc/>
            public void ReplayAudio(int serialId, float fadeInSecounds)
            {
                if (audioPlayEntityDict.TryGetValue(serialId, out var audioPlayEntity))
                {
                    audioPlayEntity.OnReplay(fadeInSecounds);
                }
            }
            /// <inheritdoc/>
            public void PauseAudio(int serialId, float fadeOutSecounds)
            {
                if (audioPlayEntityDict.TryGetValue(serialId, out var audioPlayEntity))
                {
                    audioPlayEntity.OnPause(fadeOutSecounds);
                }
            }
            /// <inheritdoc/>
            public void ResumeAudio(int serialId, float fadeInSecounds)
            {
                if (audioPlayEntityDict.TryGetValue(serialId, out var audioPlayEntity))
                {
                    audioPlayEntity.OnResume(fadeInSecounds);
                }
            }
            /// <inheritdoc/>
            public void StopAudio(int serialId, float fadeOutSecounds)
            {
                if (audioPlayEntityDict.TryGetValue(serialId, out var audioPlayEntity))
                {
                    audioPlayEntity.OnStop(fadeOutSecounds);
                }
            }
            /// <summary>
            /// 暂停组内所有使用指定资源的声音实体
            /// </summary>
            public void PauseAudios(string audioAssetName, float fadeOutSecounds)
            {
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    var audioPlayEntity = audioPlayEntityCache[i];
                    if (audioPlayEntity.AudioAssetName == audioAssetName)
                    {
                        audioPlayEntity.OnPause(fadeOutSecounds);
                    }
                }
            }
            /// <summary>
            /// 恢复组内所有使用指定资源的声音实体
            /// </summary>
            public void ResumeAudios(string audioAssetName, float fadeInSecounds)
            {
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    var audioPlayEntity = audioPlayEntityCache[i];
                    if (audioPlayEntity.AudioAssetName == audioAssetName)
                    {
                        audioPlayEntity.OnResume(fadeInSecounds);
                    }
                }
            }
            /// <summary>
            /// 停止组内所有使用指定资源的声音实体
            /// </summary>
            public void StopAudios(string audioAssetName, float fadeOutSecounds)
            {
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    var audioPlayEntity = audioPlayEntityCache[i];
                    if (audioPlayEntity.AudioAssetName == audioAssetName)
                    {
                        audioPlayEntity.OnStop(fadeOutSecounds);
                    }
                }
            }
            /// <inheritdoc/>
            public void PauseAudios(float fadeOutSecounds)
            {
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    audioPlayEntityCache[i].OnPause(fadeOutSecounds);
                }
            }
            /// <inheritdoc/>
            public void ResumeAudios(float fadeInSecounds)
            {
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    audioPlayEntityCache[i].OnResume(fadeInSecounds);
                }
            }
            /// <inheritdoc/>
            public void StopAudios(float fadeOutSecounds)
            {
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    audioPlayEntityCache[i].OnStop(fadeOutSecounds);
                }
            }
            /// <inheritdoc/>
            public void SetAudioParams(int serialId, AudioParams audioParams)
            {
                if (audioPlayEntityDict.TryGetValue(serialId, out var audioPlayEntity))
                {
                    audioPlayEntity.SetAudioParams(audioParams);
                }
            }
            /// <inheritdoc/>
            public void SetAudiosParams(string audioAssetName, AudioParams audioParams)
            {
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    var audioPlayEntity = audioPlayEntityCache[i];
                    if (audioPlayEntity.AudioAssetName == audioAssetName)
                    {
                        audioPlayEntity.SetAudioParams(audioParams);
                    }
                }
            }
            /// <inheritdoc/>
            public bool HasAudio(int serialId)
            {
                return audioPlayEntityDict.ContainsKey(serialId);
            }
            /// <inheritdoc/>
            public bool GetAudioPlayInfo(int serialId, out AudioPlayInfo audioPlayInfo)
            {
                audioPlayInfo = AudioPlayInfo.Default;
                if (!audioPlayEntityDict.TryGetValue(serialId, out var audioPlayEntity))
                    return false;
                audioPlayInfo = new AudioPlayInfo(
                    audioPlayEntity.AudioAssetName,
                    AudioGroupName,
                    audioPlayEntity.SerialId,
                    audioPlayEntity.AudioPlayStatusType,
                    audioPlayEntity.CurrentVolume,
                    audioPlayEntity.PlaybackTime);
                return true;
            }
            /// <inheritdoc/>
            public void ReleaseAudios(string audioAssetName)
            {
                var length = audioPlayEntityCache.Count;
                if (length == 0)
                    return;
                var writeIndex = 0;
                for (int i = 0; i < length; i++)
                {
                    var audioPlayEntity = audioPlayEntityCache[i];
                    if (audioPlayEntity.AudioAssetName == audioAssetName)
                    {
                        audioPlayEntityDict.Remove(audioPlayEntity.SerialId);
                        ReferencePool.Release(audioPlayEntity);
                    }
                    else
                    {
                        audioPlayEntityCache[writeIndex++] = audioPlayEntity;
                    }
                }
                if (writeIndex < length)
                    audioPlayEntityCache.RemoveRange(writeIndex, length - writeIndex);
            }
            /// <inheritdoc/>
            public void Release()
            {
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    //归还 AudioSource 后实体自身也回到引用池
                    ReferencePool.Release(audioPlayEntityCache[i]);
                }
                audioPlayEntityCache.Clear();
                audioPlayEntityDict.Clear();
                volume = AudioConstant.DEFAULT_AUDIO_GROUP_VOLUME;
                maxConcurrentAudios = AudioConstant.DEFAULT_MAX_CONCURRENT_AUDIOS;
                mute = false;
                AudioGroupName = string.Empty;
            }
            /// <summary>
            /// 移除并回收单个播放实体
            /// </summary>
            /// <param name="audioPlayEntity">目标播放实体</param>
            void RemoveAudioPlayEntity(AudioPlayEntity audioPlayEntity)
            {
                audioPlayEntityDict.Remove(audioPlayEntity.SerialId);
                audioPlayEntityCache.Remove(audioPlayEntity);
                ReferencePool.Release(audioPlayEntity);
            }
            /// <summary>
            /// 并发上限已满时挑选可被抢占的实体：
            /// 取组内优先级最低（priority数值最大）的实体，若新音效优先级不高于它则抢占，
            /// 否则本次播放放弃（与 Unity 自身"满员时丢弃低优先级新音效"一致）。
            /// </summary>
            /// <param name="priority">新音效优先级</param>
            /// <returns>可抢占的实体，null表示不抢占</returns>
            AudioPlayEntity FindStealVictim(int priority)
            {
                AudioPlayEntity victim = null;
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    var audioPlayEntity = audioPlayEntityCache[i];
                    if (!audioPlayEntity.IsInterruptible)
                        continue;
                    //同优先级时保留先来的（列表顺序即播放顺序）
                    if (victim == null || audioPlayEntity.AudioPriority > victim.AudioPriority)
                        victim = audioPlayEntity;
                }
                if (victim == null)
                    return null;
                return priority <= victim.AudioPriority ? victim : null;
            }
            /// <summary>
            /// 创建音效组
            /// </summary>
            /// <param name="audioGroupName">音效组名</param>
            /// <returns>音效组</returns>
            public static AudioGroup Create(string audioGroupName)
            {
                var audioGroup = ReferencePool.Acquire<AudioGroup>();
                audioGroup.AudioGroupName = audioGroupName;
                audioGroup.volume = AudioConstant.DEFAULT_AUDIO_GROUP_VOLUME;
                audioGroup.maxConcurrentAudios = AudioConstant.DEFAULT_MAX_CONCURRENT_AUDIOS;
                audioGroup.mute = false;
                return audioGroup;
            }
            /// <summary>
            /// 释放音效组
            /// </summary>
            /// <param name="audioGroup">音效组</param>
            public static void Release(AudioGroup audioGroup)
            {
                ReferencePool.Release(audioGroup);
            }
        }
    }
}
