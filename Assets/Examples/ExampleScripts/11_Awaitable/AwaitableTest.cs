using System.Collections;
using UnityEngine;
using Cosmos;
using System.Threading.Tasks;
using Cosmos.Awaitable;
public class AwaitableTest : MonoBehaviour
{
    private async void Start()
    {
        Debug.Log("AwaitableTest >>> Before Coroutine Start");
        await StartCoroutine(EnumRun());
        await new WaitForSeconds(3);
        await EnumRun();
        Debug.Log("AwaitableTest >>> After Coroutine Start");
        await EnumWait();
        var task = Task.Run(async
           () =>
           {
               await Task.Delay(3000);
               Debug.Log("Task.Delay(3000) run Done");
           });
        await task;
        var routineTask = Task.Run(async
           () =>
        {
            await Task.Delay(1000);
            Debug.Log("routineTask delay 1000  run Done");
        });
        await routineTask.AsCoroutine();
        Debug.Log("await routineTask  delay 3000 Done");
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
