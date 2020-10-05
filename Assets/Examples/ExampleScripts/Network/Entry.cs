using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
public class Entry : MonoBehaviour
{
    private void Awake()
    {
        Utility.Debug.SetHelper(new UnityDebugHelper());
        Facade.CheckCosmosModule();
    }
}
