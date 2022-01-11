using Cosmos.Audio;
using Cosmos.Resource;
using System;
using UnityEngine;

namespace Cosmos
{
    public class DefaultAudioAssetHelper : IAudioAssetHelper
    {
        IResourceManager ResourceManager { get { return CosmosEntry.ResourceManager; } }
        public AudioObject LoadAudio(AudioAssetInfo assetInfo)
        {
            var clip = ResourceManager.LoadAsset<AudioClip>(assetInfo);
            if (clip == null)
                return null;
            var audioObject = new AudioObject(assetInfo.AudioName, clip);
            return audioObject;
        }
        public Coroutine LoadAudioAsync(AudioAssetInfo assetInfo, Action<AudioObject> callback)
        {
            return ResourceManager.LoadAssetAsync<AudioClip>(assetInfo,clip=> 
            {
                if (clip == null)
                {
                    callback?.Invoke(null);
                }
                else
                {
                    var audioObject = new AudioObject(assetInfo.AudioName, clip);
                    callback?.Invoke(audioObject);
                }
            });
        }
    }
}
