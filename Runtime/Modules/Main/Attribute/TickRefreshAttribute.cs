﻿using System;
using System.Collections.Generic;
namespace Cosmos
{
    /// <summary>
    /// 在指定可使用此特性的类中将此特性挂载于无参方法上，则被挂载的无参方法可以被Unity的Update方法轮询；
    /// 此特性在类方法中具有唯一性；
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited =true)]
    public class TickRefreshAttribute:Attribute
    {
        static List<Exception> exceptionList = new List<Exception>();
        /// <summary>
        /// 获取轮询函数；
        /// 默认查找对象基类中的此特性；
        /// </summary>
        /// <param name="obj">包含此特性的对象</param>
        /// <param name="action">轮询委托</param>
        public static void GetRefreshAction(object obj,  out Action action)
        {
            GetRefreshAction(obj, true, out action);
        }
        /// <summary>
        /// 获取轮询函数；
        /// </summary>
        /// <param name="obj">包含此特性的对象</param>
        /// <param name="inherit">是否查找基类中的特性</param>
        /// <param name="action">轮询委托</param>
        public static void GetRefreshAction(object obj,bool inherit,out Action action)
        {
            var type = obj.GetType();
            action=null;
            var refreshMethods = Utility.Assembly.GetTypeMethodsByAttribute(type, typeof(TickRefreshAttribute), inherit);
            if (refreshMethods != null)
            {
                if (refreshMethods.Length > 1)
                    exceptionList.Add(new OverflowException($"{ obj }'s TickRefreshAttribute must have exactly one"));
                else if (refreshMethods.Length == 1)
                {
                    var args = refreshMethods[0].GetParameters();
                    if (args.Length > 0)
                        exceptionList.Add(new MethodAccessException("TickRefreshAttribute target method must be no argument"));
                    else
                    {
                        action= new Action(() => refreshMethods[0].Invoke(obj, null));
                    }
                }
            }
            if (exceptionList.Count > 0)
            {
                var arr = exceptionList.ToArray();
                exceptionList.Clear();
                throw new AggregateException(arr);
            }
        }
    }
}
