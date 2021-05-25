using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.CosmosEditor
{
    public static partial class EditorUtilities
    {
        public static class Coroutine
        {
            public static EditorCoroutine StartCoroutine(IEnumerator coroutine)
            {
                return EditorCoroutineCore.StartCoroutine(coroutine);
            }
            public static void StopCoroutine(EditorCoroutine coroutine)
            {
                EditorCoroutineCore.StopCoroutine(coroutine);
            }
            public static void StopCoroutine(IEnumerator coroutine)
            {
                EditorCoroutineCore.StopCoroutine(coroutine);
            }
        }
    }
}
