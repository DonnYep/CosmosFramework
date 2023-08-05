using System.Collections;
namespace Cosmos.Editor
{
    public static partial class EditorUtil
    {
        /// <summary>
        /// EditorCoroutine 嵌套协程无法识别 yield return IEnumerator；
        /// 嵌套协程尽量使用yield return EditorCoroutine；
        /// </summary>
        public static class Coroutine
        {
            /// <summary>
            /// EditorCoroutine 嵌套协程无法识别 yield return IEnumerator；
            /// 嵌套协程尽量使用yield return EditorCoroutine；
            /// </summary>
            public static Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutine StartCoroutine(IEnumerator coroutine)
            {
                return Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(coroutine);
            }
            public static void StopCoroutine(Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutine coroutine)
            {
                Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StopCoroutine(coroutine);
            }
            public static void StopCoroutine(IEnumerator coroutine)
            {
                Cosmos.Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(coroutine);
            }
        }
    }
}
