using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
public class AwaitableTest : MonoBehaviour
{
    private async void Start()
    {
        Debug.Log("AwaitableTest >>> Before Coroutine Start");
        await StartCoroutine(EnumRun());
        await new WaitForSeconds(3);
        await EnumRun();
        Debug.LogError("AwaitableTest >>> After Coroutine Start");
        await EnumWait();
    }
    IEnumerator EnumRun()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("AwaitableTest >>> After IEnumerator EnumRun");
    }
    IEnumerator EnumWait()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("AwaitableTest >>> After IEnumerator EnumWait");
    }
}
