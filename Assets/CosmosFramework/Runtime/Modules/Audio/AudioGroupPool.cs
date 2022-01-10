namespace Cosmos.Audio
{
    /// <summary>
    /// 音效组池；
    /// </summary>
    internal class AudioGroupPool
    {
        Pool<IAudioGroup> audioGroupPool;
        public AudioGroupPool()
        {
            audioGroupPool = new Pool<IAudioGroup>(() => { return new AudioGroup(); },ag=> { ag.Release(); });
        }
        public void Despawn(IAudioGroup audioGroup)
        {
            audioGroupPool.Despawn(audioGroup);
        }
        public IAudioGroup Spawn()
        {
            return audioGroupPool.Spawn();
        }
    }
}
