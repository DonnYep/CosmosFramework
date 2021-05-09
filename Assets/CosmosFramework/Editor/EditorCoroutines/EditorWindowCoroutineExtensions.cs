using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Cosmos.CosmosEditor
{
    public static class EditorWindowCoroutineExtensions
    {
		public static EditorCoroutine StartCoroutine(this EditorWindow thisRef, IEnumerator coroutine)
		{
			return EditorCoroutineCore.StartCoroutine(coroutine, thisRef);
		}
		public static EditorCoroutine StartCoroutine(this EditorWindow thisRef, string methodName)
		{
			return EditorCoroutineCore.StartCoroutine(methodName, thisRef);
		}
		public static EditorCoroutine StartCoroutine(this EditorWindow thisRef, string methodName, object value)
		{
			return EditorCoroutineCore.StartCoroutine(methodName, value, thisRef);
		}
		public static void StopCoroutine(this EditorWindow thisRef, IEnumerator coroutine)
		{
			EditorCoroutineCore.StopCoroutine(coroutine, thisRef);
		}
		public static void StopCoroutine(this EditorWindow thisRef, string methodName)
		{
			EditorCoroutineCore.StopCoroutine(methodName, thisRef);
		}
		public static void StopAllCoroutines(this EditorWindow thisRef)
		{
			EditorCoroutineCore.StopAllCoroutines(thisRef);
		}
	}
}
