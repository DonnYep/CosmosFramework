using System.Collections.Generic;
namespace Cosmos.Audio
{
    internal sealed partial class AudioManager
    {
        /// <summary>
        /// 声音组；
        /// 这里声音组是一个容器，用于存储IAudioObject，逻辑由MGR执行；
        /// </summary>
        private class AudioGroup : IAudioGroup
        {
            Dictionary<string, AudioObject> audioDict = new Dictionary<string, AudioObject>();
            public int AudioCount { get { return audioDict.Count; } }
            public string AudioGroupName { get; set; }
            public Dictionary<string, AudioObject> AudioDict { get { return audioDict; } }
            public bool HasAudio(string audioName)
            {
                return audioDict.ContainsKey(audioName);
            }
            public void AddOrUpdateAudio(string audioName, AudioObject audio)
            {
                var has = audioDict.TryGetValue(audioName, out var oldAudio);
                if (has)
                {
                    oldAudio.SetGroup(string.Empty);
                }
                audio.SetGroup(AudioGroupName);
                audioDict[audioName] = audio;
            }
            public bool RemoveAudio(string audioName)
            {
                var result = audioDict.TryRemove(audioName, out var audio);
                if (result)
                    audio.SetGroup(string.Empty);
                return result;
            }
            public void RemoveAllAudios()
            {
                foreach (var ao in audioDict)
                {
                    ao.Value.SetGroup(string.Empty);
                }
                audioDict.Clear();
            }
            public void Release()
            {
                audioDict.Clear();
            }
        }

        /// <summary>
        /// 音效组池；
        /// </summary>
        private class AudioGroupPool
        {
            Pool<IAudioGroup> audioGroupPool;
            public AudioGroupPool()
            {
                audioGroupPool = new Pool<IAudioGroup>(() => { return new AudioGroup(); }, ag => { ag.Release(); });
            }
            public void Despawn(IAudioGroup audioGroup)
            {
                audioGroupPool.Despawn(audioGroup);
            }
            public IAudioGroup Spawn()
            {
                return audioGroupPool.Spawn();
            }
        }
    }
}