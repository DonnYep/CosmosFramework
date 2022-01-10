using UnityEngine;

namespace Cosmos.Audio
{
    public interface IAudioObject
    {
        string AudioName { get; }
        string AudioGroupName { get; }
        AudioClip AudioClip { get; }
    }
}
