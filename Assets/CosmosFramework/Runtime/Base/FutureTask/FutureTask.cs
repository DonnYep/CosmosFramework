using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 异步对象的wrapper；
    /// 常用异步有.NET提供的Task，也有unity提供的Coroutine；
    /// 此结构体是作为Wrapper将异步对象进行地址缓存；
    /// </summary>
    /// <see cref="Task">Return vaalue</see>
    /// <see cref="UnityEngine. Coroutine">Return vaalue</see>
    public class FutureTask : IReference
    {
        static int FutureTaskIndex = 0;
         Action completed;
        Action polling;
        bool isPause;
        public bool IsPause { get { return isPause; } }
        public int FutureTaskId { get; private set; }
        public string Description { get; set; }
        public float ElapseTime { get; private set; }
        public Func<bool> Condition{ get; private set; }
        public event Action Completed { add { completed += value; } remove { completed -= value; } }
        public event Action Polling{ add { polling+= value; } remove { polling -= value; } }
        public FutureTask(){}
        public FutureTask(Func<bool> condition)
        {
            FutureTaskId = FutureTaskIndex++;
            Condition = condition;
            FutureTaskMonitor.Instance.AddFutureTask(this);
        }
        public void Release()
        {
            Condition = null;
            FutureTaskId = 0;
            Description = string.Empty;
            ElapseTime = 0;
            isPause = false;
        }
        public void Abort()
        {
            isPause = true;
        }
        public static FutureTask Create(Func<bool>condition)
        {
            var futureTask = ReferencePool.Accquire<FutureTask>();
            futureTask.FutureTaskId = FutureTaskIndex++;
            futureTask.Condition = condition;
            FutureTaskMonitor.Instance.AddFutureTask(futureTask);
            return futureTask;
        }
        internal void OnComplete()
        {
            completed?.Invoke();
        }
        internal void OnRefresh(float realDeltatime)
        {
            if (isPause)
                return;
            polling?.Invoke();
            ElapseTime += realDeltatime;
            if(Condition.Invoke())
            {
                isPause = true;
            }
        }
    }
}
