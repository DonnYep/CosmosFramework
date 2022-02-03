namespace Cosmos.Audio
{
    public interface IAudioPlayHelper
    {
        bool Mute { get; set; }
        void PlayAudio(AudioObject audioObject,AudioParams audioParams, AudioPlayInfo audioPlayInfo);
        void StopAudio(AudioObject audioObject);
        void PauseAudio(AudioObject audioObject);
        void UnPauseAudio(AudioObject audioObject);
        void ClearAllAudio();
        void TickRefresh();
    }
}
