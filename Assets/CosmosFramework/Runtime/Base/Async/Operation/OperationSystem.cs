using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Operation
{
    /// <summary>
    /// 异步操作调度系统（框架异步底座）。
    /// <para>所在程序集：<c>Cosmos.Async</c>（<c>Runtime/Base/Async</c>），不依赖任何业务模块。</para>
    /// <para>职责：</para>
    /// <para>1、登记 <see cref="OperationBase"/> 并按 <see cref="OperationBase.Priority"/> 排序；</para>
    /// <para>2、每帧推进（OnUpdate）、在完成时收尾（OnFinish）并移出队列；</para>
    /// <para>3、单个操作抛异常时只把该操作标记为失败，不中断整条异步管线；</para>
    /// <para>4、为"等待协程"的操作提供协程执行器（<see cref="RunCoroutine"/>）。</para>
    /// </summary>
    public static class OperationSystem
    {
        static readonly List<OperationBase> operationList = new List<OperationBase>(1000);
        static readonly List<OperationBase> newList = new List<OperationBase>(1000);
        /// <summary>
        /// 异步刷新驱动
        /// </summary>
        static OperationDriver asyncOperationDriver;
        static int operationIndex;
        /// <summary>
        /// 每帧推进（由 <see cref="OperationDriver"/> 调用；也可在无 Unity 环境下手动驱动）
        /// </summary>
        public static void Update()
        {
            // 添加新增的异步操作
            if (newList.Count > 0)
            {
                var sorting = false;
                for (int i = 0; i < newList.Count; i++)
                {
                    if (newList[i].Priority > 0)
                    {
                        sorting = true;
                        break;
                    }
                }
                operationList.AddRange(newList);
                newList.Clear();
                // 重新排序优先级
                if (sorting)
                    operationList.Sort();
            }
            // 更新进行中的异步操作
            for (int i = 0; i < operationList.Count; i++)
            {
                var operation = operationList[i];
                if (operation.IsFinish)
                    continue;
                if (!operation.IsDone)
                {
                    try
                    {
                        operation.InvokeUpdate();
                    }
                    catch (Exception e)
                    {
                        //单个操作抛异常：标记失败并走正常收尾，避免每帧重复抛、或阻塞其它操作
                        operation.InvokeFail(e);
                    }
                }
                if (!operation.IsDone)
                    continue;
                try
                {
                    operation.InvokeFinish();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
            // 移除已经完成的异步操作
            for (int i = operationList.Count - 1; i >= 0; i--)
            {
                if (operationList[i].IsFinish)
                    operationList.RemoveAt(i);
            }
        }
        /// <summary>
        /// 销毁异步操作系统（模块终止/场景重载时调用）
        /// </summary>
        public static void DestroyAll()
        {
            operationList.Clear();
            newList.Clear();
        }
        /// <summary>
        /// 登记并开始驱动一个异步操作
        /// </summary>
        /// <param name="operation">异步操作</param>
        public static void StartOperation(OperationBase operation)
        {
            if (operation == null)
                return;
            CheckAsyncOperationDriver();
            if (operation.OperationId == 0)
                operation.OperationId = ++operationIndex;
            newList.Add(operation);
            try
            {
                operation.InvokeStart();
            }
            catch (Exception e)
            {
                operation.InvokeFail(e);
            }
        }
        /// <summary>
        /// 按 Id 查找在途操作（调试/诊断用）
        /// </summary>
        /// <param name="operationId">操作 Id</param>
        /// <param name="operation">操作</param>
        /// <returns>是否存在</returns>
        public static bool TryGetOperation(int operationId, out OperationBase operation)
        {
            if (operationId != 0)
            {
                for (int i = 0; i < operationList.Count; i++)
                {
                    var candidate = operationList[i];
                    if (candidate.IsFinish)
                        continue;
                    if (candidate.OperationId == operationId)
                    {
                        operation = candidate;
                        return true;
                    }
                }
                for (int i = 0; i < newList.Count; i++)
                {
                    var candidate = newList[i];
                    if (candidate.IsFinish)
                        continue;
                    if (candidate.OperationId == operationId)
                    {
                        operation = candidate;
                        return true;
                    }
                }
            }
            operation = null;
            return false;
        }
        /// <summary>
        /// 当前在途操作数量
        /// </summary>
        public static int OperationCount
        {
            get { return operationList.Count + newList.Count; }
        }
        /// <summary>
        /// 用框架驱动组件跑一个协程（供"等待协程"的操作使用，业务无需自己找 MonoBehaviour）
        /// </summary>
        /// <param name="routine">协程</param>
        /// <returns>协程句柄；驱动不可用时为 null</returns>
        internal static Coroutine RunCoroutine(IEnumerator routine)
        {
            if (routine == null)
                return null;
            CheckAsyncOperationDriver();
            return asyncOperationDriver != null ? asyncOperationDriver.StartCoroutine(routine) : null;
        }
        /// <summary>
        /// 停止由 <see cref="RunCoroutine"/> 启动的协程
        /// </summary>
        /// <param name="coroutine">协程句柄</param>
        internal static void StopCoroutine(Coroutine coroutine)
        {
            if (coroutine == null || asyncOperationDriver == null)
                return;
            asyncOperationDriver.StopCoroutine(coroutine);
        }
        static void CheckAsyncOperationDriver()
        {
            if (asyncOperationDriver != null)
                return;
            var driverObject = new GameObject("AsyncOperationDriver");
            driverObject.hideFlags = HideFlags.HideInHierarchy;
            GameObject.DontDestroyOnLoad(driverObject);
            asyncOperationDriver = driverObject.AddComponent<OperationDriver>();
        }
    }
}
