using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace PureMVC
{
    public class MVC
    {
        #region Controller
        public static void RegisterCommand(string actionKey, Type cmdType)
        {
            Controller.Instance.RegisterCommand(actionKey, cmdType);
        }
        public static void RegisterCommand<T>(string actionKey)
            where T : Command
        {
            RegisterCommand(actionKey, typeof(T));
        }
        public static void RemoveCommand(string actionKey)
        {
            Controller.Instance.RemoveCommand(actionKey);
        }
        public static bool HasCommand(string actionKey)
        {
            return Controller.Instance.HasCommand(actionKey);
        }
        #endregion
        #region View
        public static void RegisterMediator(Mediator mediator)
        {
            View.Instance.RegisterMediator(mediator);
        }
        public static Mediator PeekMediator(string mediatorName)
        {
            return View.Instance.PeekMediator(mediatorName);
        }
        public static T PeekMediator<T>(string mediatorName)
            where T : Mediator
        {
            return View.Instance.PeekMediator(mediatorName) as T;
        }
        public static bool HasMediator(string mediatorName)
        {
            return View.Instance.HasMediator(mediatorName);
        }
        public static void RemoveMediator(string mediatorName)
        {
            View.Instance.RemoveMediator(mediatorName);
        }
        public static void Dispatch(INotifyArgs notifyArgs)
        {
            View.Instance.Dispatch(notifyArgs);
        }
        public static void Dispatch(string notifyName)
        {
            View.Instance.Dispatch(new NotifyArgs(notifyName));
        }
        #endregion
        #region Model
        public static bool HasProxy(string proxyName)
        {
            return Model.Instance.HasProxy(proxyName);
        }
        public static T PeekProxy<T>(string proxyName)
            where T : Proxy
        {
            return Model.Instance.PeekProxy(proxyName) as T;
        }
        public static Proxy PeekProxy(string proxyName)
        {
            return Model.Instance.PeekProxy(proxyName);
        }
        public static void RegisterProxy(Proxy proxy)
        {
            Model.Instance.RegisterProxy(proxy);
        }
        public static void RemoveProxy(string proxyName, out Proxy proxy)
        {
            Model.Instance.RemoveProxy(proxyName, out proxy);
        }
        public static void RemoveProxy(string proxyName)
        {
            Model.Instance.RemoveProxy(proxyName, out _);
        }
        #endregion
        /// <summary>
        /// 自动注册被标记的PureMVC成员；
        /// MVCCommandAttribute;
        /// MVCProxyAttribute
        /// MVCMediatorAttribute
        /// </summary>
        /// <param name="assembly">目标程序集</param>
        public static void RegisterAttributedMVC(Assembly assembly)
        {
            var mediators = GetInstancesByAttribute<MVCMediatorAttribute, Mediator>(false, assembly);
            var medLength = mediators.Length;
            for (int i = 0; i < medLength; i++)
            {
                RegisterMediator(mediators[i]);
            }
            var proxies = GetInstancesByAttribute<MVCProxyAttribute, Proxy>(false, assembly);
            var prxLength = proxies.Length;
            for (int i = 0; i < prxLength; i++)
            {
                RegisterProxy(proxies[i]);
            }
            var cmdTypes = GetDerivedTypes<Command>(assembly);
            var cmdLength = cmdTypes.Length;
            for (int i = 0; i < cmdLength; i++)
            {
                var attributes = cmdTypes[i].GetCustomAttributes<MVCCommandAttribute>().ToList();
                var attLength = attributes.Count();
                if (attLength > 0)
                {
                    for (int j = 0; j < attLength; j++)
                    {
                        RegisterCommand(attributes[j].ActionKey, cmdTypes[i]);
                    }
                }
            }
        }
        static K[] GetInstancesByAttribute<T, K>(bool inherit = false, System.Reflection.Assembly assembly = null)
             where T : Attribute
             where K : class
        {
            List<K> set = new List<K>();
            var types = GetDerivedTypes<K>(assembly);
            int length = types.Length;
            for (int i = 0; i < length; i++)
            {
                if (types[i].GetCustomAttributes<T>(inherit).Count() > 0)
                {
                    set.Add(Activator.CreateInstance(types[i]) as K);
                }
            }
            return set.ToArray();
        }
        static Type[] GetDerivedTypes<T>(System.Reflection.Assembly assembly = null)
            where T : class
        {
            Type type = typeof(T);
            List<Type> set = new List<Type>();
            Type[] types;
            if (assembly == null)
                types = type.Assembly.GetTypes();
            else
                types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (type.IsAssignableFrom(types[i]))
                {
                    if (types[i].IsClass && !types[i].IsAbstract)
                    {
                        set.Add(types[i]);
                    }
                }
            }
            return set.ToArray();
        }
    }
}