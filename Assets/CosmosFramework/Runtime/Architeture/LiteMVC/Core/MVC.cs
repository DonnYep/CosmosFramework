using System;
using LiteMVC.Core;
namespace LiteMVC
{
    public partial class MVC
    {
        readonly static Controller controller = new Controller();
        readonly static View view = new View();
        readonly static Model model = new Model();
        readonly static Notification notification = new Notification();
        public static int ModelCount { get { return model.ProxyCount; } }
        public static int ViewCount { get { return view.MediatorCount; } }
        public static int BindTypeCount { get { return notification.BindTypeCount; } }
        public static int TypeBindCount { get { return controller.TypeBindCount; } }

        #region Command
        /// <summary>
        /// 注册command
        /// </summary>
        /// <typeparam name="T">订阅的数据类型</typeparam>
        /// <typeparam name="K">command类型</typeparam>
        public static void BindCommand<T, K>()
        {
            BindCommand<T>(string.Empty, typeof(K));
        }
        /// <summary>
        /// 注册command
        /// </summary>
        /// <typeparam name="T">订阅的数据类型</typeparam>
        /// <typeparam name="K">command类型</typeparam>
        /// <param name="eventName">事件名</param>
        public static void BindCommand<T, K>(string eventName)
            where K : Command<T>
        {
            BindCommand<T>(eventName, typeof(K));
        }
        /// <summary>
        /// 绑定command
        /// </summary>
        /// <typeparam name="T">订阅的数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <param name="cmdType">command类型</param>
        public static void BindCommand<T>(string eventName, Type cmdType)
        {
            controller.BindCommand<T>(eventName, cmdType);
        }
        /// <summary>
        ///注销对指定类型的数据的指定cmd绑定
        /// </summary>
        /// <typeparam name="T">订阅的数据类型</typeparam>
        /// <typeparam name="K">cmd类型</typeparam>
        public static void UnbindCommand<T, K>()
            where K : Command<T>
        {
            UnbindCommand(typeof(T), string.Empty, typeof(K));
        }
        /// <summary>
        ///注销对指定类型的数据的指定cmd绑定
        /// </summary>
        /// <typeparam name="T">订阅的数据类型</typeparam>
        /// <typeparam name="K">cmd类型</typeparam>
        /// <param name="eventName">事件名</param>
        public static void UnbindCommand<T, K>(string eventName)
            where K : Command<T>
        {
            UnbindCommand(typeof(T), eventName, typeof(K));
        }
        /// <summary>
        ///注销对指定类型的数据的指定cmd绑定
        /// </summary>
        /// <typeparam name="T">订阅的数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <param name="cmdType">cmd类型</param>
        public static void UnbindCommand<T>(string eventName, Type cmdType)
        {
            UnbindCommand(typeof(T), eventName, cmdType);
        }
        /// <summary>
        ///注销对指定类型的数据的指定cmd绑定
        /// </summary>
        /// <param name="dataType">订阅的数据类型</param>
        /// <param name="eventName">事件名</param>
        /// <param name="cmdType">cmd类型</param>
        public static void UnbindCommand(Type dataType, string eventName, Type cmdType)
        {
            controller.UnbindCommand(dataType, eventName, cmdType);
        }
        /// <summary>
        /// 注销订阅的数据类型
        /// </summary>
        /// <typeparam name="T">订阅的数据类型</typeparam>
        public static void UnbindCommands<T>()
        {
            UnbindCommands(typeof(T), string.Empty);
        }
        /// <summary>
        /// 注销订阅的数据类型
        /// </summary>
        /// <typeparam name="T">订阅的数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        public static void UnbindCommands<T>(string eventName)
        {
            UnbindCommands(typeof(T), eventName);
        }
        /// <summary>
        ///注销订阅的数据类型
        /// </summary>
        /// <param name="dataType">订阅的数据类型</param>
        /// <param name="eventName">事件名</param>
        public static void UnbindCommands(Type dataType, string eventName)
        {
            controller.UnbindCommands(dataType, eventName);
        }
        /// <summary>
        /// 是否注册了订阅的数据类型
        /// </summary>
        /// <param name="dataType">订阅的数据类型</param>
        /// <param name="cmdType">command类型</param>
        /// <returns>是否订阅</returns>
        public static bool HasCommandBind(Type dataType, Type cmdType)
        {
            return HasCommandBind(dataType, string.Empty, cmdType);
        }
        /// <summary>
        /// 是否注册了订阅的数据类型
        /// </summary>
        /// <param name="dataType">订阅的数据类型</param>
        /// <param name="eventName">事件名</param>
        /// <param name="cmdType">command类型</param>
        /// <returns>是否订阅</returns>
        public static bool HasCommandBind(Type dataType, string eventName, Type cmdType)
        {
            return controller.HasCommandBind(dataType, eventName, cmdType);
        }
        /// <summary>
        /// 是否存在对指定数据类型的绑定
        /// </summary>
        /// <param name="dataType">订阅的数据类型</param>
        /// <returns>绑定结果</returns>
        public static bool HasBind(Type dataType)
        {
            return controller.HasBind(dataType, string.Empty);
        }
        /// <summary>
        /// 是否存在对指定数据类型的绑定
        /// </summary>
        /// <param name="dataType">订阅的数据类型</param>
        /// <param name="eventName">事件名</param>
        /// <returns>绑定结果</returns>
        public static bool HasBind(Type dataType, string eventName)
        {
            return controller.HasBind(dataType, eventName);
        }
        /// <summary>
        /// 派发消息
        /// </summary>
        /// <param name="data">数据</param>
        public static void Dispatch(object data)
        {
            Dispatch(string.Empty, data);
        }
        /// <summary>
        /// 派发消息
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="data">数据</param>
        public static void Dispatch(string eventName, object data)
        {
            controller.Dispatch(eventName, data);
            notification.Dispatch(eventName, data);
        }

