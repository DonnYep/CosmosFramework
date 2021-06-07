using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos
{
    /// <summary>
    /// 协程队列 TODO未完成，须包含完整的协程生命周期；
    /// </summary>
    class CoroutineQueue
    {
        readonly uint maxActive;
        readonly Func<IEnumerator, Coroutine> coroutineStarter;
        readonly Queue<IEnumerator> coroutineQueue;
        uint numActive;
        public uint MaxActive { get { return maxActive; } }
        public uint NumActive { get { return numActive; } }
        public int RemainCoroutineCount { get { return coroutineQueue.Count; } }
        public CoroutineQueue(uint maxActive, Func<IEnumerator, Coroutine> coroutineStarter)
        {
            if (maxActive == 0)
                throw new ArgumentException("Must be at least one", "maxActive");
            if(coroutineStarter==null)
                throw new ArgumentNullException("coroutineStarter is invalid ! ", "coroutineStarter");
            this.maxActive = maxActive;
            this.coroutineStarter = coroutineStarter;
            coroutineQueue = new Queue<IEnumerator>();
        }
        public void Run(IEnumerator coroutine)
        {
            if (numActive < maxActive)
            {
                var runner = CoroutineRunner(coroutine);
                coroutineStarter(runner);
            }
            else
            {
                coroutineQueue.Enqueue(coroutine);
            }
        }
        IEnumerator CoroutineRunner(IEnumerator coroutine)
        {
            numActive++;
            while (coroutine.MoveNext())
            {
                yield return coroutine.Current;
            }
            numActive--;
            if (coroutineQueue.Count > 0)
            {
                var next = coroutineQueue.Dequeue();
                Run(next);
            }
        }
    }
}
