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
        Dictionary<GameObject, List<AudioSource>> multipleAudio = new Dictionary<GameObject, List<AudioSource>>();
        //放着先，到时候再说-->>
        List<AudioSource> multipleAudios = new List<AudioSource>();

        //世界音效，为3D背景音乐、3D技能音效对白等设计
        Dictionary<GameObject, AudioSource> worldAudios = new Dictionary<GameObject, AudioSource>();
        protected override void InitModule()
        {
            Facade.Instance.AddMonoListener(CheckAudioSources,UpdateType.Update);
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
        /// <param name="arg"></param>
        public void PlayBackgroundAudio(GameEventArgs arg)
        {
            var tempArgs = arg as LogicEventArgs<AudioVariable>;
            if (backgroundAduio == null)
            {
                backgroundAduio = CreateAudioSource(tempArgs.Data);
                backgroundAduio.clip = tempArgs.Data.AudioDataSet.AudioClip;
                backgroundAduio.Play();
            }
            else
            {
                if (backgroundAduio.isPlaying)
                {
                    backgroundAduio.Stop();
                }
                backgroundAduio.clip = tempArgs.Data.AudioDataSet. AudioClip;
                SetAudioProperties(ref backgroundAduio, tempArgs.Data);
                backgroundAduio.Play();
            }
        }
        public void PauseBackgroundAudio()
        {
            if (backgroundAduio != null)
                backgroundAduio.Pause();
            else
                Utility.DebugError("BackgroundAudio  not exist!");
        }
        public void UnpauseBackgroundAudio()
        {
            if (backgroundAduio != null)
                backgroundAduio.UnPause();
            else
                Utility.DebugError("BackgroundAudio  not exist!");
        }
        public void StopBackgroundAudio()
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
        /// <param name="arg">具体参数</param>
        public void PlayWorldAudio(GameObject attachTarget, GameEventArgs args)
        {
            var tempArgs = args as LogicEventArgs<AudioVariable>;
            if (worldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = worldAudios[attachTarget];
                if (audio.isPlaying)
                    audio.Stop();
                SetAudioProperties(ref audio, tempArgs.Data);
                audio.clip =tempArgs.Data.AudioDataSet.AudioClip;
                audio.Play();
            }
            else
            {
                AudioSource audio = AttachAudioSource(attachTarget, tempArgs.Data);
                worldAudios.Add(attachTarget, audio);
                audio.clip = tempArgs.Data.AudioDataSet.AudioClip;
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
                //Utility.DebugError("AudioManager\n" + "World"+attachTarget.name + "\n is unregistered", attachTarget);
            throw new CFrameworkException("AudioManager\n" + "World" + attachTarget.name + "\n is unregistered");
        }
        public void UnpauseWorldAudio(GameObject attachTarget)
        {
            if (worldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = worldAudios[attachTarget];
                audio.UnPause();
            }else
                //Utility.DebugError("AudioManager\n"+"World" +attachTarget.name + "\n is unregistered", attachTarget);
            throw new CFrameworkException("AudioManager\n" + "World" + attachTarget.name + "\n is unregistered");
        }
        public void StopWorldAudio(GameObject attachTarget)
        {
            if (worldAudios.ContainsKey(attachTarget))
            {
                AudioSource audio = worldAudios[attachTarget];
                audio.Stop();
            }else
                //Utility.DebugError("AudioManager\n"+"World" +attachTarget.name + "\n is unregistered", attachTarget);
            throw new CFrameworkException("AudioManager\n" + "World" + attachTarget.name + "\n is unregistered");
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
        public void PlayMultipleAudio(GameObject attachTarget,GameEventArgs[] args) 
        {
            if (multipleAudio.ContainsKey(attachTarget))
            {
                var audios = multipleAudio[attachTarget];
                short audioCount =(short)audios.Count;
                short argsCount = (short)args.Length;
                short differenceValue = (short)(argsCount - audioCount);
                //补齐差值
                for (short i = 0; i < differenceValue; i++)
                {
                    AudioSource audio = AttachAudioSource(attachTarget);
                    multipleAudio[attachTarget].Add(audio);
                }
                for (short i = 0; i < argsCount; i++)
                {
                    var audio= multipleAudio[attachTarget][i];
                    if (audio.isPlaying)
                        audio.Stop();
                    var tempArgs = args[i] as LogicEventArgs<AudioVariable>;
                    SetAudioProperties(ref audio, tempArgs.Data);
                    audio.clip =tempArgs.Data.AudioDataSet.AudioClip;
                    audio.Play();
                }
            }
            else
            {
                multipleAudio.Add(attachTarget, new List<AudioSource>());
                for (int i = 0; i < args.Length; i++)
                {
                    var tempArgs = args[i] as LogicEventArgs<AudioVariable>;
                    AudioSource audio = AttachAudioSource(attachTarget, tempArgs.Data);
                    audio.clip = tempArgs.Data.AudioDataSet.AudioClip;
                    audio.Play();
                    multipleAudio[attachTarget].Add(audio);
                }
            }
        }
        public void PauseMultipleAudio(GameObject attachTarget)
        {
            if (multipleAudio.ContainsKey(attachTarget))
            {
                for (short i = 0; i < multipleAudio[attachTarget].Count; i++)
                {
                    multipleAudio[attachTarget][i].Pause();
                }
            }else
                //Utility.DebugError("AudioManager\n"+"Multiple"+attachTarget.name + "\n is unregistered", attachTarget);
            throw new CFrameworkException("AudioManager\n" + "Multiple" + attachTarget.name + "\n is unregistered");
        }
        public void UnpauseMultipleAudio(GameObject attachTarget)
        {
            if (multipleAudio.ContainsKey(attachTarget))
            {
                for (short i = 0; i < multipleAudio[attachTarget].Count; i++)
                {
                    multipleAudio[attachTarget][i].UnPause();
                }
            }
            else
                //Utility.DebugError("AudioManager\n"+"Multiple" + attachTarget.name + "\n is unregistered", attachTarget);
            throw new CFrameworkException("AudioManager\n" + "Multiple" + attachTarget.name + "\n is unregistered");

        }
        public void StopMultipleAudio(GameObject attachTarget)
        {
            if (multipleAudio.ContainsKey(attachTarget))
            {
                for (short i = 0; i < multipleAudio[attachTarget].Count; i++)
                {
                    multipleAudio[attachTarget][i].Stop();
                }
            }
            else
                //Utility.DebugError("AudioManager\n"+"Multiple" + attachTarget.name + "\n is unregistered", attachTarget);
            throw new CFrameworkException("AudioManager\n" + "Multiple" + attachTarget.name + "\n is unregistered");

        }
        #endregion
        AudioSource CreateAudioSource( AudioVariable arg)
        {
            GameObject go = new GameObject(arg.AudioDataSet.ObjectName);
            go.transform.SetParent(ModuleMountObject.transform);
            go.transform.ResetLocalTransform();
            AudioSource audio = go.AddComponent<AudioSource>();
            SetAudioProperties(ref audio, arg);
            return audio;
        }
        AudioSource AttachAudioSource(GameObject target, AudioVariable arg)
        {
            AudioSource audio = target.AddComponent<AudioSource>();
            SetAudioProperties(ref audio, arg);
            return audio;
        }
        AudioSource AttachAudioSource(GameObject target)
        {
            AudioSource audio = target.AddComponent<AudioSource>();
            return audio;
        }
        void SetAudioProperties(ref AudioSource audio, AudioVariable arg)
        {
            audio.playOnAwake = arg.AudioDataSet.PlayOnAwake;
            audio.volume = arg.AudioDataSet.Volume;
            audio.pitch = arg.AudioDataSet.Speed;
            audio.spatialBlend = arg.AudioDataSet.SpatialBlend;
            audio.mute = arg.AudioDataSet.Mute;
            audio.loop = arg.AudioDataSet.Loop;
        }
        //轮询间距，按照update渲染的5秒计算，不使用真实时间
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
    }
}