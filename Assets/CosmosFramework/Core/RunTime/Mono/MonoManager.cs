using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.ComponentModel;

namespace Cosmos.Mono
{
    /// <summary>
    /// 不继承自mono的对象通过这个管理器来实现update等需要mono才能做到的功能
    /// 当前只生成一个mc
    /// </summary>
    [Module]
    internal sealed class MonoManager : Module<MonoManager>
    {
        IMonoProvider monoProvider;
        public override void OnInitialization()
        {
            monoProvider = (new GameObject(typeof(MonoProvider).Name)).AddComponent<MonoProvider>();
        }
        #region Methods
        /// <summary>
        /// 嵌套协程
        /// </summary>
        /// <param name="routine">执行条件</param>
        /// <param name="callBack">执行条件结束后自动执行回调函数</param>
        /// <returns>Coroutine</returns>
        internal Coroutine StartCoroutine(Coroutine routine, Action callBack)
        {
            return monoProvider.StartCoroutine(routine, callBack);
        }
        internal Coroutine DelayCoroutine(float delay, Action callBack)
        {
            return monoProvider.DelayCoroutine(delay, callBack);
        }
        internal Coroutine PredicateCoroutine(Func<bool> handler, Action callBack)
        {
            return monoProvider.PredicateCoroutine(handler, callBack);
        }
        internal Coroutine StartCoroutine(IEnumerator routine)
        {
            return monoProvider.StartCoroutine(routine);
        }
        internal void StopAllCoroutines()
        {
            monoProvider.StopAllCoroutines();
        }
        /// <summary>
        /// 关闭协程
        /// </summary>
        /// <param name="methodName"></param>
        internal void StopCoroutine(IEnumerator routine)
        {
            monoProvider.StopCoroutine(routine);
        }
        internal void StopCoroutine(Coroutine routine)
        {
            monoProvider.StopCoroutine(routine);
        }
        #endregion
    }
}