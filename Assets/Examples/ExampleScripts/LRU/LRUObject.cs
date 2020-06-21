using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using System;

public class LRUObject : MonoObjectBase
{
    public override  Type Type { get { return this.GetType(); } }
    public override  void OnSpawn()
    {
        Utility.DebugLog("LRUObject OnSpawn");
    }
    public override void OnDespawn()
    {
        Utility.DebugLog("LRUObject OnDespawn");
    }
    public override  void OnInitialization()
    {
        Utility.DebugLog("LRUObject OnInitialization");
    }
    public override void OnTermination()
    {
        Utility.DebugLog("LRUObject OnTermination");
        GameManagerAgent.KillObject(this.gameObject);
    }
}
