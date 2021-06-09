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
        public int RunningRoutineCount { get { return runningTasksLnk.Count; } }
        /// <summary>
        /// 等待中的任务数量；
        /// </summary>
        public int WaitingTaskCount { get { return waitingTasksLnk.Count; } }
        /// <summary>
        /// 任务程序的总数；
        /// </summary>
        public int RoutineAmount { get { return routinesCacheQueue.Count + runningTasksLnk.Count; } }
        /// <summary>
        /// 任务程序缓存数量；
        /// </summary>
        public int RoutineCacheCount { get { return routinesCacheQueue.Count; } }
        public TaskPool()
        {
            routinesCacheQueue = new Queue<ITaskRoutine<T>>();
            runningTasksLnk = new LinkedList<ITaskRoutine<T>>();
            waitingTasksLnk = new LinkedList<T>();
            paused = false;
        }
        public void AddRoutine(ITaskRoutine<T>routine)
        {
            if (routine == null)
                throw new ArgumentNullException("Routine is invalid ! ");
            routine.Initialize();
            routinesCacheQueue.Enqueue(routine);
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
        public TaskInfo[] GetTaskInfos(string tag)
        {
            var results = new List<TaskInfo>();
            GetTaskInfos(tag, results);
            return results.ToArray();
        }
        public TaskInfo[] GetAllTaskInfos()
        {
            int idx = 0;
            TaskInfo[] taskResults = new TaskInfo[runningTasksLnk.Count + waitingTasksLnk.Count];
            foreach (var routine in runningTasksLnk)
            {
                var runningTask = routine.Task;
                taskResults[idx++] = new TaskInfo(runningTask.TaskSerialId, runningTask.Tag, runningTask.CustomeData,
                        runningTask.Done ? TaskStatus.Done : TaskStatus.Running, runningTask.Description);
            }
            foreach (var waitingTask in waitingTasksLnk)
            {
                taskResults[idx++] = new TaskInfo(waitingTask.TaskSerialId, waitingTask.Tag, waitingTask.CustomeData,
                            TaskStatus.Prepare, waitingTask.Description);
            }
            return taskResults;
        }
        public void GetTaskInfos(string tag, IList<TaskInfo> results)
        {
            if (results == null)
                throw new ArgumentNullException("Results is invalid ! ");
            results.Clear();
            foreach (var routine in runningTasksLnk)
            {
                var runningTask = routine.Task;
                if (runningTask.Tag == tag)
                {
                    results.Add(new TaskInfo(runningTask.TaskSerialId, runningTask.Tag, runningTask.CustomeData,
                        runningTask.Done ? TaskStatus.Done : TaskStatus.Running, runningTask.Description));
                }
                foreach (var waitingTask in waitingTasksLnk)
                {
                    if (waitingTask.Tag == tag)
                    {
                        results.Add(new TaskInfo(waitingTask.TaskSerialId, waitingTask.Tag, waitingTask.CustomeData,
                            TaskStatus.Prepare, waitingTask.Description));
                    }
                }
            }
        }
        public bool RemoveTask(int taskSerialId)
        {
            foreach (var task in waitingTasksLnk)
            {
                if (task.TaskSerialId == taskSerialId)
                {
                    waitingTasksLnk.Remove(task);
                    ReferencePool.Release(task);
                    return true;
                }
            }
            foreach (var routine in runningTasksLnk)
            {
                if (routine.Task.TaskSerialId == taskSerialId)
                {
                    var task = routine.Task;
                    routine.Reset();
                    routinesCacheQueue.Enqueue(routine);
                    runningTasksLnk.Remove(routine);
                    ReferencePool.Release(task);
                    return true;
                }
            }
            return false;
        }
        public int RemoveTasks(string tag)
        {
            int count = 0;
            var currentWaitingTask = waitingTasksLnk.First;
            while (currentWaitingTask != null)
            {
                var next = currentWaitingTask.Next;
                var task = currentWaitingTask.Value;
                if (task.Tag == tag)
                {
                    waitingTasksLnk.Remove(currentWaitingTask);
                    ReferencePool.Release(task);
                    count++;
                }
                currentWaitingTask = next;
            }
            var currentRunningRoutine = runningTasksLnk.First;
            while (currentRunningRoutine != null)
            {
                var next = currentRunningRoutine.Next;
                var runningRoutine = currentRunningRoutine.Value;
                var task = runningRoutine.Task;
                if (task.Tag == tag)
                {
                    runningRoutine.Reset();
                    routinesCacheQueue.Enqueue(runningRoutine);
                    ReferencePool.Release(task);
                    count++;
                }
                currentRunningRoutine = next;
            }
            return count;
        }
        public int RemoveAllTask()
        {
            var count = runningTasksLnk.Count + waitingTasksLnk.Count;
            foreach (var task in waitingTasksLnk)
            {
                ReferencePool.Release(task);
            }
            waitingTasksLnk.Clear();
            foreach (var routine in runningTasksLnk)
            {
                var task = routine.Task;
                routine.Reset();
                routinesCacheQueue.Enqueue(routine);
                ReferencePool.Release(task);
            }
            return count;
        }
        public void Shutdown()
        {
            RemoveAllTasks();
            while (routinesCacheQueue.Count > 0)
            {
                routinesCacheQueue.Dequeue().Shutdown();
            }
        }
        public int RemoveAllTasks()
        {
            var taskCount = waitingTasksLnk.Count + runningTasksLnk.Count;
            foreach (var task in waitingTasksLnk)
            {
                ReferencePool.Release(task);
            }
            waitingTasksLnk.Clear();
            foreach (var runningTask in runningTasksLnk)
            {
                var task = runningTask.Task;
                runningTask.Reset();
                routinesCacheQueue.Enqueue(runningTask);
                ReferencePool.Release(task);
            }
            runningTasksLnk.Clear();
            return taskCount;
        }
        public void OnRefresh(float elapseSeconds, float realElapseSeconds)
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
