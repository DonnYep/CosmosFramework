using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Cosmos.Hotfix
{
    public enum HotfixMethodType
    {
        Invalid,
        Action,
        Action_1Arg,
        Action_2Arg,
        Action_3Arg,
        Func,
        Func_1Arg,
        Func_2Arg,
        Func_3Arg
    }
    /// <summary>
    /// 热修复管理器
    /// </summary>
    [Module]
    internal class HotfixManager : Module , IHotfixManager
    {
        #region Properties
        /// <summary>
        /// 热更新DLL
        /// </summary>
        public TextAsset HotfixDll { get; private set; }
        /// <summary>
        /// 热更新程序集
        /// </summary>
        public Assembly HotfixAssembly { get; private set; }
        /// <summary>
        /// 热更新环境
        /// </summary>
        public object HotfixEnvironment { get; private set; }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        public Dictionary<HotfixMethodType, Dictionary<string, MethodInfo>> FixedMethods { get; private set; }  
        /// <summary>
        /// 热修复后的方法
        /// </summary>
        public Dictionary<HotfixMethodType, Dictionary<string, Delegate>> FixedDelegates { get; private set; } 
        #endregion
        #region Methods
        public override void OnInitialization()
        {
            FixedMethods = new Dictionary<HotfixMethodType, Dictionary<string, MethodInfo>>();
            FixedDelegates = new Dictionary<HotfixMethodType, Dictionary<string, Delegate>>();
        }
        public Action FixMethod(Action action)
        {
            return null;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Action<T> FixMethod<T>(Action<T> action)
        {
            return null;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Action<T1, T2> FixMethod<T1, T2>(Action<T1, T2> action)
        {
            return null;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Action<T1, T2, T3> FixMethod<T1, T2, T3>(Action<T1, T2, T3> action)
        {
            return null;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Func<TResult> FixMethod<TResult>(Func<TResult> action)
        {
            return null;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Func<T, TResult> FixMethod<T, TResult>(Func<T, TResult> action)
        {
            return null;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Func<T1, T2, TResult> FixMethod<T1, T2, TResult>(Func<T1, T2, TResult> action)
        {
            return null;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public Func<T1, T2, T3, TResult> FixMethod<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> action)
        {
            return null;
        }
        #endregion

    }
}