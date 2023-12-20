using Cosmos.Audio;
using Cosmos.Resource;
using System;
using UnityEngine;
namespace Cosmos
{
    public class DefaultAudioAssetHelper : IAudioAssetHelper
    {
        IResourceManager ResourceManager { get { return CosmosEntry.ResourceManager; } }
        public Coroutine LoadAudioAsync(string audioAssetName, Action<AudioObject> loadSuccess, Action loadFailure)
        {
            return ResourceManager.LoadAssetAsync<AudioClip>(audioAssetName, clip=> 
            {
                if (clip == null)
                {
                    loadFailure?.Invoke();
                }
                else
                {
                    var audioObject = new AudioObject(audioAssetName, clip);
                    loadSuccess?.Invoke(audioObject);
                }
            });
        }
        public void UnloadAudio(string audioAssetName)
        {
            ResourceManager.UnloadAsset(audioAssetName);
        }
    }
}
