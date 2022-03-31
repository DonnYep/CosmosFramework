using System.Collections.Generic;
namespace Cosmos.Audio
{
    public interface IAudioGroup
    {
        string AudioGroupName { get; set; }
        int AudioCount { get; }
        Dictionary<string, AudioObject> AudioDict { get; }
        bool HasAudio(string audioName);
        void AddOrUpdateAudio(string audioName, AudioObject audio);
        bool RemoveAudio(string audioName);
        void RemoveAllAudios();
        void Release();
    }
}
