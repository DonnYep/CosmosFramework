using System.Collections;
using UnityEngine;

namespace Cosmos.Audio
{
    /// <summary>
    /// 声音代理对象；
    /// </summary>
    public class AudioProxy : IAudioProxy
    {
        public bool IsFading { get; set; }
        public AudioSource AudioSource { get; set; }
        Coroutine currentCoroutine;
        AudioParams audioParams;
        public void OnPlay(float fadeTime, AudioParams audioParams)
        {
            this.audioParams = audioParams;
            AudioSource.loop = audioParams.Loop;
            AudioSource.priority = audioParams.Priority;
            AudioSource.pitch = audioParams.Pitch;
            AudioSource.panStereo = audioParams.StereoPan;
            AudioSource.spatialBlend = audioParams.SpatialBlend;
            AudioSource.reverbZoneMix = audioParams.ReverbZoneMix;
            AudioSource.dopplerLevel = audioParams.DopplerLevel;
            AudioSource.spread = audioParams.Spread;
            AudioSource.maxDistance = audioParams.MaxDistance;
            if (currentCoroutine != null)
                Utility.Unity.StopCoroutine(currentCoroutine);
            currentCoroutine = Utility.Unity.StartCoroutine(EnumFadeInPlay(fadeTime));
        }
        public void OnUnPause(float fadeTime)
        {
            if (currentCoroutine != null)
                Utility.Unity.StopCoroutine(currentCoroutine);
            currentCoroutine = Utility.Unity.StartCoroutine(EnumFadeInUnPause(fadeTime));
        }
        public void OnPause(float fadeTime)
        {
            if (currentCoroutine != null)
                Utility.Unity.StopCoroutine(currentCoroutine);
            currentCoroutine = Utility.Unity.StartCoroutine(EnumFadeOutPause(fadeTime));
        }
        public void OnStop(float fadeTime)
        {
            if (currentCoroutine != null)
                Utility.Unity.StopCoroutine(currentCoroutine);
            currentCoroutine = Utility.Unity.StartCoroutine(EnumFadeOutStop(fadeTime));
        }
        public void Dispose()
        {
            if (currentCoroutine != null)
                Utility.Unity.StopCoroutine(currentCoroutine);
            IsFading = false;
            currentCoroutine = null;
            AudioSource = null;
        }
        IEnumerator EnumFadeInPlay(float fadeTime)
        {
            IsFading = true;
            if (fadeTime <= 0)
            {
                AudioSource.Play();
                IsFading = false;
                yield break;
            }
            AudioSource.volume = 0;
            AudioSource.Play();
            while (AudioSource.volume < audioParams.Volume)
            {
                AudioSource.volume += Time.deltaTime / fadeTime;
                yield return null;
            }
            AudioSource.volume = audioParams.Volume;
            IsFading = false;
        }
        IEnumerator EnumFadeInUnPause(float fadeTime)
        {
            IsFading = true;
            if (fadeTime <= 0)
            {
                AudioSource.UnPause();
                IsFading = false;
                yield break;
            }
            AudioSource.volume = 0;
            AudioSource.UnPause();
            while (AudioSource.volume < audioParams.Volume)
            {
                AudioSource.volume += Time.deltaTime / fadeTime;
                yield return null;
            }
            AudioSource.volume = audioParams.Volume;
            IsFading = false;
        }
        IEnumerator EnumFadeOutPause(float fadeTime)
        {
            IsFading = true;
            if (fadeTime <= 0)
            {
                AudioSource.Pause();
                IsFading = false;
                yield break;
            }
            float startVolume = AudioSource.volume;
            while (AudioSource.volume > 0)
            {
                AudioSource.volume -= Time.deltaTime / fadeTime;
                yield return null;
            }
            AudioSource.Pause();
            AudioSource.volume = startVolume;
            IsFading = false;
        }
        IEnumerator EnumFadeOutStop(float fadeTime)
        {
            IsFading = true;
            if (fadeTime <= 0)
            {
                AudioSource.Stop();
                IsFading = false;
                yield break;
            }
            float startVolume = AudioSource.volume;
            while (AudioSource.volume > 0)
            {
                AudioSource.volume -= Time.deltaTime / fadeTime;
                yield return null;
            }
            AudioSource.Stop();
            AudioSource.volume = startVolume;
            IsFading = false;
        }
    }
}
