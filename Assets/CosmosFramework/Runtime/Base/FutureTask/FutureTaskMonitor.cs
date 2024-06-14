using System;
using System.Collections.Generic;
namespace Cosmos
{
    internal class FutureTaskMonitor : MonoSingleton<FutureTaskMonitor>
    {
        DateTime previousTimeSinceStartup;
        Dictionary<int, FutureTask> futureTaskDict = new Dictionary<int, FutureTask>();
        List<FutureTask> futureTaskCache = new List<FutureTask>();
        bool isPause = false;
        public bool IsPause { get { return isPause; } set { isPause = value; } }
        public void Termination()
        {
            isPause = true;
        }
        internal bool HasFutureTask(int futureTaskId)
        {
            return futureTaskDict.ContainsKey(futureTaskId);
        }
        internal FutureTaskInfo GetFutureTaskInfo(int futureTaskId)
        {
            if (futureTaskDict.TryGetValue(futureTaskId, out var futureTask))
            {
                FutureTaskInfo info = new FutureTaskInfo(futureTask.FutureTaskId, futureTask.ElapsedTime, futureTask.Description);
                return info;
            }
            return FutureTaskInfo.None;
        }
        internal void AddFutureTask(FutureTask futureTask)
        {
            futureTaskDict.TryAdd(futureTask.FutureTaskId, futureTask);
        }
        internal bool RemoveFutureTask(int futureTaskId)
        {
            if (futureTaskDict.Remove(futureTaskId, out var futureTask))
            {
                futureTask.Release();
                return true;
            }
            else
                return false;

        }
        void Awake()
        {
            gameObject.hideFlags = UnityEngine.HideFlags.HideInHierarchy;
            previousTimeSinceStartup = DateTime.Now;
        }
        private void Update()
        {
            if (isPause)
                return;
            float deltaTime = (float)(DateTime.Now.Subtract(previousTimeSinceStartup).TotalMilliseconds / 1000.0f);
            previousTimeSinceStartup = DateTime.Now;
            if (futureTaskDict.Count == 0)
            {
                return;
            }
            futureTaskCache.Clear();
            foreach (var ft in futureTaskDict)
                futureTaskCache.Add(ft.Value);
            var length = futureTaskCache.Count;
            for (int i = 0; i < length; i++)
            {
                var futureTask = futureTaskCache[i];
                futureTask.OnRefresh(deltaTime);
                if (!futureTask.Available)
                {
                    futureTask.OnCompleted();
                    RemoveFutureTask(futureTask.FutureTaskId);
                }
            }
        }
    }
}
