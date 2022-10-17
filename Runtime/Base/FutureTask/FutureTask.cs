using System;
using System.Runtime.CompilerServices;

namespace Cosmos
{
    //================================================
    /*
     * 1、FutureTask是异步的监听器，支持async/await语法。
     * 
     * 2、condition是FutureTask需要检测的条件。当返回值为true时，FutureTask
    * 表示这个异步完成了条件。
    * 
    *3、FutureTask带有onPolling与onCompleted回调。异步可通过这两个回调进行
    *状态检测；
    */
    //================================================
    /// <summary>
    ///异步监听器；
    /// </summary>
    public class FutureTask : INotifyCompletion
    {
        static int FutureTaskIndex = 0;
        Action<FutureTask> onCompleted;
        Action<FutureTask> onPolling;
        bool available;
        #region AwaiterProperties
        bool isCompleted;
        Action continuation;
        #endregion
        public bool IsCompleted
        {
            get
            { return isCompleted; }
            set
            {
                isCompleted = value;
                if (isCompleted)
                {
                    continuation?.Invoke();
                }
            }
        }
        public bool Available { get { return available; } }
        public int FutureTaskId { get; private set; }
        public string Description { get; private set; }
        public float ElapsedTime { get; private set; }
        public Func<bool> Condition { get; private set; }
        public FutureTask(Func<bool> condition, Action<FutureTask> polling, Action<FutureTask> completed, string description = null)
        {
            FutureTaskId = FutureTaskIndex++;
            Condition = condition;
            available = true;
            this.onPolling = polling;
            this.onCompleted = completed;
            Description = description;
            FutureTaskMonitor.Instance.AddFutureTask(this);
        }
        public FutureTask(Func<bool> condition, Action<FutureTask> completed, string description = null)
        {
            FutureTaskId = FutureTaskIndex++;
            Condition = condition;
            available = true;
            this.onCompleted = completed;
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
        #region AwaiterMethods
        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }
        public FutureTask GetAwaiter() { return this; }
        public void GetResult() { }
        #endregion
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
            onCompleted?.Invoke(this);
            IsCompleted = true;
        }
        internal void OnRefresh(float realDeltatime)
        {
            if (!available)
                return;
            ElapsedTime += realDeltatime;
            onPolling?.Invoke(this);
            if (Condition.Invoke())
            {
                available = false;
            }
        }
    }
}
