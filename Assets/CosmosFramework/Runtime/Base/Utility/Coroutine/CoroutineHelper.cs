using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    [DisallowMultipleComponent]
    public class CoroutineHelper : MonoBehaviour
    {
        class DelayTask : IEquatable<DelayTask>, IReference
        {
            public long TaskId;
            public float Delay;
            public float CurrentTime;
            public Action Action;
            public bool Equals(DelayTask other)
            {
                return other.TaskId == this.TaskId;
            }
            public void Release()
            {
                TaskId = -1;
                Delay = 0;
                CurrentTime = 0;
                Action = null;
            }
        }
        List<IEnumerator> routineList = new List<IEnumerator>();
        static long taskIndex = 0;
        readonly Dictionary<long, DelayTask> delayTaskDict = new Dictionary<long, DelayTask>();
        readonly List<DelayTask> delayTaskList = new List<DelayTask>();
        readonly List<DelayTask> removalDelayTaskList = new List<DelayTask>();
        /// <summary>
        /// 加入延迟任务
        /// </summary>
        /// <param name="delay">延迟时间，delay>=0</param>
        /// <param name="action">触发的事件</param>
        /// <returns>任务Id</returns>
        public long AddDelayTask(float delay, Action action)
        {
            if (delay < 0)
                delay = 0;
            var delayTask = ReferencePool.Acquire<DelayTask>();
            delayTask.TaskId = taskIndex;
            delayTask.CurrentTime = 0;
            delayTask.Delay = delay;
            delayTask.Action = action;

            delayTaskList.Add(delayTask);
            delayTaskDict.Add(delayTask.TaskId, delayTask);
            if (taskIndex == int.MaxValue)
                taskIndex = 0;
            else
                taskIndex++;

            return delayTask.TaskId;
        }
        /// <summary>
        /// 移除延迟任务，已触发的则自动移除
        /// </summary>
        /// <param name="taskId">任务Id</param>
        public void RemoveDelayTask(long taskId)
        {
            if (delayTaskDict.TryRemove(taskId, out var delayTask))
            {
                delayTaskList.Remove(delayTask);
                ReferencePool.Release(delayTask);
            }
        }
        public void StopAllDelayTask()
        {
            delayTaskList.Clear();
            delayTaskDict.Clear();
        }
        /// <summary>
        /// 条件协程；
        /// </summary>
        /// <param name="handler">目标条件</param>
        /// <param name="callBack">条件达成后执行的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine PredicateCoroutine(Func<bool> handler, Action callBack)
        {
            return StartCoroutine(EnumPredicateCoroutine(handler, callBack));
        }
        /// <summary>
        /// 嵌套协程；
        /// </summary>
        /// <param name="predicateHandler">条件函数</param>
        /// <param name="nestHandler">条件成功后执行的嵌套协程</param>
        /// <returns>Coroutine></returns>
        public Coroutine PredicateNestCoroutine(Func<bool> predicateHandler, Action nestHandler)
        {
            return StartCoroutine(EnumPredicateNestCoroutine(predicateHandler, nestHandler));
        }
        /// <summary>
        /// 延时协程；
        /// </summary>
        /// <param name="delay">延时的时间</param>
        /// <param name="callBack">延时后的回调函数</param>
        /// <returns>协程对象</returns>
        public Coroutine DelayCoroutine(float delay, Action callBack)
        {
            return StartCoroutine(EnumDelay(delay, callBack));
        }
        public Coroutine StartCoroutine(Action handler)
        {
            return StartCoroutine(EnumCoroutine(handler));
        }
        public Coroutine StartCoroutine(Action handler, Action callback)
        {
            return StartCoroutine(EnumCoroutine(handler, callback));
        }
        /// <summary>
        /// 嵌套协程
        /// </summary>
        /// <param name="routine">执行条件</param>
        /// <param name="callBack">执行条件结束后自动执行回调函数</param>
        /// <returns>Coroutine</returns>
        public Coroutine StartCoroutine(Coroutine routine, Action callBack)
        {
            return StartCoroutine(EnumCoroutine(routine, callBack));
        }
        public void AddCoroutine(IEnumerator routine)
        {
            routineList.Add(routine);
        }
        void Update()
        {
            while (routineList.Count > 0)
            {
                var routine = routineList[0];
                routineList.RemoveAt(0);
                StartCoroutine(routine);
            }
            RefreshDelayTask();
        }
        IEnumerator EnumDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }
        IEnumerator EnumCoroutine(Coroutine routine, Action callBack)
        {
            yield return routine;
            callBack?.Invoke();
        }
        IEnumerator EnumCoroutine(Action handler)
        {
            handler?.Invoke();
            yield return null;
        }
        IEnumerator EnumCoroutine(Action handler, Action callack)
        {
            yield return StartCoroutine(handler);
            callack?.Invoke();
        }
        IEnumerator EnumPredicateCoroutine(Func<bool> handler, Action callBack)
        {
            yield return new WaitUntil(handler);
            callBack();
        }
        /// <summary>
        /// 嵌套协程执行体；
        /// </summary>
        /// <param name="predicateHandler">条件函数</param>
        /// <param name="nestHandler">条件成功后执行的嵌套协程</param>
        IEnumerator EnumPredicateNestCoroutine(Func<bool> predicateHandler, Action nestHandler)
        {
            yield return new WaitUntil(predicateHandler);
            yield return StartCoroutine(EnumCoroutine(nestHandler));
        }
        void RefreshDelayTask()
        {
            removalDelayTaskList.Clear();
            var taskArray = delayTaskList.ToArray();
            var taskCount = taskArray.Length;
            for (int i = 0; i < taskCount; i++)
            {
                var task = taskArray[i];
                task.CurrentTime += Time.deltaTime;
                if (task.CurrentTime >= task.Delay)
                {
                    try
                    {
                        task.Action?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Utility.Debug.LogError(e);
                    }
                    removalDelayTaskList.Add(task);
                }
            }
            var removeCount = removalDelayTaskList.Count;
            for (int i = 0; i < removeCount; i++)
            {
                var removeTask = removalDelayTaskList[i];
                RemoveDelayTask(removeTask.TaskId);
            }
        }
    }
}
