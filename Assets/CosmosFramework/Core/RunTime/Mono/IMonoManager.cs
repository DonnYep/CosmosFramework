using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
namespace Cosmos.Mono
{
    public interface IMonoManager: IModuleManager
    {
        Coroutine StartCoroutine(Coroutine routine, Action callBack);
        Coroutine DelayCoroutine(float delay, Action callBack);
        Coroutine PredicateCoroutine(Func<bool> handler, Action callBack);
        Coroutine StartCoroutine(IEnumerator routine);
        void StopAllCoroutines();
        void StopCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
    }
}
