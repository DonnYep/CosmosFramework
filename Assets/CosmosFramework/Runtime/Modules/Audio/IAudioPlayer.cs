namespace Cosmos.Audio
{
    public interface IAudioPlayer
    {
        void PlayAudio();
        void StopAudio();
        void PauseAudio();
        void UnpauseAudio();
    }
}