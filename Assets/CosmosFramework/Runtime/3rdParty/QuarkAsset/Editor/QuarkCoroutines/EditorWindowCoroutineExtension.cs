using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Quark.Editor
{
    public static class EditorWindowCoroutineExtension
    {
        public static EditorCoroutine StartCoroutine(this EditorWindow window, IEnumerator routine)
        {
            return new EditorCoroutine(routine, window);
        }
        public static void StopCoroutine(this EditorWindow window, EditorCoroutine coroutine)
        {
            if(coroutine == null)
            {
                Debug.LogAssertion("Provided EditorCoroutine handle is null.");
                return;
            }

            if(coroutine.m_Owner == null)
            {
                Debug.LogError("The EditorCoroutine is ownerless. Please use EditorCoroutineEditor.StopCoroutine to terminate such coroutines.");
                return;
            }

            if (!coroutine.m_Owner.IsAlive)
                return; //The EditorCoroutine's owner was already terminated execution will cease next time it is processed

            var owner = coroutine.m_Owner.Target as EditorWindow;

            if (owner == null || owner != null && owner != window)
            {
                Debug.LogErrorFormat("The EditorCoroutine is owned by another object: {0}.", coroutine.m_Owner.Target);
                return;
            }

            EditorCoroutineUtility.StopCoroutine(coroutine);
        }
    }
}