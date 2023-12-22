using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Audio;

namespace Cosmos
{
    public class AudioPlayHelper
    {
        //音效播放帮助体应支持同一个音效资源多个实体播放
        AudioSourcePool audioSourcePool;
        readonly List<IAudioProxy> audioProxyList;
        readonly Dictionary<int, IAudioProxy> audioProxyDict = new Dictionary<int, IAudioProxy>();
        Pool<IAudioProxy> audioProxyPool;
        long latestTime;
        bool mute;
        public bool Mute
        {
            get { return mute; }
            set
            {
                mute = value;
                foreach (var audioProxy in audioProxyList)
                {
                    if (audioProxy.AudioSource != null)
                    {
                        audioProxy.AudioSource.mute = value;
                    }
                }
            }
        }
        public AudioPlayHelper()
        {
            audioSourcePool = new AudioSourcePool();
            audioProxyList = new List<IAudioProxy>();
            latestTime = Utility.Time.SecondNow();
            audioProxyPool = new Pool<IAudioProxy>(() => { return new AudioProxy(); }, audio => { audio.Dispose(); });
        }
        public void ClearAllAudio()
        {
        }

        public void PauseAudio(AudioObject audioObject, float fadeTime)
        {
        }

        public void PlayAudio(AudioObject audioObject, AudioParams audioParams, AudioPositionParams audioPlayInfo)
        {
        }

        public void ResumeAudio(AudioObject audioObject, float fadeTime)
        {
        }

        public void SetAudioParam(AudioObject audioObject, AudioParams audioParams)
        {
        }

        public void StopAudio(AudioObject audioObject, float fadeTime)
        {
        }

        public void TickRefresh()
        {
        }
    }
}
