using System.Collections.Generic;
using Cosmos.Audio;
using UnityEngine;
namespace Cosmos
{
    /// <summary>
    /// 声音播放代理对象；
    /// AudioSource代理；
    /// </summary>
    public class DefaultAudioPlayHelper : IAudioPlayHelper
    {
        AudioSourcePool pool;
        Dictionary<string, IAudioProxy> playingDict;
        Dictionary<string, IAudioProxy> pauseDict;
        Pool<IAudioProxy> audioProxyPool;
        List<string> deactiveCache;
        long latestTime;
        bool mute;
        public bool Mute
        {
            get { return mute; }
            set
            {
                mute = value;
                foreach (var audioProxy in playingDict)
                {
                    if (audioProxy.Value.AudioSource != null)
                    {
                        audioProxy.Value.AudioSource.mute = value;
                    }
                }
            }
        }
        public DefaultAudioPlayHelper()
        {
            pool = new AudioSourcePool();
            playingDict = new Dictionary<string, IAudioProxy>();
            pauseDict = new Dictionary<string, IAudioProxy>();
            deactiveCache = new List<string>();
            latestTime = Utility.Time.SecondNow();
            audioProxyPool = new Pool<IAudioProxy>(() => { return new AudioProxy(); }, audio => { audio.Dispose(); });
        }
        public void PlayAudio(AudioObject audioObject, AudioParams audioParams, AudioPlayInfo audioPlayInfo)
        {
            if (playingDict.TryGetValue(audioObject.AudioName, out var playingProxy))
            {
                playingProxy.OnPlay(audioParams.FadeInTime, audioParams);
                return;
            }
            if (pauseDict.TryRemove(audioObject.AudioName, out var pausedProxy))
            {
                playingDict.Add(audioObject.AudioName, pausedProxy);
                pausedProxy.OnPlay(audioParams.FadeInTime, audioParams);
                return;
            }
            var audioSource = pool.Spawn();
            audioSource.name = AudioConstant.PREFIX + audioObject.AudioName;
            if (audioPlayInfo.BindObject == null)
            {
                audioSource.transform.SetParent(CosmosEntry.AudioManager.InstanceObject().transform);
                audioSource.transform.position = audioPlayInfo.WorldPosition;
            }
            else
                audioSource.transform.SetParent(audioPlayInfo.BindObject);
            audioSource.clip = audioObject.AudioClip;

            var audioProxy = audioProxyPool.Spawn();
            audioProxy.AudioSource = audioSource;
            audioProxy.OnPlay(audioParams.FadeInTime, audioParams);
            playingDict[audioObject.AudioName] = audioProxy;
        }
        public void PauseAudio(AudioObject audioObject, float fadeTime)
        {
            if (playingDict.Remove(audioObject.AudioName, out var audioProxy))
            {
                pauseDict.Add(audioObject.AudioName, audioProxy);
                audioProxy.OnPause(fadeTime);
            }
        }
        public void UnPauseAudio(AudioObject audioObject, float fadeTime)
        {
            if (pauseDict.Remove(audioObject.AudioName, out var audioProxy))
            {
                playingDict.Add(audioObject.AudioName, audioProxy);
                audioProxy.OnUnPause(fadeTime);
            }
        }
        public void StopAudio(AudioObject audioObject, float fadeTime)
        {
            if (pauseDict.Remove(audioObject.AudioName, out var pausedProxy))
            {
                pausedProxy.OnStop(fadeTime);
                playingDict.Add(audioObject.AudioName, pausedProxy);
            }
            else
            {
                if (playingDict.TryGetValue(audioObject.AudioName, out var playingProxy))
                {
                    playingProxy.OnStop(fadeTime);
                }
            }
        }
        public void SetAudioParam(AudioObject audioObject, AudioParams audioParams)
        {
            IAudioProxy audioProxy = null;
            if (playingDict.TryGetValue(audioObject.AudioName, out audioProxy)
                || pauseDict.TryGetValue(audioObject.AudioName, out audioProxy))
            {
                audioProxy.AudioSource.clip = audioObject.AudioClip;
                audioProxy.AudioSource.loop = audioParams.Loop;
                audioProxy.AudioSource.priority = audioParams.Priority;
                audioProxy.AudioSource.volume = audioParams.Volume;
                audioProxy.AudioSource.pitch = audioParams.Pitch;
                audioProxy.AudioSource.panStereo = audioParams.StereoPan;
                audioProxy.AudioSource.spatialBlend = audioParams.SpatialBlend;
                audioProxy.AudioSource.reverbZoneMix = audioParams.ReverbZoneMix;
                audioProxy.AudioSource.dopplerLevel = audioParams.DopplerLevel;
                audioProxy.AudioSource.spread = audioParams.Spread;
                audioProxy.AudioSource.maxDistance = audioParams.MaxDistance;
            }
        }
        public void ClearAllAudio()
        {
            foreach (var audioProxy in playingDict)
            {
                GameObject.Destroy(audioProxy.Value.AudioSource);
            }
            foreach (var audioProxy in pauseDict)
            {
                GameObject.Destroy(audioProxy.Value.AudioSource);
            }
            pauseDict.Clear();
            playingDict.Clear();
            pool.Clear();
        }
        public void TickRefresh()
        {
            var now = Utility.Time.SecondNow();
            if (latestTime > now)
            {
                return;
            }
            latestTime = now + AudioConstant.CheckPlayingIntervalSecond;
            var activeLength = playingDict.Count;
            if (activeLength == 0)
                return;
            deactiveCache.Clear();
            foreach (var ao in playingDict)
            {
                if (!ao.Value.AudioSource.isPlaying || ao.Value == null)
                {
                    deactiveCache.Add(ao.Key);
                }
            }
            var deactiveLength = deactiveCache.Count;
            if (deactiveLength == 0)
                return;
            for (int i = 0; i < deactiveLength; i++)
            {
                if (playingDict.Remove(deactiveCache[i], out var audioProxy))
                {
                    if (audioProxy != null)
                    {
                        audioProxy.AudioSource.name = AudioConstant.PREFIX;
                        audioProxy.AudioSource.transform.SetParent(CosmosEntry.AudioManager.InstanceObject().transform);
                        pool.Despawn(audioProxy.AudioSource);
                        audioProxyPool.Despawn(audioProxy);
                    }
                }
            }
        }
    }
}
