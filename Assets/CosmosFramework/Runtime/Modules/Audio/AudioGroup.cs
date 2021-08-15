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
        Dictionary<string, IAudioObject> audioDict = new Dictionary<string, IAudioObject>();
        /// <summary>
        /// 轮询时的声音对象缓存；
        /// </summary>
        List<IAudioObject> audioCache = new List<IAudioObject>();
        /// <summary>
        /// 移除声音时的声音对象缓存；
        /// </summary>
        List<IAudioObject> removeAaudioCache = new List<IAudioObject>();
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
                    //for (int i = 0; i < length; i++)
                    //    audioCache[i].Mute = mute;
                }
            }
        }
        public bool HasAudio(string audioName)
        {
            return audioDict.ContainsKey(audioName);
        }
        public bool AddAudio(string audioName, IAudioObject audio)
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
