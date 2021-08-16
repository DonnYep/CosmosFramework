using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
