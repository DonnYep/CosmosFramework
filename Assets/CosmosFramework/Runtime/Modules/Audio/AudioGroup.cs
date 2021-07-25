using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Audio
{
    /// <summary>
    /// 声音组；
    /// </summary>
    public class AudioGroup
    {
        Dictionary<string, AudioObject> audioDict = new Dictionary<string, AudioObject>();
        /// <summary>
        /// 轮询时的声音对象缓存；
        /// </summary>
        List<AudioObject> audioCache = new List<AudioObject>();
        /// <summary>
        /// 移除声音时的声音对象缓存；
        /// </summary>
        List<AudioObject> removeAaudioCache = new List<AudioObject>();
        public string AudioGroupName { get; set; }
        bool mute;
        public bool Mute
        {
            get { return mute; }
            set
            {
                if (mute != value)
                {
                    mute = value;
                    var length = audioCache.Count;
                    for (int i = 0; i < length; i++)
                        audioCache[i].Mute = mute;
                }
            }
        }
        /// <summary>
        /// 轮询函数；
        /// </summary>
        public void TickRefresh()
        {
            //var length = audioCache.Count;
            //if (length <= 0)
            //    return;
            //for (int i = 0; i < length; i++)
            //{
            //    if (!audioCache[i].IsPlaying)
            //    {
            //        removeAaudioCache.Add(audioCache[i]);
            //    }
            //}
            //var removeLength = removeAaudioCache.Count;
            //for (int i = 0; i < removeLength; i++)
            //{
            //    audioDict.Remove(removeAaudioCache[i].AudioName);
            //    audioCache.Remove(audioCache[i]);
            //}
        }
        public bool HasAudio(string audioName)
        {
            return audioDict.ContainsKey(audioName);
        }
        public bool AddAudio(string audioName, AudioObject audio)
        {
            var result = audioDict.TryAdd(audioName, audio);
            if (result)
                audioCache.Add(audio);
            return result;
        }
        public bool RemoveAudio(string audioName)
        {
            var result = audioDict.TryRemove(audioName, out var audio);
            if (result)
                audioCache.Remove(audio);
            return result;
        }
        public void RemoveAllAudios()
        {
            audioDict.Clear();
            audioCache.Clear();
        }
        public void Release()
        {

        }
    }
}
