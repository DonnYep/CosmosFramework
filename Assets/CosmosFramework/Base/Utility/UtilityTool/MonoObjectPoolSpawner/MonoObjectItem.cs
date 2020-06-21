using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using System;

public class MonoObjectItem : MonoObjectBase
{
    public override Type Type { get { return this.GetType(); } }
    public override void OnDespawn()
    {
        Utility.DebugLog("MonoObjectItem OnDespawn");
    }
    public override void OnInitialization()
    {
        Utility.DebugLog("MonoObjectItem OnInitialization");
    }
    public override void OnSpawn()
    {
        Utility.DebugLog("MonoObjectItem OnSpawn");
    }
    public override void OnTermination()
    {
        Utility.DebugLog("MonoObjectItem OnTermination");
    }
}
