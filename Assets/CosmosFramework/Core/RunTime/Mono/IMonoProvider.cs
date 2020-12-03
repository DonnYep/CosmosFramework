using UnityEngine;
using System.Collections;
using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace Cosmos.Mono
{
    public interface IMonoProvider
    {
        Coroutine StartCoroutine(Coroutine routine, Action callBack);
        Coroutine StartCoroutine(Action handler);
        Coroutine DelayCoroutine(float delay, Action callBack);
        Coroutine PredicateCoroutine(Func<bool> handler, Action callBack);
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
        void StopAllCoroutines();
    }
}