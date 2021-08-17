using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Audio
{
    /// <summary>
    /// 声音播放代理对象；
    /// 派生自Mono，AudioSource代理；
    /// </summary>
    public class DefaultAudioPlayHelper : IAudioPlayHelper
    {
        AudioSourcePool pool;
        Dictionary<string, AudioSource> playingDict;
        Dictionary<string, AudioSource> pauseDict;
        List<string> deactiveCache;
        const string prefix = "SND-";
        long latestTime;
        /// <summary>
        /// 间隔5秒；
        /// </summary>
        const int intervalSecond = 5;
        bool mute;
        public bool Mute
        {
            get { return mute; }
            set
            {
                mute = value;
                foreach (var asObj in playingDict)
                {
                    asObj.Value.mute = value;
                }
            }
        }
        public DefaultAudioPlayHelper()
        {
            pool = new AudioSourcePool();
            playingDict = new Dictionary<string, AudioSource>();
            pauseDict = new Dictionary<string, AudioSource>();
            deactiveCache = new List<string>();
            latestTime = Utility.Time.SecondNow();
        }
        public void PlayAudio(IAudioObject audioObject, AudioParams audioParams, AudioPlayInfo audioPlayInfo)
        {
            var audioSource = pool.Spawn();
            audioSource.name = prefix + audioObject.AudioName;
            if (audioPlayInfo.BindObject == null)
            {
                audioSource.transform.SetParent(CosmosEntry.AudioMount.transform);
                audioSource.transform.position = audioPlayInfo.WorldPosition;
            }
            else
                audioSource.transform.SetParent(audioPlayInfo.BindObject);
            audioSource.clip = audioObject.AudioClip;
            audioSource.loop = audioParams.Loop;
            audioSource.priority = audioParams.Priority;
            audioSource.volume = audioParams.Volume;
            audioSource.pitch = audioParams.Pitch;
            audioSource.panStereo = audioParams.StereoPan;
            audioSource.spatialBlend = audioParams.SpatialBlend;
            audioSource.reverbZoneMix = audioParams.ReverbZoneMix;
            audioSource.dopplerLevel = audioParams.DopplerLevel;
            audioSource.spread = audioParams.Spread;
            audioSource.maxDistance = audioParams.MaxDistance;
            audioSource.Play();
            playingDict.AddOrUpdate(audioObject.AudioName, audioSource);
        }
        public void PauseAudio(IAudioObject audioObject)
        {
            if (playingDict.Remove(audioObject.AudioName, out var asObj))
            {
                pauseDict.Add(audioObject.AudioName, asObj);
                asObj.Pause();
            }
        }
        public void UnPauseAudio(IAudioObject audioObject)
        {
            if (pauseDict.Remove(audioObject.AudioName, out var asObj))
            {
                playingDict.Add(audioObject.AudioName, asObj);
                asObj.UnPause();
            }
        }
        public void StopAudio(IAudioObject audioObject)
        {
            if (playingDict.TryGetValue(audioObject.AudioName, out var asObj))
            {
                asObj.Stop();
            }
        }
        public void TickRefresh()
        {
            var now = Utility.Time.SecondNow();
            if (latestTime > now)
            {
                return;
            }
            latestTime = now + intervalSecond;
            var activeLength = playingDict.Count;
            if (activeLength == 0)
                return;
            deactiveCache.Clear();
            foreach (var ao in playingDict)
            {
                if (!ao.Value.isPlaying || ao.Value == null)
                {
                    deactiveCache.Add(ao.Key);
                }
            }
            var deactiveLength = deactiveCache.Count;
            if (deactiveLength == 0)
                return;
            for (int i = 0; i < deactiveLength; i++)
            {
                if (playingDict.Remove(deactiveCache[i], out var asObj))
                {
                    if (asObj != null)
                    {
                        asObj.name = prefix;
                        pool.Despawn(asObj);
                    }
                }
            }
        }

        public void PlayAudio(IAudioObject audioObject)
        {
            throw new NotImplementedException();
        }
    }
}
