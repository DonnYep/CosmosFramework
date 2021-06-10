using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class FutureTaskMonitor : Singleton<FutureTaskMonitor>
    {
        DateTime previousTimeSinceStartup;
        Dictionary<int, FutureTask> futureTaskDict = new Dictionary<int, FutureTask>();
        List<FutureTask> futureTaskCache = new List<FutureTask>();
        bool isPause;
        public bool IsPause { get { return isPause; } set { isPause = value; } }
        public void Initiate()
        {
            GameManager.RefreshHandler += OnRefresh;
            isPause = false;
        }
        public void Termination()
        {
            isPause = true;
            try
            {
                GameManager.RefreshHandler -= OnRefresh;
            }
            catch { }
        }
        public bool HasFutureTask(int futureTaskId)
        {
            return futureTaskDict.ContainsKey(futureTaskId);
        }
        public FutureTaskInfo GetFutureTaskInfo(int futureTaskId)
        {
            if( futureTaskDict.TryGetValue(futureTaskId, out var futureTask))
            {
                FutureTaskInfo info = new FutureTaskInfo();
                info.ElapseTime = futureTask.ElapseTime;
                info.FutureTaskId= futureTask.FutureTaskId;
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
            if (futureTaskDict.Remove(futureTaskId,out var futureTask))
            {
                ReferencePool.Release(futureTask);
                return true;
            }
            else
                return false;

        }
        internal void OnRefresh()
        {
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
                if (futureTask.IsPause)
                {
                    futureTask.OnComplete();
                    RemoveFutureTask(futureTask.FutureTaskId);
                }
            }
        }
    }
}
