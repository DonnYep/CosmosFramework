using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using System;

public class TempController : ControllerBase<TempController>
{
    protected override void RefreshHandler ()
    {
        Utility.DebugLog("IController OnRefresh",MessageColor.INDIGO);
    }

}
