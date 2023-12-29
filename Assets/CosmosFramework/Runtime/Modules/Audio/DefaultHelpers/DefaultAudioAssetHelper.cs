using Cosmos.Audio;
using Cosmos.Resource;
using System;
using UnityEngine;
namespace Cosmos
{
    public class DefaultAudioAssetHelper : IAudioAssetHelper
    {
        IResourceManager ResourceManager { get { return CosmosEntry.ResourceManager; } }
        /// <inheritdoc/>
        public Coroutine LoadAudioAsync(string audioAssetName, Action<string, AudioClip> onSuccess, Action<string,string> onFailure)
        {
            return ResourceManager.LoadAssetAsync<AudioClip>(audioAssetName, clip =>
            {
                if (clip == null)
                {
                    onFailure?.Invoke(audioAssetName,$"AudioAsset : {audioAssetName} do not exist");
                }
                else
                {
                    onSuccess?.Invoke(audioAssetName, clip);
                }
            });
        }
        /// <inheritdoc/>
        public void UnloadAudio(string audioAssetName)
        {
            ResourceManager.UnloadAsset(audioAssetName);
        }
    }
}
