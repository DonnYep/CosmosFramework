using Cosmos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVariable : Variable
{
    public AudioDataSet AudioDataSet { get; set; }
    public GameObject MountObject { get; set; }

    public override void Clear()
    {
        AudioDataSet.Reset();
        MountObject = null;
    }
}
