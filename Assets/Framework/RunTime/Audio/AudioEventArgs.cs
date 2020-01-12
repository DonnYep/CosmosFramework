using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Event;
using System;

namespace Cosmos.Audio
{
    public class AudioEventArgs : GameEventArgs
    {
        public AudioEventObject AudioEventObject { get; set; }
        public override void Reset()
        {
            AudioEventObject.Reset();
        }
        public GameObject MountObject { get; set; }
    }
}