using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cosmos.Mvvm
{
    /// <summary>
    /// MVC调度类；
    /// 此结构为：M<->C<->P
    /// <->表示双向可操控；M与V解耦；
    /// </summary>
    public static class MVVM
    {
        #region ViewModel
        public static void RegisterCommand(string actionKey,Type cmdType)
        {
            ViewModel.Instance.RegisterCommand(actionKey, cmdType);
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
        public static bool HasMediator(string mediatorName)
        {
            return View.Instance.HasMediator(mediatorName);
        }
        public static void Dispatch(string actionKey, object sender,NotifyArgs notifyArgs)
        {
            View.Instance.Dispatch(actionKey, sender,notifyArgs);
        }
        public static void Dispatch(string actionKey, NotifyArgs notifyArgs)
        {
            View.Instance.Dispatch(actionKey, null, notifyArgs);
        }
        #endregion
        #region Model
        public static bool HasProxy(string proxyName)
        {
            return Model.Instance.HasProxy(proxyName);
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
        #endregion
    }
}
