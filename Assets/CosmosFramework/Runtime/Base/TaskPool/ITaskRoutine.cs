using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 任务执行程序；
    /// </summary>
    /// <typeparam name="T">任务的类型</typeparam>
    public interface ITaskRoutine<T>
        where T :TaskBase
    {
        void Initialize();
        T Task { get; }
        TaskStartStatus Start(T task);
        void Refresh(float elapseSeconds, float realElapseSeconds);
        void Shutdown();
        void Reset();
    }
}
