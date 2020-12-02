using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using System;

public class TempController : ControllerBase<TempController>
{
    protected override void RefreshHandler()
    {
        Utility.Debug.LogInfo("IController OnRefresh",MessageColor.INDIGO);
    }
}
