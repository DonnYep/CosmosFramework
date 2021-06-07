using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 任务池，兼容协程与多线程异步；
    /// </summary>
    /// <see cref="TaskBase">
    /// <typeparam name="T">池中任务的类型，须为TaskBase的派生类</typeparam>
    public class TaskPool<T> where T : TaskBase
    {
        bool paused;
        readonly Queue<ITaskRoutine<T>> routinesCacheQueue;
        readonly LinkedList<ITaskRoutine<T>> runningTasksLnk;
        readonly LinkedList<T> waitingTasksLnk;

        public bool Paused { get { return paused; } set { paused = value; } }
        /// <summary>
        /// 执行中的任务数量；
        /// </summary>
        public int RunningTaskCount { get { return runningTasksLnk.Count; } }
        /// <summary>
        /// 等待中的任务数量；
        /// </summary>
        public int WaitingTaskCount { get { return waitingTasksLnk.Count; } }
        public TaskPool()
        {
            routinesCacheQueue = new Queue<ITaskRoutine<T>>();
            runningTasksLnk = new LinkedList<ITaskRoutine<T>>();
            waitingTasksLnk = new LinkedList<T>();
        }
        public TaskInfo GetTaskInfo(int taskSerialId)
        {
            foreach (var task in runningTasksLnk)
            {
                var tempTask = task.Task;
                if (task.Task.TaskSerialId == taskSerialId)
                {
                    return new TaskInfo(tempTask.TaskSerialId, tempTask.Tag, tempTask.CustomeData, TaskStatus.Running, tempTask.Description);
                }
            }
            foreach (var task in waitingTasksLnk)
            {
                if (task.TaskSerialId == taskSerialId)
                {
                    return new TaskInfo(task.TaskSerialId, task.Tag, task.CustomeData, TaskStatus.Running, task.Description);
                }
            }
            return default(TaskInfo);
        }

        public void Refresh(float elapseSeconds, float realElapseSeconds)
        {
            if (paused)
                return;
            RefreshRunningTasks(elapseSeconds, realElapseSeconds);
            RefreshWaitingTasks(elapseSeconds, realElapseSeconds);
        }
        void RefreshRunningTasks(float elapseSeconds, float realElapseSeconds)
        {
            var current = runningTasksLnk.First;
            while (current != null)
            {
                var task = current.Value.Task;
                if (!task.Done)
                {
                    current.Value.Refresh(elapseSeconds, realElapseSeconds);
                    current = current.Next;
                    continue;
                }
                var next = current.Next;
                current.Value.Reset();
                routinesCacheQueue.Enqueue(current.Value);
                runningTasksLnk.Remove(current);
                ReferencePool.Release(task);
                current = next;
            }
        }
        void RefreshWaitingTasks(float elapseSeconds, float realElapseSeconds)
        {
            var current = waitingTasksLnk.First;
            while (current != null && routinesCacheQueue.Count > 0)
            {
                var routine = routinesCacheQueue.Dequeue();
                var lnkNode = runningTasksLnk.AddLast(routine);
                var task = current.Value;
                var next = current.Next;
                TaskStartStatus status = routine.Start(task);

                if (status == TaskStartStatus.Done || status == TaskStartStatus.HasToWait || status == TaskStartStatus.UnknownError)
                {
                    routine.Reset();
                    routinesCacheQueue.Enqueue(routine);
                    runningTasksLnk.Remove(lnkNode);
                }
                if (status == TaskStartStatus.Done || status == TaskStartStatus.CanResume || status == TaskStartStatus.UnknownError)
                {
                    waitingTasksLnk.Remove(current);
                }
                if (status == TaskStartStatus.Done || status == TaskStartStatus.UnknownError)
                {
                    ReferencePool.Release(task);
                }
                current = next;
            }
        }
    }
}
