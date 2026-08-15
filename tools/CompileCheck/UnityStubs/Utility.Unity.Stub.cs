// Stub partial for Cosmos.Utility.Unity used by the WebRequest compile-check.
// The real implementation (Utility.Unity.cs) pulls in UnityEngine.UI etc., which is out of scope here.
using System.Collections;
using UnityEngine;

namespace Cosmos
{
    public static partial class Utility
    {
        public static class Unity
        {
            public static Coroutine StartCoroutine(IEnumerator routine) { return null; }
            public static void StopCoroutine(Coroutine routine) { }
            public static void StopAllCoroutines() { }
        }
    }
}
