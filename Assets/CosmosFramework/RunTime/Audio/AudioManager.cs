using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Audio
{
    public sealed class AudioManager : Module<AudioManager>
    {
        //背景音乐，新BGM覆盖旧的
        AudioSource backgroundAduio;
        //单一不重复音效，全局只播放一个，新的单通道会覆盖旧的单通道音效，例如NPC
        AudioSource singleAudio;
        //多通道音效，多用于技能、UI
        List<AudioSource> mutipleAudio = new List<AudioSource>();
        //世界音效，为3D背景音乐、3D技能音效对白等设计
        Dictionary<GameObject, AudioSource> worldAudios = new Dictionary<GameObject, AudioSource>();
        protected override void InitModule()
        {
            RegisterModule(CFModule.Audio);
            //MonoManager.Instance.AddListener(CheckAudioSources, Mono.UpdateType.Update);
            Facade.Instance.AddMonoListener(CheckAudioSources, Mono.UpdateType.Update);
        }
        bool mute=false;
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
                    for (int i = 0; i < mutipleAudio.Count; i++)
                    {
                        mutipleAudio[i].mute = mute;
                    }
                    foreach (var audio in worldAudios)
                    {
                        audio.Value.mute = mute;
                    }
                }
            }
        }
        #region backGroundAudio
        /// <summary>
        /// 播放背景音乐，唯一的
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="arg"></param>
        public void PlayBackgroundAudio(AudioEventArgs arg)
        {
            if (backgroundAduio == null)
            {
                backgroundAduio = CreateAudioSource(arg);
                backgroundAduio.clip = arg.AudioEventObject.AudioClip;
                backgroundAduio.Play();
            }
            else
            {
                if (backgroundAduio.isPlaying)
                {
                    backgroundAduio.Stop();
                }
                backgroundAduio.clip = arg.AudioEventObject. AudioClip;
                SetAudioProperties(ref backgroundAduio, arg);
                backgroundAduio.Play();
            }
            Utility.DebugLog("audioPlaying");
        }
        public void PauseBackgroundAudio()
        {
            backgroundAduio.Pause();
        }
        public void UnPauseBackgroundAudio()
        {
            backgroundAduio.UnPause();
        }
        public void StopBackgroundAudio()
        {
            backgroundAduio.Stop();
        }
        #endregion
        #region worldAudio
        /// <summary>
        /// 播放世界音效
        /// 可用在3D环境声音以及特效爆炸等上
        /// </summary>
        /// <param name="attachTarget">audioSource挂载的对象</param>
        /// <param name="clip">音频</param>
        /// <param name="arg">具体参数</param>
        public void PlayWorldAudio(GameObject attachTarget, AudioEventArgs arg)
        {
            if (worldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = worldAudios[attachTarget];
                if (audio.isPlaying)
                    audio.Stop();
                SetAudioProperties(ref audio, arg);
                audio.clip = arg.AudioEventObject.AudioClip;
                audio.Play();
            }
            else
            {
                AudioSource audio = AttachAudioSource(attachTarget, arg);
                worldAudios.Add(attachTarget, audio);
                SetAudioProperties(ref audio, arg);
                audio.clip = arg.AudioEventObject .AudioClip;
                audio.Play();
            }
        }
        public void PauseWorldAudio(GameObject attachTarget)
        {
            if (worldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = worldAudios[attachTarget];
                audio.Pause();
            }
            else
                Utility.DebugError(attachTarget.name + "not register in audio manager", attachTarget);
        }
        public void UnPauseWorldAudio(GameObject attachTarget)
        {
            if (worldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = worldAudios[attachTarget];
                audio.UnPause();
            }else
                Utility.DebugError(attachTarget.name + "not register in audio manager", attachTarget);
        }
        public void StopWorldAudio(GameObject attachTarget)
        {
            if (worldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = worldAudios[attachTarget];
                audio.UnPause();
            }else
                Utility.DebugError(attachTarget.name + "not register in audio manager", attachTarget);
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
        AudioSource CreateAudioSource(AudioEventArgs arg)
        {
            GameObject go = new GameObject(arg.AudioEventObject .AudioName);
            go.transform.SetParent(ModuleMountObject.transform);
            go.transform.RestLocalTransform();
            //AudioSource audio = go.AddComponent<AudioSource>();
            AudioSource audio = Utility.Add<AudioSource>(go);
            SetAudioProperties(ref audio, arg);
            return audio;
        }
        AudioSource AttachAudioSource(GameObject target, AudioEventArgs arg)
        {
            AudioSource audio = target.AddComponent<AudioSource>();
           SetAudioProperties(ref audio, arg);
            return audio;
        }
        void SetAudioProperties(ref AudioSource audio, AudioEventArgs arg)
        {
            AudioSource aduioSou = audio;
            audio.playOnAwake = arg. AudioEventObject .PlayOnAwake;
            audio.volume = arg.AudioEventObject .Volume;
            audio.pitch = arg.AudioEventObject .Speed;
            audio.spatialBlend = arg.AudioEventObject .SpatialBlend;
            audio.mute = arg.AudioEventObject .Mute;
            audio.loop = arg.AudioEventObject .Loop;
        }
        //轮询间距，按照update渲染的5秒计算，不计算实际时间
        public const short _Interval = 5;
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
    }
}