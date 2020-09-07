using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
public class TempController : ControllerBase
{
    public override void OnRefresh()
    {
        Utility.Debug.LogInfo("IController OnRefresh",MessageColor.INDIGO);
    }

}
