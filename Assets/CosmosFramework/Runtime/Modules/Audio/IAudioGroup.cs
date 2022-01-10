using System.Collections.Generic;

namespace Cosmos.Audio
{
    public interface IAudioGroup
    {
        string AudioGroupName { get; set; }
        int AudioCount { get; }
        Dictionary<string, IAudioObject> AudioDict { get; }
        bool HasAudio(string audioName);
        void AddOrUpdateAudio(string audioName, IAudioObject audio);
        bool RemoveAudio(string audioName);
        void RemoveAllAudios();
        void Release();
    }
}
