namespace Cosmos.Audio
{
    public interface IAudioGroup
    {
        /// <summary>
        /// 是否静音
        /// </summary>
        bool Mute { get; set; }
        /// <summary>
        /// 音效组名
        /// </summary>
        string AudioGroupName { get; }
        /// <summary>
        /// 播放器数量
        /// </summary>
        int AudioPlayEntityCount { get; }
    }
}
