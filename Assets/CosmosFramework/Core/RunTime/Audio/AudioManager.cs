using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Cosmos.Audio
{
    [Module]
    internal sealed class AudioManager : Module, IAudioManager
    {
        #region Properties
        //背景音乐，新BGM覆盖旧的
        AudioSource backgroundAduio;
        //单一不重复音效，全局只播放一个，新的单通道会覆盖旧的单通道音效，例如NPC
        AudioSource singleAudio;
        //多通道音效，多用于技能、UI
        Dictionary<GameObject, List<AudioSource>> multipleAudio;
        //放着先，到时候再说-->>
        List<AudioSource> multipleAudios;
        //世界音效，为3D背景音乐、3D技能音效对白等设计
        Dictionary<GameObject, AudioSource> worldAudios;
        #endregion
        //轮询间距，按照update渲染的5秒计算，不使用真实时间
        internal const short _Interval = 5;
        float coolTime = 0;
        #region Methods
        public override void OnInitialization()
        {
            multipleAudio = new Dictionary<GameObject, List<AudioSource>>();
            multipleAudios = new List<AudioSource>();
            worldAudios = new Dictionary<GameObject, AudioSource>();
        }
        public override void OnRefresh()
        {
            CheckAudioSources();
        }
        bool mute = false;
        //整个AudioManager下的所有声音都设置位静音
        public bool Mute
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
        public void PlayBackgroundAudio(IAudio  audio)
        {
            if (backgroundAduio == null)
            {
                backgroundAduio = CreateBGMAudioSource(audio);
                backgroundAduio.clip = audio.AudioClip;
                backgroundAduio.Play();
            }
            else
            {
                if (backgroundAduio.isPlaying)
                {
                    backgroundAduio.Stop();
                }
                backgroundAduio.clip = audio.AudioClip;
                SetAudioProperties(ref backgroundAduio, audio);
                backgroundAduio.Play();
            }
        }
        public void PauseBackgroundAudio()
        {
            if (backgroundAduio != null)
                backgroundAduio.Pause();
            else
                Utility.Debug.LogError("BackgroundAudio  not exist!");
        }
        public void UnpauseBackgroundAudio()
        {
            if (backgroundAduio != null)
                backgroundAduio.UnPause();
            else
                Utility.Debug.LogError("BackgroundAudio  not exist!");
        }
        public void StopBackgroundAudio()
        {
            if (backgroundAduio != null)
                backgroundAduio.Stop();
            else
                Utility.Debug.LogError("BackgroundAudio  not exist!");
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
        public void PlayWorldAudio(GameObject attachTarget, IAudio audio)
        {
            if (worldAudios.ContainsKey(attachTarget))
            {
                AudioSource audioSrc = worldAudios[attachTarget];
                if (audioSrc.isPlaying)
                    audioSrc.Stop();
                SetAudioProperties(ref audioSrc, audio);
                audioSrc.clip = audio.AudioClip;
                audioSrc.Play();
            }
            else
            {
                AudioSource audioSrc = AttachAudioSource(attachTarget, audio);
                worldAudios.Add(attachTarget, audioSrc);
                audioSrc.clip = audio.AudioClip;
                audioSrc.Play();
            }
        }
        public void PauseWorldAudio(GameObject attachTargset)
        {
            if (worldAudios.ContainsKey(attachTargset))
            {
                AudioSource audio = worldAudios[attachTargset];
                audio.Pause();
            }
            else
                throw new ArgumentNullException("AudioManager\n" + "World" + attachTargset.name + "\n is unregistered");
        }
        public void UnpauseWorldAudio(GameObject attachTargset)
        {
            if (worldAudios.ContainsKey(attachTargset))
            {
                AudioSource audio = worldAudios[attachTargset];
                audio.UnPause();
            }
            else
                throw new ArgumentNullException("AudioManager\n" + "World" + attachTargset.name + "\n is unregistered");
        }
        public void StopWorldAudio(GameObject attachTargset)
        {
            if (worldAudios.ContainsKey(attachTargset))
            {
                AudioSource audio = worldAudios[attachTargset];
                audio.Stop();
            }
            else
                throw new ArgumentNullException("AudioManager\n" + "World" + attachTargset.name + "\n is unregistered");
        }
        public void StopAllWorldAudio()
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
        public void PlayMultipleAudio(GameObject attachTargset, IAudio[]  audios)
        {
            if (multipleAudio.ContainsKey(attachTargset))
            {
                var audioSrcs = multipleAudio[attachTargset];
                short audioSrcCount = (short)audioSrcs.Count;
                short audiosCount = (short)audios.Length;
                short differenceValue = (short)(audiosCount - audioSrcCount);
                //补齐差值
                for (short i = 0; i < differenceValue; i++)
                {
                    AudioSource audio = AttachAudioSource(attachTargset);
                    multipleAudio[attachTargset].Add(audio);
                }
                for (short i = 0; i < audiosCount; i++)
                {
                    var audio = multipleAudio[attachTargset][i];
                    if (audio.isPlaying)
                        audio.Stop();
                    SetAudioProperties(ref audio, audios[i]);
                    audio.clip = audios[i].AudioClip;
                    audio.Play();
                }
            }
            else
            {
                multipleAudio.Add(attachTargset, new List<AudioSource>());
                var length = audios.Length;
                for (int i = 0; i < length; i++)
                {
                    AudioSource audio = AttachAudioSource(attachTargset, audios[i]);
                    audio.clip = audios[i].AudioClip;
                    audio.Play();
                    multipleAudio[attachTargset].Add(audio);
                }
            }
        }
        public void PauseMultipleAudio(GameObject attachTargset)
        {
            if (multipleAudio.ContainsKey(attachTargset))
            {
                for (short i = 0; i < multipleAudio[attachTargset].Count; i++)
                {
                    multipleAudio[attachTargset][i].Pause();
                }
            }
            else
                throw new ArgumentNullException("AudioManager\n" + "Multiple" + attachTargset.name + "\n is unregistered");
        }
        public void UnpauseMultipleAudio(GameObject attachTargset)
        {
            if (multipleAudio.ContainsKey(attachTargset))
            {
                for (short i = 0; i < multipleAudio[attachTargset].Count; i++)
                {
                    multipleAudio[attachTargset][i].UnPause();
                }
            }
            else
                throw new ArgumentNullException("AudioManager\n" + "Multiple" + attachTargset.name + "\n is unregistered");

        }
        public void StopMultipleAudio(GameObject attachTargset)
        {
            if (multipleAudio.ContainsKey(attachTargset))
            {
                for (short i = 0; i < multipleAudio[attachTargset].Count; i++)
                {
                    multipleAudio[attachTargset][i].Stop();
                }
            }
            else
                throw new ArgumentNullException("AudioManager\n" + "Multiple" + attachTargset.name + "\n is unregistered");

        }
        #endregion
        AudioSource CreateBGMAudioSource(IAudio args)
        {
            GameObject go = new GameObject(args.AudioClip.name);
            var mountGo = GameManager.GetModuleMount<IAudioManager>();
            go.transform.SetParent(mountGo.transform);
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
                    GameObject.Destroy(audio.Value);
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
                        GameObject.Destroy(audioList.Value[i]);
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