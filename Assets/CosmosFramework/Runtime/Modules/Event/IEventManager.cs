using System;
namespace Cosmos.Event
{
    public interface IEventManager : IModuleManager
    {
        /// <summary>
        /// 当前注册的事件类型总数。
        /// </summary>
        int EventCount { get; }
        /// <summary>
        /// 添加事件。
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        /// <param name="handler">事件监听函数</param>
        void AddEvent<T>(Action<T> handler) where T : GameEventArgs;
        /// <summary>
        /// 移除事件。假如此类型的事件监听数量为零，自动移除此事件。
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        /// <param name="handler">事件监听函数</param>
        void RemoveEvent<T>(Action<T> handler) where T : GameEventArgs;
        /// <summary>
        /// 移除一个类型的所有事件。
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        void RemoveAllEvents<T>() where T : GameEventArgs;
        /// <summary>
        /// 清空所有的事件监听。
        /// </summary>
        void RemoveAllEvents();
        /// <summary>
        /// 事件分发。
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        /// <param name="args">事件数据</param>
        void Dispatch<T>(T args) where T : GameEventArgs;
        /// <summary>
        /// 判断事件类似是否存在。
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        bool HasEvent<T>() where T : GameEventArgs;
        /// <summary>
        /// 获取事件的信息。
        /// </summary>
        /// <typeparam name="T">事件数据类型</typeparam>
        EventInfo GetEventInfo<T>() where T : GameEventArgs;
    }
}
