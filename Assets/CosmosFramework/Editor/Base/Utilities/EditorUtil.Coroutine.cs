using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.CosmosEditor
{
    public static partial class EditorUtil
    {
        /// <summary>
        /// EditorCoroutine 嵌套协程无法识别 yield return IEnumerator；
        /// 嵌套协程尽量使用yield return EditorCoroutine；
        /// </summary>
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
