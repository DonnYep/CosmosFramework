using System;
namespace Cosmos.Audio
{
    //================================================
    /*
     * 1、音效采用注册播放方式。若需要成功播放一个音效，须先注册音效资源。
     * 
     * 2、默认存在一个内置的音效组，不可移除。
     * 
     * 3、音效的播放、暂停、恢复、停止操作，都可传入过渡时间。
     * 
     * 4、相同音效支持多个实体播放。
     * 
     * 5、所有播放声音所返回的serialId都大于等于1。
     * 
     * 6、状态机语义（由管理器每帧推进，不使用协程）：
     *    · 所有命令同步生效，不存在"要等下一帧协程才响应"；
     *    · 过渡可被后到的命令打断，总是从当前音量继续，不会出现两套过渡互相覆盖；
     *    · 恢复播放只在暂停/暂停过渡中生效，正在播放时是幂等空操作；
     *    · 播放结束（自然播完、停止完成）在同一帧内被回收，随后 HasAudio 返回 false。
     */
    //================================================
    public interface IAudioManager : IModuleManager
    {
        /// <summary>
        /// 声音注册失败事件，参数为失败的资源名称
        /// </summary>
        event Action<AudioRegisterFailureEventArgs> AudioAssetRegisterFailure;
        /// <summary>
        /// 声音注册成功事件
        /// </summary>
        event Action<AudioRegisterSuccessEventArgs> AudioAssetRegisterSuccess;
        /// <summary>
        /// 注册的声音资产数量
        /// </summary>
        int AudioAssetCount { get; }
        /// <summary>
        /// 静音
        /// </summary>
        bool Mute { get; set; }
        /// <summary>
        /// 设置声音资源加载帮助体
        /// </summary>
        /// <param name="helper">声音资源加载帮助体</param>
        void SetAudioAssetHelper(IAudioAssetHelper helper);
        /// <summary>
        ///注册声音资源
        ///<para>注册成功回调：<see cref="AudioAssetRegisterSuccess"/></para>
        ///<para>注册失败回调：<see cref="AudioAssetRegisterFailure"/></para>
        /// </summary>
        /// <param name="audioAssetName">声音资源名称</param>
        void RegisterAudioAsset(string audioAssetName);
        /// <summary>
        /// 注销声音资源，同时释放所有使用此资源名称的播放实体
        /// </summary>
        /// <param name="audioAssetName">声音资源名称</param>
        void DeregisterAudioAsset(string audioAssetName);
        /// <summary>
        /// 播放声音
        /// <para>未设置声音组，默认归入<see cref="AudioConstant.DEFAULT_AUDIO_GROUP"/></para>
        /// </summary>
        /// <param name="audioAssetName">声音资源名称</param>
        /// <returns>播放序列号</returns>
        int PlayAudio(string audioAssetName);
        /// <summary>
        /// 播放声音
        /// <para>未设置声音组，默认归入<see cref="AudioConstant.DEFAULT_AUDIO_GROUP"/></para>
        /// </summary>
        /// <param name="audioAssetName">声音资源名称</param>
        /// <param name="audioParams">声音播放参数</param>
        /// <returns>播放序列号</returns>
        int PlayAudio(string audioAssetName, AudioParams audioParams);
        /// <summary>
        /// 播放声音
        /// <para>未设置声音组，默认归入<see cref="AudioConstant.DEFAULT_AUDIO_GROUP"/></para>
        /// </summary>
        /// <param name="audioAssetName">声音资源名称</param>
        /// <param name="audioParams">声音播放参数</param>
        /// <param name="audioPositionParams">声音播放位置信息</param>
        /// <returns>播放序列号</returns>
        int PlayAudio(string audioAssetName, AudioParams audioParams, AudioPositionParams audioPositionParams);
        /// <summary>
        /// 播放声音
        /// <para>播放若需要声音组，则须先添加声音组</para>
        /// <para>添加声音组方法：<see cref="AddAudioGroup"/></para>
        /// </summary>
        /// <param name="audioAssetName">声音资源名称</param>
        /// <param name="audioGroupName">声音组名称</param>
        /// <param name="audioParams">声音播放参数</param>
        /// <param name="audioPositionParams">声音播放位置信息</param>
        /// <returns>播放序列号；资源未注册、声音组不存在，或组内并发上限已满且本音效优先级不足时返回0</returns>
        int PlayAudio(string audioAssetName, string audioGroupName, AudioParams audioParams, AudioPositionParams audioPositionParams);
        /// <summary>
        /// 重播声音
        /// <para>停止→回到<see cref="AudioParams.PlayTime"/>→播放→淡入；可用于正在停止过渡中的实体（会直接打断停止过渡）</para>
        /// </summary>
        /// <param name="serialId">播放序列号</param>
        /// <param name="fadeInSeconds">淡入时间，单位秒</param>
        void ReplayAudio(int serialId, float fadeInSeconds);
        /// <summary>
        /// 暂停声音
        /// <para>已在暂停或暂停过渡中时幂等</para>
        /// </summary>
        /// <param name="serialId">播放序列号</param>
        /// <param name="fadeOutSeconds">淡出时间，单位秒</param>
        void PauseAudio(int serialId, float fadeOutSeconds);
        /// <summary>
        /// 暂停所有使用指定资源的声音实体
        /// </summary>
        /// <param name="audioAssetName">声音资源名称</param>
        /// <param name="fadeOutSeconds">淡出时间，单位秒</param>
        void PauseAudios(string audioAssetName, float fadeOutSeconds);
        /// <summary>
        /// 恢复播放声音
        /// <para>只在已暂停或暂停过渡中生效；正在播放时是幂等空操作（不会把正在播放的声音压到0再拉起）</para>
        /// </summary>
        /// <param name="serialId">播放序列号</param>
        /// <param name="fadeInSeconds">淡入时间，单位秒</param>
        void ResumeAudio(int serialId, float fadeInSeconds);
        /// <summary>
        /// 恢复播放所有使用指定资源的声音实体
        /// </summary>
        /// <param name="audioAssetName">声音资源名称</param>
        /// <param name="fadeInSeconds">淡入时间，单位秒</param>
        void ResumeAudios(string audioAssetName, float fadeInSeconds);
        /// <summary>
        /// 停止播放声音
        /// <para>已在停止过渡中时按新的时长重新计时</para>
        /// </summary>
        /// <param name="serialId">播放序列号</param>
        /// <param name="fadeOutSeconds">淡出时间，单位秒</param>
        void StopAudio(int serialId, float fadeOutSeconds);
        /// <summary>
        /// 停止播放所有使用指定资源的声音实体
        /// </summary>
        /// <param name="audioAssetName">声音资源名称</param>
        /// <param name="fadeOutSeconds">淡出时间，单位秒</param>
        void StopAudios(string audioAssetName, float fadeOutSeconds);
        /// <summary>
        /// 是否存在声音
        /// <para>停止完成的那一帧即被回收，之后返回false</para>
        /// </summary>
        /// <param name="serialId">播放序列号</param>
        /// <returns>是否存在</returns>
        bool HasAudio(int serialId);
        /// <summary>
        /// 获取声音播放信息
        /// <para>包含状态、当前实际音量与播放位置，可用于进度条与调试面板</para>
        /// </summary>
        /// <param name="serialId">播放序列号</param>
        /// <param name="audioPlayInfo">声音播放信息</param>
        /// <returns>是否获取到</returns>
        bool GetAudioPlayInfo(int serialId, out AudioPlayInfo audioPlayInfo);
        /// <summary>
        /// 声音资源是否已注册
        /// </summary>
        /// <param name="audioAssetName">声音资源名称</param>
        /// <returns>是否注册</returns>
        bool IsAudioAssetRegistered(string audioAssetName);
        /// <summary>
        /// 设置声音播放参数
        /// </summary>
        /// <param name="serialId">播放序列号</param>
        /// <param name="audioParams">声音播放参数</param>
        void SetAudioParams(int serialId, AudioParams audioParams);
        /// <summary>
        /// 设置所有使用指定资源的声音实体播放参数
        /// </summary>
        /// <param name="audioAssetName">声音资源名称</param>
        /// <param name="audioParams">声音播放参数</param>
        void SetAudiosParams(string audioAssetName, AudioParams audioParams);
        /// <summary>
        /// 是否存在声音组
        /// </summary>
        /// <param name="audioGroupName">声音组名称</param>
        /// <returns>是否存在</returns>
        bool HasAudioGroup(string audioGroupName);
        /// <summary>
        /// 移除声音组
        /// <para>框架存在一个默认的声音组：<see cref="AudioConstant.DEFAULT_AUDIO_GROUP"/>，此声音组无法被移除</para>
        /// <para>移除时组内仍在播放的声音会被立即释放并归还播放器</para>
        /// </summary>
        /// <param name="audioGroupName">声音组名称</param>
        void RemoveAudioGroup(string audioGroupName);
        /// <summary>
        /// 添加声音组
        /// <para>返回的声音组可设置组音量、静音与并发上限（<see cref="IAudioGroup"/>）</para>
        /// </summary>
        /// <param name="audioGroupName">声音组名称</param>
        /// <returns>声音组实体接口</returns>
        IAudioGroup AddAudioGroup(string audioGroupName);
        /// <summary>
        /// 获取声音组
        /// </summary>
        /// <param name="audioGroupName">声音组名称</param>
        /// <param name="group">声音组实体接口</param>
        /// <returns>获取结果</returns>
        bool PeekAudioGroup(string audioGroupName, out IAudioGroup group);
        /// <summary>
        /// 暂停播放所有声音
        /// </summary>
        /// <param name="fadeOutSecounds">淡出时间，单位秒</param>
        void PauseAllAudios(float fadeOutSecounds);
        /// <summary>
        /// 停止播放所有声音
        /// </summary>
        /// <param name="fadeOutSecounds">淡出时间，单位秒</param>
        void StopAllAudios(float fadeOutSecounds);
    }
}