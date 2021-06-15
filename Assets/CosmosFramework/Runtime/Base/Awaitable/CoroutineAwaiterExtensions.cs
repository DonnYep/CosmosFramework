using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos
{
    public static class CoroutineAwaiterExtensions
    {
        public static CoroutineAwaiter<IEnumerator> GetAwaiter(this IEnumerator coroutine)
        {
            return new CoroutineAwaiter<IEnumerator>(coroutine);
        }
        public static CoroutineAwaiter<WaitForNextFrame> GetAwaiter(this WaitForNextFrame waitForNextFrame)
        {
            return new CoroutineAwaiter<WaitForNextFrame>(waitForNextFrame);
        }
        public static CoroutineAwaiter<WaitForSeconds> GetAwaiter(this WaitForSeconds waitForSeconds)
        {
            return new CoroutineAwaiter<WaitForSeconds>(waitForSeconds);
        }
        public static CoroutineAwaiter<WaitForSecondsRealtime> GetAwaiter(this WaitForSecondsRealtime waitForSecondsRealtime)
        {
            return new CoroutineAwaiter<WaitForSecondsRealtime>(waitForSecondsRealtime);
        }
        public static CoroutineAwaiter<WaitForEndOfFrame> GetAwaiter(this WaitForEndOfFrame waitForEndOfFrame)
        {
            return new CoroutineAwaiter<WaitForEndOfFrame>(waitForEndOfFrame);
        }
        public static CoroutineAwaiter<WaitForFixedUpdate> GetAwaiter(this WaitForFixedUpdate waitForFixedUpdate)
        {
            return new CoroutineAwaiter<WaitForFixedUpdate>(waitForFixedUpdate);
        }
        public static CoroutineAwaiter<WaitUntil> GetAwaiter(this WaitUntil waitUntil)
        {
            return new CoroutineAwaiter<WaitUntil>(waitUntil);
        }
        public static CoroutineAwaiter<WaitWhile> GetAwaiter(this WaitWhile waitWhile)
        {
            return new CoroutineAwaiter<WaitWhile>(waitWhile);
        }
        public static CoroutineAwaiter<WWW> GetAwaiter(this WWW www)
        {
            return new CoroutineAwaiter<WWW>(www);
        }
        public static CoroutineAwaiter<UnityWebRequest> GetAwaiter(this UnityWebRequest unityWebRequest)
        {
            return new CoroutineAwaiter<UnityWebRequest>(unityWebRequest);
        }
        public static CoroutineAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation asyncOperation)
        {
            return new CoroutineAwaiter<AsyncOperation>(asyncOperation);
        }
        public static CoroutineAwaiter<CustomYieldInstruction> GetAwaiter(this CustomYieldInstruction customYieldInstruction)
        {
            return new CoroutineAwaiter<CustomYieldInstruction>(customYieldInstruction);
        }
        public static CoroutineAwaiterWaitForMainThread GetAwaiter(this WaitForMainThread waitForMainThread)
        {
            return new CoroutineAwaiterWaitForMainThread();
        }
    }
    public struct WaitForNextFrame { }
    public struct WaitForMainThread { }
}
