using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Cosmos
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