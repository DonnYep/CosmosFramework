using System.Collections.Generic;

namespace Cosmos.Audio
{
    /// <summary>
    /// 声音组；
    /// 这里声音组是一个容器，用于存储IAudioObject，逻辑由MGR执行；
    /// </summary>
    internal class AudioGroup: IAudioGroup
    {
        Dictionary<string, IAudioObject> audioDict = new Dictionary<string, IAudioObject>();
        public int AudioCount { get { return audioDict.Count; } }
        public string AudioGroupName { get; set; }
        public Dictionary<string, IAudioObject> AudioDict { get { return audioDict; } }
        public bool HasAudio(string audioName)
        {
            return audioDict.ContainsKey(audioName);
        }
        public void AddOrUpdateAudio(string audioName, IAudioObject audio)
        {
            var has = audioDict.TryGetValue(audioName, out var oldAudio);
            if (has)
            {
                oldAudio.CastTo<AudioObject>().AudioGroupName = string.Empty;
            }
            audio.CastTo<AudioObject>().AudioGroupName = AudioGroupName;
            audioDict[audioName] = audio;
        }
        public bool RemoveAudio(string audioName)
        {
            var result = audioDict.TryRemove(audioName, out var audio);
            if (result)
                audio.CastTo<AudioObject>().AudioGroupName = string.Empty;
            return result;
        }
        public void RemoveAllAudios()
        {
            foreach (var ao in audioDict)
            {
                ao.Value.CastTo<AudioObject>().AudioGroupName = string.Empty;
            }
            audioDict.Clear();
        }
        public void Release()
        {
            audioDict.Clear();
        }
    }
}
