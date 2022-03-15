namespace Cosmos.Audio
{
    /// <summary>
    /// 声音播放帮助体；
    /// </summary>
    public interface IAudioPlayHelper
    {
        bool Mute { get; set; }
        void PlayAudio(AudioObject audioObject,AudioParams audioParams, AudioPlayInfo audioPlayInfo);
        void StopAudio(AudioObject audioObject);
        void PauseAudio(AudioObject audioObject);
        void UnPauseAudio(AudioObject audioObject);
        void SetAudioParam(AudioObject audioObject, AudioParams audioParams);
        void ClearAllAudio();
        void TickRefresh();
    }
}
