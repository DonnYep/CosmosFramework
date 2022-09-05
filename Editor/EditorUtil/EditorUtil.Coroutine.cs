using System.Collections;
using Cosmos.Unity.EditorCoroutines.Editor;
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
            public static EditorCoroutine StartCoroutine(IEnumerator coroutine)
            {
                return EditorCoroutineUtility.StartCoroutineOwnerless(coroutine);
            }
            public static void StopCoroutine(EditorCoroutine coroutine)
            {
                EditorCoroutineUtility.StopCoroutine(coroutine);
            }
            public static void StopCoroutine(IEnumerator coroutine)
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(coroutine);
            }
        }
    }
}
