using UnityEngine;

namespace Cosmos.Audio
{
    /// <summary>
    /// 音效播放实体
    /// <para>1、不再使用协程：播放、暂停、恢复、停止都是同步命令，调用当帧即生效，
    /// 不存在"要等协程被调度到下一帧才响应"的问题。</para>
    /// <para>2、淡入淡出只改一个 0~1 的"音量包络因子"，由 <see cref="AudioManager"/> 每帧
    /// <see cref="Tick"/> 推进；推进到终点的同一帧内完成状态迁移（<see cref="Phase"/>）。</para>
    /// <para>3、过渡可被后到的命令打断（last-command-wins）：新命令总是从"当前包络值"继续，
    /// 不会出现两条协程各自计时、互相覆盖的中间态。</para>
    /// <para>4、最终音量 = 参数音量 × 组音量 × 包络因子；因此过渡进行中改参数或改组音量都不会跳变。</para>
    /// <para>5、终止（自然播完 / 停止完成 / 播放器被销毁）只由状态机判定，
    /// 组内回收在同一帧完成，不依赖"下一次播放"来顺带清理。</para>
    /// </summary>
    internal class AudioPlayEntity : IReference
    {
        /// <summary>
        /// 播放阶段（显式状态机）
        /// </summary>
        internal enum Phase : byte
        {
            /// <summary>
            /// 空闲（未使用）
            /// </summary>
            None = 0,
            /// <summary>
            /// 播放中，可能正处在淡入过程中
            /// </summary>
            Playing = 1,
            /// <summary>
            /// 淡出中，结束后进入 <see cref="Paused"/>
            /// </summary>
            Pausing = 2,
            /// <summary>
            /// 已暂停
            /// </summary>
            Paused = 3,
            /// <summary>
            /// 由暂停恢复的淡入中，结束后进入 <see cref="Playing"/>
            /// </summary>
            Resuming = 4,
            /// <summary>
            /// 淡出中，结束后进入 <see cref="Stopped"/>
            /// </summary>
            Stopping = 5,
            /// <summary>
            /// 已停止，等待回收
            /// </summary>
            Stopped = 6,
        }
        /// <summary>
        /// unity的声音播放器
        /// </summary>
        public AudioSource AudioSource { get { return audioSource; } }
        /// <summary>
        /// 音效资源名
        /// </summary>
        public string AudioAssetName { get; private set; }
        /// <summary>
        /// 音效序列号
        /// </summary>
        public int SerialId { get; set; }
        /// <summary>
        /// 静音
        /// </summary>
        public bool Mute
        {
            get { return audioSource != null && audioSource.mute; }
            set
            {
                if (audioSource != null)
                    audioSource.mute = value;
            }
        }
        /// <summary>
        /// 声音播放状态
        /// </summary>
        public AudioPlayStatusType AudioPlayStatusType
        {
            get
            {
                switch (phase)
                {
                    case Phase.Playing:
                    case Phase.Resuming:
                        return AudioPlayStatusType.Play;
                    case Phase.Pausing:
                    case Phase.Paused:
                        return AudioPlayStatusType.Pause;
                    case Phase.Stopping:
                    case Phase.Stopped:
                        return AudioPlayStatusType.Stop;
                    default:
                        return AudioPlayStatusType.None;
                }
            }
        }
        /// <summary>
        /// 当前实际音量（已含组音量与过渡）
        /// </summary>
        public float CurrentVolume { get { return appliedVolume; } }
        /// <summary>
        /// 当前播放位置，单位秒
        /// </summary>
        public float PlaybackTime
        {
            get
            {
                if (audioSource == null || audioSource.clip == null)
                    return 0f;
                return audioSource.time;
            }
        }
        /// <summary>
        /// 播放优先级（Unity 语义：数值越大优先级越低）
        /// </summary>
        internal int AudioPriority { get { return audioParams.Priority; } }
        /// <summary>
        /// 是否处于可被抢占的状态（已停止的不再参与抢占）
        /// </summary>
        internal bool IsInterruptible
        {
            get { return phase != Phase.None && phase != Phase.Stopped; }
        }
        /// <summary>
        /// 播放参数缓存
        /// </summary>
        AudioParams audioParams;
        /// <summary>
        /// 声音播放位置参数
        /// </summary>
        AudioPositionParams audioPositionParams;
        /// <summary>
        /// unity的声音播放器
        /// </summary>
        AudioSource audioSource;
        /// <summary>
        /// 当前阶段
        /// </summary>
        Phase phase;
        /// <summary>
        /// 所属音效组音量，由音效组每帧同步
        /// </summary>
        float groupVolume;
        /// <summary>
        /// 音量包络因子，0~1
        /// </summary>
        float envelope;
        float fadeFrom;
        float fadeTo;
        /// <summary>
        /// 过渡时长，&lt;=0 表示没有进行中的过渡
        /// </summary>
        float fadeDuration;
        float fadeElapsed;
        /// <summary>
        /// 当前已经写入 AudioSource 的音量，用于避免无意义的属性写入
        /// </summary>
        float appliedVolume;
        /// <summary>
        /// 刚调用 Play/Replay 的这一帧不做"自然播放结束"判定，
        /// 避免 isPlaying 尚未生效时把音效误判为已结束。
        /// </summary>
        bool skipEndCheckOnce;
        /// <summary>
        /// 开始播放
        /// </summary>
        /// <param name="audioAssetEntity">音效资源实体</param>
        /// <param name="audioParams">播放参数</param>
        /// <param name="audioPositionParams">播放位置参数</param>
        /// <param name="mute">所属音效组是否静音</param>
        /// <param name="groupVolume">所属音效组音量</param>
        public void OnPlay(AudioAssetEntity audioAssetEntity, AudioParams audioParams, AudioPositionParams audioPositionParams, bool mute, float groupVolume)
        {
            this.audioPositionParams = audioPositionParams;
            this.audioParams = audioParams;
            this.groupVolume = Mathf.Clamp01(groupVolume);
            AudioAssetName = audioAssetEntity.AudioAssetName;

            audioSource = AudioSourcePool.Spawn();
            audioSource.clip = audioAssetEntity.AudioClip;
            ApplyAudioSourceParams();
            audioSource.mute = mute;
            audioSource.name = AudioConstant.PREFIX + AudioAssetName;
            BindTransform();

            if (audioSource.clip != null)
                audioSource.time = ClampPlayTime(audioParams.PlayTime);
            //先把音量压到起点再播放：避免首帧以池里残留的音量出声
            envelope = 0f;
            audioSource.volume = 0f;
            audioSource.Play();

            phase = Phase.Playing;
            skipEndCheckOnce = true;
            //淡入从 0 开始；fadeInSeconds<=0 时 StartFade 会立即把包络置 1，本帧即满音量
            StartFade(1f, audioParams.FadeInSeconds, Phase.Playing);
            ApplyVolume(true);
        }
        /// <summary>
        /// 从头重播（停止→定位→播放→淡入），可用于"停止过渡中"的实体：会直接打断停止过渡
        /// </summary>
        /// <param name="fadeInSeconds">淡入时间，单位秒</param>
        public void OnReplay(float fadeInSeconds)
        {
            if (audioSource == null)
            {
                //当AudioSource空引用时，可能作为某个物体的子物体被销毁了。
                //这里直接标记为停止状态，等待回收。
                phase = Phase.Stopped;
                return;
            }
            audioSource.Stop();
            BindTransform();
            if (audioSource.clip != null)
                audioSource.time = ClampPlayTime(audioParams.PlayTime);
            envelope = 0f;
            audioSource.volume = 0f;
            audioSource.Play();

            phase = Phase.Playing;
            skipEndCheckOnce = true;
            StartFade(1f, fadeInSeconds, Phase.Playing);
            ApplyVolume(true);
        }
        /// <summary>
        /// 暂停（淡出后暂停）；已在暂停/暂停中则忽略
        /// </summary>
        /// <param name="fadeOutSeconds">淡出时间，单位秒</param>
        public void OnPause(float fadeOutSeconds)
        {
            if (audioSource == null)
            {
                phase = Phase.Stopped;
                return;
            }
            if (phase != Phase.Playing && phase != Phase.Resuming)
                return;
            //音效已经播完（这一帧还没被 Tick 回收）：直接进入停止，
            //否则会暂停一个已结束的播放器，之后既不会再播也不会被回收
            if (!AudioListener.pause && !audioSource.isPlaying)
            {
                phase = Phase.Stopped;
                return;
            }
            StartFade(0f, fadeOutSeconds, Phase.Pausing);
            ApplyVolume(false);
        }
        /// <summary>
        /// 恢复播放（淡入）；只在暂停/暂停中生效，正在播放时是幂等空操作
        /// </summary>
        /// <param name="fadeInSeconds">淡入时间，单位秒</param>
        public void OnResume(float fadeInSeconds)
        {
            if (audioSource == null)
            {
                phase = Phase.Stopped;
                return;
            }
            if (phase != Phase.Paused && phase != Phase.Pausing)
                return;
            audioSource.UnPause();
            StartFade(1f, fadeInSeconds, Phase.Resuming);
            ApplyVolume(false);
        }
        /// <summary>
        /// 停止（淡出后停止）；已在停止中则按新的时长重新计时
        /// </summary>
        /// <param name="fadeOutSeconds">淡出时间，单位秒</param>
        public void OnStop(float fadeOutSeconds)
        {
            if (audioSource == null)
            {
                phase = Phase.Stopped;
                return;
            }
            if (phase == Phase.Stopped)
                return;
            StartFade(0f, fadeOutSeconds, Phase.Stopping);
            ApplyVolume(false);
        }
        /// <summary>
        /// 立即停止（不做过渡），用于并发上限抢占；停完后由音效组回收
        /// </summary>
        internal void StopImmediate()
        {
            if (audioSource != null)
                audioSource.Stop();
            envelope = 0f;
            fadeDuration = 0f;
            appliedVolume = 0f;
            phase = Phase.Stopped;
        }
        /// <summary>
        /// 设置播放参数；过渡中修改音量也不会跳变（过渡作用在包络因子上）
        /// </summary>
        /// <param name="audioParams">播放参数</param>
        public void SetAudioParams(AudioParams audioParams)
        {
            this.audioParams = audioParams;
            if (audioSource == null)
                return;
            ApplyAudioSourceParams();
            ApplyVolume(true);
        }
        /// <summary>
        /// 按当前组音量重新应用音量（组音量变化时由音效组调用）
        /// </summary>
        /// <param name="groupVolume">所属音效组音量</param>
        internal void ApplyGroupVolume(float groupVolume)
        {
            this.groupVolume = Mathf.Clamp01(groupVolume);
            ApplyVolume(false);
        }
        /// <summary>
        /// 每帧推进；由 <see cref="AudioManager"/> 调用
        /// </summary>
        /// <param name="deltaTime">推进时间，单位秒（不受 timeScale 影响）</param>
        /// <param name="groupVolume">所属音效组音量</param>
        /// <returns>是否已经终止、可以被回收</returns>
        internal bool Tick(float deltaTime, float groupVolume)
        {
            this.groupVolume = Mathf.Clamp01(groupVolume);
            if (phase == Phase.None || phase == Phase.Stopped)
                return true;
            if (audioSource == null)
            {
                //播放器随绑定对象一起被销毁（如绑定对象随场景卸载）：直接进入回收
                phase = Phase.Stopped;
                return true;
            }
            if (fadeDuration > 0f)
                AdvanceFade(deltaTime);
            if (phase == Phase.Playing && !audioParams.Loop)
            {
                if (skipEndCheckOnce)
                {
                    skipEndCheckOnce = false;
                }
                //AudioListener.pause 时所有 AudioSource.isPlaying 都会变成 false，
                //此时必须认为音效仍在播放，否则全局暂停会把正在播的音效全部回收掉
                else if (!AudioListener.pause && !audioSource.isPlaying)
                {
                    //非循环音效自然播放结束
                    phase = Phase.Stopped;
                    return true;
                }
            }
            ApplyVolume(false);
            return phase == Phase.Stopped;
        }
        /// <inheritdoc/>
        public void Release()
        {
            if (audioSource != null)
                AudioSourcePool.Despawn(audioSource);
            audioSource = null;
            AudioAssetName = string.Empty;
            SerialId = 0;
            phase = Phase.None;
            audioParams = AudioParams.Default;
            audioPositionParams = AudioPositionParams.Default;
            groupVolume = AudioConstant.DEFAULT_AUDIO_GROUP_VOLUME;
            envelope = 0f;
            fadeFrom = 0f;
            fadeTo = 0f;
            fadeDuration = 0f;
            fadeElapsed = 0f;
            appliedVolume = 0f;
            skipEndCheckOnce = false;
        }
        /// <summary>
        /// 绑定播放器位置：优先绑定父对象，否则挂到模块根节点并按世界坐标播放
        /// </summary>
        void BindTransform()
        {
            if (audioSource == null)
                return;
            var bindParent = audioPositionParams.BindParent;
            if (bindParent != null)
            {
                audioSource.transform.SetParent(bindParent);
                return;
            }
            if (!GameManager.TryGetModule<IAudioManager>(out var audioManager))
                return;
            var instanceObject = audioManager.InstanceObject();
            if (instanceObject == null)
                return;
            audioSource.transform.SetParent(instanceObject.transform);
            audioSource.transform.position = audioPositionParams.WorldPosition;
        }
        /// <summary>
        /// 把播放参数写入 AudioSource（音量交给包络处理，不在这里写）
        /// </summary>
        void ApplyAudioSourceParams()
        {
            if (audioSource == null)
                return;
            audioSource.loop = audioParams.Loop;
            audioSource.priority = audioParams.Priority;
            //pitch<=0 会让播放位置永不推进（永久"在播"却静音），这里给一个极小值兜底
            audioSource.pitch = Mathf.Max(AudioConstant.MIN_PITCH, audioParams.Pitch);
            audioSource.panStereo = audioParams.StereoPan;
            audioSource.spatialBlend = audioParams.SpatialBlend;
            audioSource.reverbZoneMix = audioParams.ReverbZoneMix;
            audioSource.dopplerLevel = audioParams.DopplerLevel;
            audioSource.spread = audioParams.Spread;
            audioSource.maxDistance = audioParams.MaxDistance;
        }
        /// <summary>
        /// 写入当前音量：参数音量 × 组音量 × 包络因子
        /// </summary>
        /// <param name="force">是否强制写入</param>
        void ApplyVolume(bool force)
        {
            if (audioSource == null)
                return;
            var target = Mathf.Clamp01(audioParams.Volume * groupVolume * envelope);
            if (!force && target == appliedVolume)
                return;
            appliedVolume = target;
            audioSource.volume = target;
        }
        /// <summary>
        /// 开始一次过渡；时长&lt;=0 时立即完成（保证同步响应）
        /// </summary>
        /// <param name="to">目标包络值</param>
        /// <param name="seconds">过渡时长</param>
        /// <param name="fadePhase">过渡期间所处的阶段</param>
        void StartFade(float to, float seconds, Phase fadePhase)
        {
            fadeFrom = envelope;
            fadeTo = to;
            fadeElapsed = 0f;
            fadeDuration = seconds > 0f ? seconds : 0f;
            phase = fadePhase;
            if (fadeDuration <= 0f)
            {
                envelope = fadeTo;
                CompleteFade();
            }
        }
        /// <summary>
        /// 推进过渡；到点时在同一帧内完成阶段迁移
        /// </summary>
        /// <param name="deltaTime">推进时间</param>
        void AdvanceFade(float deltaTime)
        {
            fadeElapsed += deltaTime;
            if (fadeElapsed >= fadeDuration)
            {
                envelope = fadeTo;
                fadeElapsed = fadeDuration;
                fadeDuration = 0f;
                CompleteFade();
                return;
            }
            envelope = Mathf.Lerp(fadeFrom, fadeTo, fadeElapsed / fadeDuration);
        }
        /// <summary>
        /// 过渡完成时的阶段迁移
        /// </summary>
        void CompleteFade()
        {
            switch (phase)
            {
                case Phase.Pausing:
                    if (audioSource != null)
                        audioSource.Pause();
                    phase = Phase.Paused;
                    break;
                case Phase.Resuming:
                    phase = Phase.Playing;
                    break;
                case Phase.Stopping:
                    if (audioSource != null)
                        audioSource.Stop();
                    phase = Phase.Stopped;
                    break;
            }
        }
        /// <summary>
        /// 限制播放起点在音频长度内
        /// </summary>
        /// <param name="playTime">播放起点</param>
        /// <returns>合法的播放起点</returns>
        float ClampPlayTime(float playTime)
        {
            var clip = audioSource != null ? audioSource.clip : null;
            if (clip == null)
                return 0f;
            return Mathf.Clamp(playTime, 0f, Mathf.Max(0f, clip.length - 0.01f));
        }
    }
}
