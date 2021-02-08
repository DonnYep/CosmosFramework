using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cosmos.Mvvm
{
    /// <summary>
    /// MVVM架构中，VM占有较大的逻辑比重，因此事件派发主要由VM进行控制；
    /// </summary>
    public static class MVVM
    {
        #region ViewModel
        public static void RegisterCommand(string actionKey, Type cmdType)
        {
            ViewModel.Instance.RegisterCommand(actionKey, cmdType);
        }
        public static void RegisterCommand<T>(string actionKey)
            where T:Command
        {
            RegisterCommand(actionKey, typeof(T));
        }
        public static void RemoveCommand(string actionKey)
        {
            ViewModel.Instance.RemoveCommand(actionKey);
        }
        public static bool HasCommand(string actionKey)
        {
            return ViewModel.Instance.HasCommand(actionKey);
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
            where T:Mediator
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
        public static void Dispatch(string actionKey)
        {
            View.Instance.Dispatch(actionKey, null, null);
        }
        public static void Dispatch(string actionKey, NotifyArgs notifyArgs)
        {
            View.Instance.Dispatch(actionKey, null, notifyArgs);
        }
        public static void Dispatch(string actionKey, object sender, NotifyArgs notifyArgs)
        {
            View.Instance.Dispatch(actionKey, sender, notifyArgs);
        }
        #endregion
        #region Model
        public static bool HasProxy(string proxyName)
        {
            return Model.Instance.HasProxy(proxyName);
        }
        public static T PeekProxy<T>(string proxyName)
            where T:Proxy
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
    }
}
