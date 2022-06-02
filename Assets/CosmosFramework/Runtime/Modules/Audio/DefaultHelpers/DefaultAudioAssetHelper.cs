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
        public Coroutine LoadAudioAsync(AudioAssetInfo assetInfo, Action<AudioObject> loadSuccess, Action loadFailure)
        {
            return ResourceManager.LoadAssetAsync<AudioClip>(assetInfo,clip=> 
            {
                if (clip == null)
                {
                    loadFailure?.Invoke();
                }
                else
                {
                    var audioObject = new AudioObject(assetInfo.AudioName, clip);
                    loadSuccess?.Invoke(audioObject);
                }
            });
        }
    }
}