        #endregion

        #region View
        /// <summary>
        /// 注册mediator
        /// </summary>
        /// <typeparam name="T">mediator类型</typeparam>
        /// <param name="mediator">对象</param>
        public static void RegisterMediator<T>(T mediator)
where T : Mediator
        {
            view.RegisterMediator(mediator);
        }
        /// <summary>
        /// 注销mediator
        /// </summary>
        /// <typeparam name="T">mediator类型</typeparam>
        public static void DeregisterMediator<T>()
            where T : Mediator
        {
            view.DeregisterMediator<T>();
        }
        /// <summary>
        /// 是否存在mediator类型
        /// </summary>
        /// <typeparam name="T">mediator类型</typeparam>
        /// <returns>存在结果</returns>
        public static bool HasMediator<T>()
            where T : Mediator
        {
            return view.HasMediator<T>();
        }
        /// <summary>
        /// 获取一个mediator
        /// </summary>
        /// <typeparam name="T">mediator类型</typeparam>
        /// <param name="mediator">对象</param>
        /// <returns>返回结果</returns>
        public static bool PeekMediator<T>(out T mediator)
            where T : Mediator
        {
            return view.PeekMediator<T>(out mediator);
        }
        /// <summary>
        /// 注销所有mediator
        /// </summary>
        public static void DeregisterAllMediator()
        {
            view.DeregisterAllMediator();
        }
        #endregion

        #region Model
        /// <summary>
        /// 注册proxy；
        /// </summary>
        /// <typeparam name="T">proxy类型</typeparam>
        /// <param name="proxy">对象</param>
        public static void RegisterProxy<T>(T proxy)
            where T : Proxy
        {
            model.RegisterProxy(proxy);
        }
        /// <summary>
        /// 注销proxy；
        /// </summary>
        /// <typeparam name="T">proxy类型</typeparam>
        public static void DeregisterProxy<T>()
            where T : Proxy
        {
            model.DeregisterProxy<T>();
        }
        /// <summary>
        /// 是否存在proxy类型
        /// </summary>
        /// <typeparam name="T">proxy类型</typeparam>
        /// <returns>存在结果</returns>
        public static bool HasProxy<T>()
            where T : Proxy
        {
            return model.HasProxy<T>();
        }
        /// <summary>
        /// 获取一个proxy；
        /// </summary>
        /// <typeparam name="T">proxy类型</typeparam>
        /// <param name="proxy">对象</param>
        /// <returns>返回结果</returns>
        public static bool PeekProxy<T>(out T proxy)
            where T : Proxy
        {
            return model.PeekProxy<T>(out proxy);
        }
        /// <summary>
        /// 注销所有proxy；
        /// </summary>
        public static void DeregisterAllProxy()
        {
            model.DeregisterAllProxy();
        }
        #endregion

        #region Bind
        /// <summary>
        /// 数据类型是否存在绑定；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>是否存在的结果</returns>
        public static bool HasBind<T>()
        {
            return HasBind<T>(string.Empty);
        }
        /// <summary>
        /// 数据类型是否存在绑定；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <returns>是否存在的结果</returns>
        public static bool HasBind<T>(string eventName)
        {
            return notification.HasBind(typeof(T), eventName);
        }
        /// <summary>
        /// 绑定数据；
        /// 注意：使用匿名函数绑定，会导致解绑时地址寻址失败，尽量使用明确的函数进行绑定！
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="action">绑定的函数</param>
        public static void Bind<T>(Action<T> action)
        {
            Bind<T>(string.Empty, action);
        }
        /// <summary>
        /// 绑定数据；
        /// 注意：使用匿名函数绑定，会导致解绑时地址寻址失败，尽量使用明确的函数进行绑定！
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <param name="action">绑定的函数</param>
        public static void Bind<T>(string eventName, Action<T> action)
        {
            notification.Bind<T>(eventName, action);
        }
        /// <summary>
        /// 解绑数据；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="action">解绑的函数</param>
        public static void Unbind<T>(Action<T> action)
        {
            Unbind<T>(string.Empty, action);
        }
        /// <summary>
        /// 解绑数据；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <param name="action">解绑的函数</param>
        public static void Unbind<T>(string eventName, Action<T> action)
        {
            notification.Unbind<T>(eventName, action);
        }
        /// <summary>
        /// 获取制定绑定类型，被绑定函数的数量；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>绑定的数据量，若不存在则返回-1</returns>
        public static int BindCount<T>()
        {
            return BindCount<T>(string.Empty);
        }
        /// <summary>
        /// 获取制定绑定类型，被绑定函数的数量；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <returns>绑定的数据量，若不存在则返回-1</returns>
        public static int BindCount<T>(string eventName)
        {
            return notification.GetBindCount(typeof(T), eventName);
        }
        /// <summary>
        /// 清理所有绑定；
        /// 谨慎使用！
        /// </summary>
        public static void UnbindAll()
        {
            notification.UnbindAll();
        }
        #endregion
    }
}
