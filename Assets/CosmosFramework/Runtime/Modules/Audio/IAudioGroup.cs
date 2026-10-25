namespace Cosmos.Audio
{
    /// <summary>
    /// 声音组；
    /// 声音组是播放的最小混音单元：组内音量、静音、并发上限统一生效，并支持按组批处理播放状态。
    /// </summary>
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
        /// <summary>
        /// 组音量，0~1；与单个播放实体的参数音量相乘
        /// <para>过渡中的音效也会平滑跟随，不会跳变</para>
        /// </summary>
        float Volume { get; set; }
        /// <summary>
        /// 组内最大同时播放数，0表示不限制
        /// <para>达到上限时：新音效优先级高于组内最低优先级（priority数值更小）的音效，
        /// 则抢占该音效；否则本次播放被放弃（<see cref="IAudioManager.PlayAudio(string, string, AudioParams, AudioPositionParams)"/> 返回0）</para>
        /// </summary>
        int MaxConcurrentAudios { get; set; }
        /// <summary>
        /// 暂停组内所有声音
        /// </summary>
        /// <param name="fadeOutSeconds">淡出时间，单位秒</param>
        void PauseAudios(float fadeOutSeconds);
        /// <summary>
        /// 恢复组内所有声音
        /// </summary>
        /// <param name="fadeInSeconds">淡入时间，单位秒</param>
        void ResumeAudios(float fadeInSeconds);
        /// <summary>
        /// 停止组内所有声音
        /// </summary>
        /// <param name="fadeOutSeconds">淡出时间，单位秒</param>
        void StopAudios(float fadeOutSeconds);
    }
}
