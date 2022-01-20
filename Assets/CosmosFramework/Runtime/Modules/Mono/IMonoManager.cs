using System.Collections;
using UnityEngine;
using System;
namespace Cosmos
{
    public interface IMonoManager: IModuleManager
    {
        Coroutine StartCoroutine(Coroutine routine, Action callBack);
        /// <summary>
        /// 延时协程；
        /// </summary>
        /// <param name="delay">延时的时间</param>
        /// <param name="callBack">延时后的回调函数</param>
        /// <returns>协程对象</returns>
        Coroutine DelayCoroutine(float delay, Action callBack);
        /// <summary>
        /// 条件协程；
        /// </summary>
        /// <param name="handler">目标条件</param>
        /// <param name="callBack">条件达成后执行的回调</param>
        /// <returns>协程对象</returns>
        Coroutine PredicateCoroutine(Func<bool> handler, Action callBack);
        /// <summary>
        /// 嵌套协程；
        /// </summary>
        /// <param name="predicateHandler">条件函数</param>
        /// <param name="nestHandler">条件成功后执行的嵌套协程</param>
        /// <returns>Coroutine></returns>
        Coroutine PredicateNestCoroutine(Func<bool> predicateHandler, Action nestHandler);
        Coroutine StartCoroutine(IEnumerator routine);
        Coroutine StartCoroutine(Action handler);
        void StopAllCoroutines();
        void StopCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine routine);
    }
}
