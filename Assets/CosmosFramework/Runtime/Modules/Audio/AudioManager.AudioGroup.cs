using System.Collections.Generic;
namespace Cosmos.Audio
{
    internal sealed partial class AudioManager
    {
        /// <summary>
        /// 声音组；
        /// 这里声音组是一个容器，用于存储IAudioObject，逻辑由MGR执行；
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
            /// 用于移除的音效播放实体缓存
            /// </summary>
            readonly List<int> removalSerialIdCache;
            /// <inheritdoc/>
            public string AudioGroupName { get; private set; }
            /// <inheritdoc/>
            public int AudioPlayEntityCount { get { return audioPlayEntityDict.Count; } }
            /// <summary>
            /// 是否静音
            /// </summary>
            bool mute;
            public AudioGroup()
            {
                audioPlayEntityDict = new Dictionary<int, AudioPlayEntity>();
                audioPlayEntityCache = new List<AudioPlayEntity>();
                removalSerialIdCache = new List<int>();
                mute = false;
            }
            /// <inheritdoc/>
            public bool Mute
            {
                get { return mute; }
                set
                {
                    if (mute != value)
                    {
                        var length = audioPlayEntityCache.Count;
                        for (int i = 0; i < length; i++)
                        {
                            var audioPlayEntity = audioPlayEntityCache[i];
                            audioPlayEntity.Mute = mute;
                        }
                        mute = value;
                    }
                }
            }
            public void PlayAudio(int serialId, AudioAssetEntity audioAssetEntity, AudioParams audioParams, AudioPositionParams audioPositionParams)
            {
                removalSerialIdCache.Clear();
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    var audioPlayEntity = audioPlayEntityCache[i];
                    if (!audioPlayEntity.IsPlaying)
                    {
                        removalSerialIdCache.Add(audioPlayEntity.SerialId);
                    }
                }
                var removalLength = removalSerialIdCache.Count;
                for (int i = 0; i < removalLength; i++)
                {
                    var removalSerialId = removalSerialIdCache[i];

                    audioPlayEntityDict.Remove(removalSerialId, out var removalAudioPlayEntity);
                    audioPlayEntityCache.Remove(removalAudioPlayEntity);

                    ReferencePool.Release(removalAudioPlayEntity);
                }
                removalSerialIdCache.Clear();

                var newAudioPlayEntity = ReferencePool.Acquire<AudioPlayEntity>();
                newAudioPlayEntity.SerialId = serialId;
                newAudioPlayEntity.OnPlay(audioAssetEntity, audioParams, audioPositionParams);

                audioPlayEntityDict.Add(serialId, newAudioPlayEntity);
                audioPlayEntityCache.Add(newAudioPlayEntity);
            }
            public void ReplayAudio(int serialId, float fadeInSecounds)
            {
                if (audioPlayEntityDict.TryGetValue(serialId, out var audioPlayEntity))
                {
                    audioPlayEntity.OnReplay(fadeInSecounds);
                }
            }
            public void PauseAudio(int serialId, float fadeOutSecounds)
            {
                if (audioPlayEntityDict.TryGetValue(serialId, out var audioPlayEntity))
                {
                    audioPlayEntity.OnPause(fadeOutSecounds);
                }
            }
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
            public void ResumeAudio(int serialId, float fadeInSecounds)
            {
                if (audioPlayEntityDict.TryGetValue(serialId, out var audioPlayEntity))
                {
                    audioPlayEntity.OnResume(fadeInSecounds);
                }
            }
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
            public void StopAudio(int serialId, float fadeOutSecounds)
            {
                if (audioPlayEntityDict.TryGetValue(serialId, out var audioPlayEntity))
                {
                    audioPlayEntity.OnStop(fadeOutSecounds);
                }
            }
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
            public void PauseAllAudios(float fadeOutSecounds)
            {
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    var audioPlayEntity = audioPlayEntityCache[i];
                    audioPlayEntity.OnPause(fadeOutSecounds);
                }
            }
            public void StopAllAudios(float fadeOutSecounds)
            {
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    var audioPlayEntity = audioPlayEntityCache[i];
                    audioPlayEntity.OnStop(fadeOutSecounds);
                }
            }
            public void SetAudioParams(int serialId, AudioParams audioParams)
            {
                if (audioPlayEntityDict.TryGetValue(serialId, out var audioPlayEntity))
                {
                    audioPlayEntity.SetAudioParams(audioParams);
                }
            }
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
            public bool HasAudio(int serialId)
            {
                return audioPlayEntityDict.ContainsKey(serialId);
            }
            public void ReleaseAudios(string audioAssetName)
            {
                removalSerialIdCache.Clear();
                var length = audioPlayEntityCache.Count;
                for (int i = 0; i < length; i++)
                {
                    var audioPlayEntity = audioPlayEntityCache[i];
                    if (audioPlayEntity.AudioAssetName==audioAssetName)
                    {
                        removalSerialIdCache.Add(audioPlayEntity.SerialId);
                    }
                }
                var removalLength = removalSerialIdCache.Count;
                for (int i = 0; i < removalLength; i++)
                {
                    var removalSerialId = removalSerialIdCache[i];

                    audioPlayEntityDict.Remove(removalSerialId, out var removalAudioPlayEntity);
                    audioPlayEntityCache.Remove(removalAudioPlayEntity);

                    ReferencePool.Release(removalAudioPlayEntity);
                }
                removalSerialIdCache.Clear();
            }
            public void Release()
            {
                ReferencePool.Release(audioPlayEntityCache);
                audioPlayEntityCache.Clear();
                audioPlayEntityDict.Clear();
                removalSerialIdCache.Clear();
                mute = false;
                AudioGroupName = string.Empty;
            }
            public static AudioGroup Create(string audioGroupName)
            {
                var audioGroup = ReferencePool.Acquire<AudioGroup>();
                audioGroup.AudioGroupName = audioGroupName;
                return audioGroup;
            }
            public static void Release(AudioGroup audioGroup)
            {
                ReferencePool.Release(audioGroup);
            }
        }
    }
}