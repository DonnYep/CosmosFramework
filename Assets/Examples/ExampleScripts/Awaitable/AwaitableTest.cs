using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using Cosmos.Quark;
public class AwaitableTest : MonoBehaviour
{
    private async void Start()
    {
        Debug.Log("AwaitableTest >>> Before Coroutine Start");
        await StartCoroutine(EnumRun());
        //await EnumRun();
        Debug.LogError("AwaitableTest >>> After Coroutine Start");
    }
    IEnumerator EnumRun()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("AwaitableTest >>> After IEnumerator EnumRun");
    }

}
