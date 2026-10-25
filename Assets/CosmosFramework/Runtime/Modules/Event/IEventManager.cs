using System;
namespace Cosmos.Event
{
    //================================================
    /*
     * 1、事件中心（reactive）。使用数据类型作为事件的 key。
     * 2、订阅返回 IDisposable，退订即释放；Observe<T>() 可接 Where / Select / DistinctUntilChanged 等操作符。
     * 3、线程约定：主线程派发。
     */
    //================================================
    public interface IEventManager : IModuleManager
    {
        /// <summary>
        /// 已有订阅者的事件类型数量。
        /// </summary>
        int EventCount { get; }
        /// <summary>
        /// 订阅事件；返回的句柄释放即退订。
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="handler">处理函数</param>
        /// <returns>退订句柄</returns>
        IDisposable Subscribe<T>(Action<T> handler) where T : GameEventArgs;
        /// <summary>
        /// 发布事件。
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        /// <param name="args">事件参数</param>
        void Publish<T>(T args) where T : GameEventArgs;
        /// <summary>
        /// 获取事件流；可接操作符做过滤 / 映射 / 去重。
        /// </summary>
        /// <typeparam name="T">事件参数类型</typeparam>
        IObservable<T> Observe<T>() where T : GameEventArgs;
        /// <summary>
        /// 是否存在该事件类型的订阅者。
        /// </summary>
        bool HasEvent<T>() where T : GameEventArgs;
        /// <summary>
        /// 获取事件信息（订阅者数量等）。
        /// </summary>
        EventInfo GetEventInfo<T>() where T : GameEventArgs;
        /// <summary>
        /// 移除指定事件类型的全部订阅者。
        /// </summary>
        void RemoveAllEvents<T>() where T : GameEventArgs;
        /// <summary>
        /// 移除所有事件的订阅者。
        /// </summary>
        void RemoveAllEvents();
    }
}
