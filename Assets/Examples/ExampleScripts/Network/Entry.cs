using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using Cosmos.Input;
[DefaultExecutionOrder(1000)]
public class Entry : MonoBehaviour
{
    private void Awake()
    {
        Utility.Debug.SetHelper(new UnityDebugHelper());
        GameManager.PreparatoryModule();
    }
}
