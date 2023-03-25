using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteMVC.Core
{
    public class View
    {
        readonly Dictionary<Type, Mediator> mediatorDict = new Dictionary<Type, Mediator>();
        readonly object locker = new object();
        public int MediatorCount { get { return mediatorDict.Count; } }
        /// <summary>
        /// 注册mediator
        /// </summary>
        /// <typeparam name="T">mediator类型</typeparam>
        /// <param name="mediator">对象</param>
        public void RegisterMediator<T>(T mediator)
    where T : Mediator
        {
            var type = typeof(T);
            if (!mediatorDict.ContainsKey(type))
            {
                mediatorDict.Add(type, mediator);
                mediator.OnRegister();
            }
        }
        /// <summary>
        /// 注销mediator
        /// </summary>
        /// <typeparam name="T">mediator类型</typeparam>
        public void DeregisterMediator<T>()
            where T : Mediator
        {
            var type = typeof(T);
            lock (locker)
            {
                if (mediatorDict.ContainsKey(type))
                {
                    var view = mediatorDict[type];
                    mediatorDict.Remove(type);
                    view.OnDeregister();
                }
            }
        }
        /// <summary>
        /// 是否存在mediator类型
        /// </summary>
        /// <typeparam name="T">mediator类型</typeparam>
        /// <returns>存在结果</returns>
        public bool HasMediator<T>()
            where T : Mediator
        {
            var type = typeof(T);
            lock (locker)
            {
                return mediatorDict.ContainsKey(type);
            }
        }
        /// <summary>
        /// 获取一个mediator
        /// </summary>
        /// <typeparam name="T">mediator类型</typeparam>
        /// <param name="mediator">对象</param>
        /// <returns>返回结果</returns>
        public bool PeekMediator<T>(out T mediator)
            where T : Mediator
        {
            var type = typeof(T);
            mediator = default(T);
            lock (locker)
            {
                var rst = mediatorDict.TryGetValue(type, out var m);
                if (rst)
                    mediator = (T)m;
                return rst;
            }
        }
        /// <summary>
        /// 注销所有mediator
        /// </summary>
        public void DeregisterAllMediator()
        {
            var views = mediatorDict.Values.ToList();
            mediatorDict.Clear();
            foreach (var view in views)
            {
                view.OnDeregister();
            }
        }
    }
}