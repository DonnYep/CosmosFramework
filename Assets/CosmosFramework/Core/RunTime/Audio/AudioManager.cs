using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Audio
{
    internal sealed class AudioManager : Module<AudioManager>
    {
        #region Properties
        //背景音乐，新BGM覆盖旧的
        AudioSource backgroundAduio;
        //单一不重复音效，全局只播放一个，新的单通道会覆盖旧的单通道音效，例如NPC
        AudioSource singleAudio;
        //多通道音效，多用于技能、UI
        Dictionary<GameObject, List<AudioSource>> multipleAudio = new Dictionary<GameObject, List<AudioSource>>();
        //放着先，到时候再说-->>
        List<AudioSource> multipleAudios = new List<AudioSource>();

        //世界音效，为3D背景音乐、3D技能音效对白等设计
        Dictionary<GameObject, AudioSource> worldAudios = new Dictionary<GameObject, AudioSource>();
        #endregion

        #region Methods
        public override void OnInitialization()
        {
            base.OnInitialization();
        }
      public override void OnRefresh()
        {
            CheckAudioSources();
        }
        bool mute=false;
        //整个AudioManager下的所有声音都设置位静音
        internal bool Mute
        {
            get { return mute; }
            set
            {
                if (mute != value)
                {
                    mute = value;
                    backgroundAduio.mute = mute;
                    singleAudio.mute = mute;
                    for (int i = 0; i < multipleAudios.Count; i++)
                    {
                        multipleAudios[i].mute = mute;
                    }
                    foreach (var audio in worldAudios)
                    {
                        audio.Value.mute = mute;
                    }
                    foreach (var audio in multipleAudio)
                    {
                        for (int i = 0; i < audio.Value.Count; i++)
                        {
                            audio.Value[i].mute = mute;
                        }
                    }
                }
            }
        }
        #region backGroundAudio
        /// <summary>
        /// 播放背景音乐，唯一的
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="args"></param>
        internal void PlayBackgroundAudio(GameEventArgs args)
        {
            var tempArgs = args as LogicEventArgs<IAudio>;
            if (backgroundAduio == null)
            {
                backgroundAduio = CreateBGMAudioSource(tempArgs.Data);
                backgroundAduio.clip = tempArgs.Data.AudioClip;
                backgroundAduio.Play();
            }
            else
            {
                if (backgroundAduio.isPlaying)
                {
                    backgroundAduio.Stop();
                }
                backgroundAduio.clip = tempArgs.Data. AudioClip;
                SetAudioProperties(ref backgroundAduio, tempArgs.Data);
                backgroundAduio.Play();
            }
        }
        internal void PauseBackgroundAudio()
        {
            if (backgroundAduio != null)
                backgroundAduio.Pause();
            else
                Utility.DebugError("BackgroundAudio  not exist!");
        }
        internal void UnpauseBackgroundAudio()
        {
            if (backgroundAduio != null)
                backgroundAduio.UnPause();
            else
                Utility.DebugError("BackgroundAudio  not exist!");
        }
        internal void StopBackgroundAudio()
        {
            if (backgroundAduio != null)
                backgroundAduio.Stop();
            else
                Utility.DebugError("BackgroundAudio  not exist!");
        }
        #endregion
        #region worldAudio
        /// <summary>
        /// 播放世界音效
        /// 可用在3D环境声音以及特效爆炸等上
        /// </summary>
        /// <param name="attachTarget">audioSource挂载的对象</param>
        /// <param name="clip">音频</param>
        /// <param name="args">具体参数</param>
        internal void PlayWorldAudio(GameObject attachTarget, GameEventArgs argss)
        {
            var tempArgs = argss as LogicEventArgs<IAudio>;
            if (worldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = worldAudios[attachTarget];
                if (audio.isPlaying)
                    audio.Stop();
                SetAudioProperties(ref audio, tempArgs.Data);
                audio.clip =tempArgs.Data.AudioClip;
                audio.Play();
            }
            else
            {
                AudioSource audio = AttachAudioSource(attachTarget, tempArgs.Data);
                worldAudios.Add(attachTarget, audio);
                audio.clip = tempArgs.Data.AudioClip;
                audio.Play();
            }
        }
        internal void PauseWorldAudio(GameObject attachTargset)
        {
            if (worldAudios.ContainsKey(attachTargset))
            {
                AudioSource audio = worldAudios[attachTargset];
                audio.Pause();
            }
            else
            throw new CFrameworkException("AudioManager\n" + "World" + attachTargset.name + "\n is unregistered");
        }
        internal void UnpauseWorldAudio(GameObject attachTargset)
        {
            if (worldAudios.ContainsKey(attachTargset))
            {
                AudioSource audio = worldAudios[attachTargset];
                audio.UnPause();
            }else
            throw new CFrameworkException("AudioManager\n" + "World" + attachTargset.name + "\n is unregistered");
        }
        internal void StopWorldAudio(GameObject attachTargset)
        {
            if (worldAudios.ContainsKey(attachTargset))
            {
                AudioSource audio = worldAudios[attachTargset];
                audio.Stop();
            }else
            throw new CFrameworkException("AudioManager\n" + "World" + attachTargset.name + "\n is unregistered");
        }
        internal void StopAllWorldAudio()
        {
            foreach (var audio in worldAudios)
            {
                if (audio.Value.isPlaying)
                {
                    audio.Value.Stop();
                }
            }
        }
        #endregion
        #region MultipleAudio
        internal void PlayMultipleAudio(GameObject attachTargset,GameEventArgs[] argss) 
        {
            if (multipleAudio.ContainsKey(attachTargset))
            {
                var audios = multipleAudio[attachTargset];
                short audioCount =(short)audios.Count;
                short argssCount = (short)argss.Length;
                short differenceValue = (short)(argssCount - audioCount);
                //补齐差值
                for (short i = 0; i < differenceValue; i++)
                {
                    AudioSource audio = AttachAudioSource(attachTargset);
                    multipleAudio[attachTargset].Add(audio);
                }
                for (short i = 0; i < argssCount; i++)
                {
                    var audio= multipleAudio[attachTargset][i];
                    if (audio.isPlaying)
                        audio.Stop();
                    var tempArgs = argss[i] as LogicEventArgs<IAudio>;
                    SetAudioProperties(ref audio, tempArgs.Data);
                    audio.clip =tempArgs.Data.AudioClip;
                    audio.Play();
                }
            }
            else
            {
                multipleAudio.Add(attachTargset, new List<AudioSource>());
                for (int i = 0; i < argss.Length; i++)
                {
                    var tempArgs = argss[i] as LogicEventArgs<IAudio>;
                    AudioSource audio = AttachAudioSource(attachTargset, tempArgs.Data);
                    audio.clip = tempArgs.Data.AudioClip;
                    audio.Play();
                    multipleAudio[attachTargset].Add(audio);
                }
            }
        }
        internal void PauseMultipleAudio(GameObject attachTargset)
        {
            if (multipleAudio.ContainsKey(attachTargset))
            {
                for (short i = 0; i < multipleAudio[attachTargset].Count; i++)
                {
                    multipleAudio[attachTargset][i].Pause();
                }
            }else
            throw new CFrameworkException("AudioManager\n" + "Multiple" + attachTargset.name + "\n is unregistered");
        }
        internal void UnpauseMultipleAudio(GameObject attachTargset)
        {
            if (multipleAudio.ContainsKey(attachTargset))
            {
                for (short i = 0; i < multipleAudio[attachTargset].Count; i++)
                {
                    multipleAudio[attachTargset][i].UnPause();
                }
            }
            else
            throw new CFrameworkException("AudioManager\n" + "Multiple" + attachTargset.name + "\n is unregistered");

        }
        internal void StopMultipleAudio(GameObject attachTargset)
        {
            if (multipleAudio.ContainsKey(attachTargset))
            {
                for (short i = 0; i < multipleAudio[attachTargset].Count; i++)
                {
                    multipleAudio[attachTargset][i].Stop();
                }
            }
            else
            throw new CFrameworkException("AudioManager\n" + "Multiple" + attachTargset.name + "\n is unregistered");

        }
        #endregion
        AudioSource CreateBGMAudioSource( IAudio args)
        {
            GameObject go = new GameObject(args.AudioClip.name);
            go.transform.SetParent(ModuleMountObject.transform);
            go.transform.ResetLocalTransform();
            AudioSource audio = go.AddComponent<AudioSource>();
            SetAudioProperties(ref audio, args);
            return audio;
        }
        AudioSource AttachAudioSource(GameObject targset, IAudio args)
        {
            AudioSource audio = targset.AddComponent<AudioSource>();
            SetAudioProperties(ref audio, args);
            return audio;
        }
        AudioSource AttachAudioSource(GameObject targset)
        {
            AudioSource audio = targset.AddComponent<AudioSource>();
            return audio;
        }
        void SetAudioProperties(ref AudioSource audio, IAudio args)
        {
            audio.playOnAwake = args.PlayOnAwake;
            audio.volume = args.Volume;
            audio.pitch = args.Speed;
            audio.spatialBlend = args.SpatialBlend;
            audio.mute = args.Mute;
            audio.loop = args.Loop;
        }
        //轮询间距，按照update渲染的5秒计算，不使用真实时间
        internal const short _Interval = 5;
        float coolTime = 0;
        /// <summary>
        /// 声音状态轮询，间隔一段时间后，销毁已经播放结束的声音组件
        /// </summary>
        void CheckAudioSources()
        {
            if (coolTime < _Interval)
            {
                coolTime += Time.deltaTime;
            }
            if (coolTime >= _Interval)
            {
                coolTime = 0;
                ClearIdleWorldAudio();
                ClearIdleMultipleAudio();
            }
        }
        void ClearIdleWorldAudio()
        {
            HashSet<GameObject> removeSet = new HashSet<GameObject>();
            foreach (var audio in worldAudios)
            {
                if (!audio.Value.isPlaying)
                {
                    removeSet.Add(audio.Key);
                    GameManager.KillObject(audio.Value);
                }
            }
            foreach (var item in removeSet)
            {
                worldAudios.Remove(item);
            }
        }
        void ClearIdleMultipleAudio()
        {
            HashSet<GameObject> removeSet = new HashSet<GameObject>();
            foreach (var audioList in multipleAudio)
            {
                HashSet<AudioSource> clips = new HashSet<AudioSource>();
                if (audioList.Value.Count == 0)
                {
                    removeSet.Add(audioList.Key);
                    return;
                }
                for (int i = 0; i < audioList.Value.Count; i++)
                {
                    if (!audioList.Value[i].isPlaying)
                    {
                        GameManager.KillObject(audioList.Value[i]);
                        clips.Add(audioList.Value[i]);
                    }
                }
                foreach (var item in clips)
                {
                    audioList.Value.Remove(item);
                }
                clips.Clear();
            }
            foreach (var item in removeSet)
            {
                multipleAudio.Remove(item);
            }
            removeSet.Clear();
        }
        #endregion
    }
}