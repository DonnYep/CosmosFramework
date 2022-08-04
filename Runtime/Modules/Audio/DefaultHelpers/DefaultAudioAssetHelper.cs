using Cosmos.Audio;
using Cosmos.Resource;
using System;
using UnityEngine;
namespace Cosmos
{
    public class DefaultAudioAssetHelper : IAudioAssetHelper
    {
        IResourceManager ResourceManager { get { return CosmosEntry.ResourceManager; } }
        public Coroutine LoadAudioAsync(AudioAssetInfo assetInfo, Action<AudioObject> loadSuccess, Action loadFailure)
        {
            return ResourceManager.LoadAssetAsync<AudioClip>(assetInfo.AssetName,clip=> 
            {
                if (clip == null)
                {
                    loadFailure?.Invoke();
                }
                else
                {
                    var audioObject = new AudioObject(assetInfo.AssetName, clip);
                    loadSuccess?.Invoke(audioObject);
                }
            });
        }
    }
}
