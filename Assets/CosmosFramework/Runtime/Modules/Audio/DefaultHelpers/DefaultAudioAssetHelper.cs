using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos.Audio
{
    public class DefaultAudioAssetHelper : IAudioAssetHelper
    {
        IResourceManager ResourceManager { get { return CosmosEntry.ResourceManager; } }
        public IAudioObject LoadAudio(AudioAssetInfo assetInfo)
        {
            var clip = ResourceManager.LoadAsset<AudioClip>(assetInfo);
            if (clip == null)
                return null;
            var audioObject = new AudioObject() { AudioClip = clip, AudioName = assetInfo.AudioName };
            return audioObject;
        }
        public Coroutine LoadAudioAsync(AudioAssetInfo assetInfo, Action<IAudioObject> callback)
        {
            return ResourceManager.LoadAssetAsync<AudioClip>(assetInfo,clip=> 
            {
                if (clip == null)
                {
                    callback?.Invoke(null);
                }
                else
                {
                    var audioObject = new AudioObject() { AudioClip = clip, AudioName = assetInfo.AudioName };
                    callback?.Invoke(audioObject);
                }
            });
        }
    }
}
