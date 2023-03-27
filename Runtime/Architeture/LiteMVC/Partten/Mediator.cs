using System;

namespace LiteMVC
{
    /// <summary>
    /// View代理类
    /// </summary>
    public abstract class Mediator
    {
        /// <summary>
        /// 绑定数据；
        /// 注意：使用匿名函数绑定，会导致解绑时地址寻址失败，尽量使用明确的函数进行绑定！
        /// </summary>
        /// <typeparam name="KValue">数据类型</typeparam>
        /// <param name="action">绑定的函数</param>
        public void Bind<KValue>(Action<KValue> action)
        {
            MVC.Bind(action);
        }
        /// <summary>
        /// 绑定数据；
        /// 注意：使用匿名函数绑定，会导致解绑时地址寻址失败，尽量使用明确的函数进行绑定！
        /// </summary>
        /// <typeparam name="KValue">数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <param name="action">绑定的函数</param>
        public void Bind<KValue>(string eventName, Action<KValue> action)
        {
            MVC.Bind(eventName, action);
        }
        /// <summary>
        /// 解绑数据；
        /// </summary>
        /// <typeparam name="KValue">数据类型</typeparam>
        /// <param name="action">解绑的函数</param>
        public void Unbind<KValue>(Action<KValue> action)
        {
            MVC.Unbind(action);
        }
        /// <summary>
        /// 解绑数据；
        /// </summary>
        /// <typeparam name="KValue">数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <param name="action">解绑的函数</param>
        public void Unbind<KValue>(string eventName, Action<KValue> action)
        {
            MVC.Unbind(eventName, action);
        }
        /// <summary>
        /// 执行&触发；
        /// </summary>
        /// <typeparam name="KValue">数据类型</typeparam>
        /// <param name="data">具体数据</param>
        public void Execute<KValue>(KValue data)
        {
            MVC.Dispatch(string.Empty, data);
        }
        /// <summary>
        /// 执行&触发；
        /// </summary>
        /// <typeparam name="KValue">数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <param name="data">具体数据</param>
        public void Execute<KValue>(string eventName, KValue data)
        {
            MVC.Dispatch(eventName, data);
        }
        /// <summary>
        /// 数据类型是否存在绑定；
        /// </summary>
        /// <typeparam name="KValue">数据类型</typeparam>
        /// <returns>是否存在的结果</returns>
        public bool HasBind<KValue>()
        {
            return MVC.HasBind<KValue>();
        }
        /// <summary>
        /// 数据类型是否存在绑定；
        /// </summary>
        /// <typeparam name="KValue">数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <returns>是否存在的结果</returns>
        public bool HasBind<KValue>(string eventName)
        {
            return MVC.HasBind<KValue>(eventName);
        }
        public virtual void OnRegister() { }
        public virtual void OnDeregister() { }
    }
}
