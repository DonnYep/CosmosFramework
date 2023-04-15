using System;
namespace LiteMVC
{
    public static class MVCExts
    {
        public static IObservable Bind<T>(this IObservable @this, Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException($"action is empty!");
            MVC.Bind(action);
            return @this;
        }
        public static IObservable Bind<T>(this IObservable @this, string eventName, Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException($"action is empty!");
            MVC.Bind(eventName, action);
            return @this;
        }
        public static IObservable Unbind<T>(this IObservable @this, Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException($"action is empty!");
            MVC.Unbind(action);
            return @this;
        }
        public static IObservable Unbind<T>(this IObservable @this, string eventName, Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException($"action is empty!");
            MVC.Unbind(eventName, action);
            return @this;
        }
        public static bool HasBind<T>(this IObservable @this)
        {
            return MVC.HasBind<T>();
        }
        public static bool HasBind<T>(this IObservable @this, string eventName)
        {
            return MVC.HasBind<T>(eventName);
        }
        public static IObservable Dispatch<T>(this IObservable @this, T data)
        {
            MVC.Dispatch(string.Empty, data);
            return @this;
        }
        public static IObservable Dispatch<T>(this IObservable @this, string eventName, T data)
        {
            MVC.Dispatch(eventName, data);
            return @this;
        }
        public static int BindCount<T>(this IObservable @this)
        {
            return MVC.BindCount<T>();
        }
        public static int BindCount<T>(this IObservable @this, string eventName)
        {
            return MVC.BindCount<T>(eventName);
        }
    }
}
