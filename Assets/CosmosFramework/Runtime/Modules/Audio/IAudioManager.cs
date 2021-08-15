using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    public interface IAudioManager :IModuleManager
    {
        bool Mute { get; set; }

    }
}