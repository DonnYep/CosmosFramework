namespace Cosmos.Audio
{
    /// <summary>
    /// 声音播放效果帮助体；
    /// 用以实现播放时渐入渐出，高音等效果；
    /// </summary>
    public interface IAudioEffectHelper
    {
        void OnStart();
        void OnPlaying();
        void OnEnd();
    }
}
