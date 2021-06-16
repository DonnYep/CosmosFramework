using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    //================================================
    //1、FutureTask是异步的监听器，，本身不具备异步逻辑处理功能。
    //2、condition是FutureTask需要检测的条件。当返回值为true时，FutureTask
    //表示这个异步完成了条件，并回收FutureTask；
    //3、FutureTask带有Polling与Completed回调。异步可通过这两个回调进行
    //状态检测；
    //================================================
    /// <summary>
    ///FutureTask是异步的监听器，包含异步完成时的检测，本身不具备异步逻辑处理功能；
    ///异步操作本身请使用真正的异步操作；
    /// </summary>
    /// <see cref="Task">Return vaalue</see>
    /// <see cref="UnityEngine. Coroutine">Return vaalue</see>
    public class FutureTask : IReference
    {
        static int FutureTaskIndex = 0;
        Action<FutureTask> completed;
        Action<FutureTask> polling;
        bool available;

        public bool Available { get { return available; } }
        public int FutureTaskId { get; private set; }
        public string Description { get; private set; }
        public float ElapsedTime { get; private set; }
        public Func<bool> Condition { get; private set; }
        public FutureTask() { }
        public FutureTask(Func<bool> condition, Action<FutureTask> polling, Action<FutureTask> completed, string description = null)
        {
            FutureTaskId = FutureTaskIndex++;
            Condition = condition;
            available = true;
            this.polling = polling;
            this.completed = completed;
            Description = description;
            FutureTaskMonitor.Instance.AddFutureTask(this);
        }
        public FutureTask(Func<bool> condition, Action<FutureTask> completed, string description = null)
        {
            FutureTaskId = FutureTaskIndex++;
            Condition = condition;
            available = true;
            this.completed = completed;
            Description = description;
            FutureTaskMonitor.Instance.AddFutureTask(this);
        }
        public FutureTask(Func<bool> condition, string description = null)
        {
            FutureTaskId = FutureTaskIndex++;
            Condition = condition;
            Description = description;
            available = true;
            FutureTaskMonitor.Instance.AddFutureTask(this);
        }
        public void Release()
        {
            Condition = null;
            FutureTaskId = 0;
            Description = string.Empty;
            ElapsedTime = 0;
            available = true;
        }
        /// <summary>
        /// 终止；
        /// </summary>
        public void Abort()
        {
            available = false;
        }
        /// <summary>
        /// 检测一个状态；
        /// </summary>
        /// <param name="condition">需要检测的状态</param>
        /// <param name="polling">轮询行为</param>
        /// <param name="completed">完成事件</param>
        /// <param name="description">描述</param>
        /// <returns>FutureTask异步检测对象</returns>
        public static FutureTask Detection(Func<bool> condition, Action<FutureTask> polling, Action<FutureTask> completed, string description = null)
        {
            var futureTask = ReferencePool.Accquire<FutureTask>();
            futureTask.FutureTaskId = FutureTaskIndex++;
            futureTask.Condition = condition;
            futureTask.available = true;
            futureTask.completed = completed;
            futureTask.polling = polling;
            futureTask.Description = description;
            FutureTaskMonitor.Instance.AddFutureTask(futureTask);
            return futureTask;
        }
        /// <summary>
        /// 检测一个状态；
        /// </summary>
        /// <param name="condition">需要检测的状态</param>
        /// <param name="completed">完成事件</param>
        /// <param name="description">描述</param>
        /// <returns>FutureTask异步检测对象</returns>
        public static FutureTask Detection(Func<bool> condition, Action<FutureTask> completed, string description = null)
        {
            var futureTask = ReferencePool.Accquire<FutureTask>();
            futureTask.FutureTaskId = FutureTaskIndex++;
            futureTask.Condition = condition;
            futureTask.available = true;
            futureTask.completed = completed;
            futureTask.Description = description;
            FutureTaskMonitor.Instance.AddFutureTask(futureTask);
            return futureTask;
        }
        /// <summary>
        /// 检测一个状态；
        /// </summary>
        /// <param name="condition">需要检测的状态</param>
        /// <param name="description">描述</param>
        /// <returns>FutureTask异步检测对象</returns>
        public static FutureTask Detection(Func<bool> condition, string description = null)
        {
            var futureTask = ReferencePool.Accquire<FutureTask>();
            futureTask.FutureTaskId = FutureTaskIndex++;
            futureTask.Condition = condition;
            futureTask.Description = description;
            futureTask.available = true;
            FutureTaskMonitor.Instance.AddFutureTask(futureTask);
            return futureTask;
        }
        public static bool HasFutureTask(int futureTaskId)
        {
            return FutureTaskMonitor.Instance.HasFutureTask(futureTaskId);
        }
        public static FutureTaskInfo GetFutureTaskInfo(int futureTaskId)
        {
            return FutureTaskMonitor.Instance.GetFutureTaskInfo(futureTaskId);
        }
        internal void OnCompleted()
        {
            completed?.Invoke(this);
        }
        internal void OnRefresh(float realDeltatime)
        {
            if (!available)
                return;
            ElapsedTime += realDeltatime;
            polling?.Invoke(this);
            if (Condition.Invoke())
            {
                available = false;
            }
        }
    }
}
