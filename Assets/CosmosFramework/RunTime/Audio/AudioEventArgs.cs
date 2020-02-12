using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Cosmos
{
    public class AudioEventArgs : GameEventArgs
    {
        public AudioDataSet AudioDataSet { get; set; }
        public override void Reset()
        {
            AudioDataSet.Reset();
        }
        public GameObject MountObject { get; set; }
    }
}