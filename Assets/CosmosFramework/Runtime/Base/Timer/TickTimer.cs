using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Cosmos
{
    /// <summary>
    /// 计时器，需要从外部调用轮询。
    /// 所有逻辑线程安全；
    /// </summary>
    public class TickTimer
    {
        class TickTask
        {
            /// <summary>
            /// 任务Id；
            /// </summary>
            public int TaskId;
            /// <summary>
            /// 间隙时间；
            /// </summary>
            public int IntervalTime;
            /// <summary>
            /// 循环执行次数；
            /// </summary>
            public int LoopCount;
            public double DestTime;
            public Action<int> TaskCallback;
            public Action<int> CancelCallback;
            public double StartTime;
            public int LoopIndex;
            public bool IsPause;
            public double PauseRemainTime;
            public TickTask(int taskId, int loopCount, int intervalTime, double destTime, Action<int> taskCallback, Action<int> cancelCallback, double startTime)
            {
                this.TaskId = taskId;
                this.LoopCount = loopCount;
                this.IntervalTime = intervalTime;
                this.DestTime = destTime;
                this.TaskCallback = taskCallback;
                this.CancelCallback = cancelCallback;
                this.StartTime = startTime;
                this.IsPause = false;
                this.PauseRemainTime = 0;
            }
            public void Dispose()
            {
                this.TaskId = 0;
                this.LoopCount = 0;
                this.IntervalTime = 0;
                this.DestTime = 0;
                this.TaskCallback = null;
                this.CancelCallback = null;
                this.StartTime = 0;
                this.LoopIndex = 0;
                this.IsPause = false;
                this.PauseRemainTime = 0;
            }
        }
        public Action<string> LogInfo { get; set; }
        public Action<string> LogWarn { get; set; }
        public Action<string> LogError { get; set; }
        readonly DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0);
        readonly ConcurrentDictionary<int, TickTask> taskDict;
        int taskIndex = 0;
        public int TaskCount { get { return taskDict.Count; } }
        Queue<TickTask> taskQueue;
        bool usePool;
        public bool UsePool { get { return usePool; } }
        /// <summary>
        /// 计时器构造函数；
        /// </summary>
        /// <param name="usePool">是否使用池缓对task进行缓存。任务量大时，建议为true</param>
        public TickTimer(bool usePool = false)
        {
            taskDict = new ConcurrentDictionary<int, TickTask>();
            this.usePool = usePool;
            if (usePool)
                taskQueue = new Queue<TickTask>();
        }
        /// <summary>
        /// 添加任务；
        /// 若任务添加成功，则返回大于0的TaskId；
        /// 若任务添加失败，则返回-1；
        /// </summary>
        /// <param name="intervalTime">毫秒级别时间间隔</param>
        /// <param name="taskCallback">执行回调</param>
        /// <param name="cancelCallback">任务取消回调</param>
        /// <param name="loopCount">执行次数</param>
        /// <returns>添加事件成功后返回的ID</returns>
        public int AddTask(int intervalTime, Action<int> taskCallback, Action<int> cancelCallback, int loopCount = 1)
        {
            return AddTask(intervalTime, 0, taskCallback, cancelCallback, loopCount);
        }
        /// <summary>
        /// 添加任务；
        /// 若任务添加成功，则返回大于0的TaskId；
        /// 若任务添加失败，则返回-1；
        /// </summary>
        /// <param name="intervalTime">毫秒级别时间间隔</param>
        /// <param name="delayTime">毫秒级别时间延迟</param>
        /// <param name="taskCallback">执行回调</param>
        /// <param name="cancelCallback">任务取消回调</param>
        /// <param name="loopCount">执行次数</param>
        /// <returns>添加事件成功后返回的ID</returns>
        public int AddTask(int intervalTime, int delayTime, Action<int> taskCallback, Action<int> cancelCallback, int loopCount = 1)
        {
            int tid = GenerateTaskId();
            double startTime = GetUTCMilliseconds() + delayTime;
            double destTime = startTime + intervalTime;
            TickTask task = null;
            if (usePool)
                task = AcquireTickTask(tid, loopCount, intervalTime, destTime, taskCallback, cancelCallback, startTime);
            else
                task = new TickTask(tid, loopCount, intervalTime, destTime, taskCallback, cancelCallback, startTime);
            if (!taskDict.TryAdd(tid, task))
                return -1;
            return tid;
        }
        /// <summary>
        /// 移除任务；
        /// </summary>
        /// <param name="taskId">任务Id</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveTask(int taskId)
        {
            if (!taskDict.TryRemove(taskId, out TickTask task))
                return false;
            task.CancelCallback?.Invoke(taskId);
            if (usePool)
                ReleaseTickTask(task);
            return true;
        }
        public bool PauseTask(int taskId)
        {
            if (!taskDict.TryGetValue(taskId, out TickTask task))
                return false;
            task.IsPause = true;
            var remainTime = task.DestTime - GetUTCMilliseconds();
            task.PauseRemainTime = remainTime > 0 ? remainTime : 0;
            return true;
        }
        public bool UnPauseTask(int taskId)
        {
            if (!taskDict.TryGetValue(taskId, out TickTask task))
                return false;
            task.IsPause = false;
            task.DestTime = task.PauseRemainTime + GetUTCMilliseconds();
            return true;
        }
        public bool HasTask(int taskId)
        {
            return taskDict.ContainsKey(taskId);
        }
        /// <summary>
        /// 轮询；
        /// </summary>
        public void TickRefresh()
        {
            try
            {
                double nowTime = GetUTCMilliseconds();
                foreach (var task in taskDict.Values)
                {
                    if (task.IsPause)
                        continue;
                    if (nowTime < task.DestTime)
                        continue;
                    ++task.LoopIndex;
                    //循环次数++，若循环idx比循环总数小，则进入下次循环；
                    if (task.LoopIndex < task.LoopCount)
                    {
                        task.DestTime = nowTime + task.IntervalTime;
                        task.TaskCallback.Invoke(task.TaskId);
                    }
                    else
                    {
                        //若循环idx比循环总数大或等于，则终止循环，并移除任务；
                        //线程安全字典，遍历过程中删除无影响。
                        if (taskDict.TryRemove(task.TaskId, out _))
                        {
                            task.TaskCallback.Invoke(task.TaskId);
                            task.CancelCallback = null;
                            if (usePool)
                                ReleaseTickTask(task);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogError(e.ToString());
            }
        }
        /// <summary>
        /// 重置计时器；
        /// </summary>
        public void Reset()
        {
            taskDict.Clear();
            if (usePool)
                taskQueue.Clear();
        }
        int GenerateTaskId()
        {
            ++taskIndex;
            while (taskDict.ContainsKey(taskIndex))
            {
                ++taskIndex;
                if (taskIndex == int.MaxValue)
                    taskIndex = 0;
            }
            return taskIndex;
        }
        /// <summary>
        /// 获取毫秒级别时间；
        /// </summary>
        double GetUTCMilliseconds()
        {
            TimeSpan ts = DateTime.UtcNow - startDateTime;
            return ts.TotalMilliseconds;
        }
        TickTask AcquireTickTask(int taskId, int loopCount, int intervalTime, double destTime, Action<int> taskCallback, Action<int> cancelCallback, double startTime)
        {
            TickTask task = null;
            if (taskQueue.Count > 0)
            {
                task = taskQueue.Dequeue();
                task.TaskId = taskId;
                task.LoopCount = loopCount;
                task.IntervalTime = intervalTime;
                task.DestTime = destTime;
                task.TaskCallback = taskCallback;
                task.CancelCallback = cancelCallback;
                task.StartTime = startTime;
                task.IsPause = false;
                task.PauseRemainTime = 0;
            }
            else
                task = new TickTask(taskId, loopCount, intervalTime, destTime, taskCallback, cancelCallback, startTime);
            return task;
        }
        void ReleaseTickTask(TickTask task)
        {
            task.Dispose();
            taskQueue.Enqueue(task);
        }
    }
}
