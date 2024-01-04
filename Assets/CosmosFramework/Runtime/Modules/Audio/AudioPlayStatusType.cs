namespace Cosmos.Audio
{
    public enum AudioPlayStatusType : byte
    {
        None=1,
        Play=1<<1,
        Stop=1<<2,
        Pause=1 << 3
    }
}
