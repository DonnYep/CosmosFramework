using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    public sealed partial class Utility
    {
        /// <summary>
        /// 断言处理工具
        /// </summary>
        public static class Assert
        {
            /// <summary>
            /// 判断不为空
            /// </summary>
            /// <typeparam name="T">泛型类型</typeparam>
            /// <param name="arg">泛型对象</param>
            public static void NotNull(object obj)
            {
                if (obj == null)
                    throw new ArgumentNullException("object" + obj.ToString() + "isEmpty !");
            }
            /// <summary>
            /// 判断不为空
            /// </summary>
            /// <typeparam name="T">泛型类型</typeparam>
            /// <param name="arg">泛型对象</param>
            /// <param name="message">自定义需要打印的信息</param>
            public static void NotNull(object obj, object message)
            {
                if (obj == null)
                    throw new ArgumentNullException(message.ToString());
            }
            /// <summary>
            /// 判断不为空
            /// 若不为空，则执行回调
            /// </summary>
            /// <typeparam name="T">泛型类型</typeparam>
            /// <param name="arg">泛型对象</param>
            /// <param name="callBack">若不为空，则执行回调</param>
            public static void NotNull(object obj, CFAction notNullCallBack)
            {
                if (obj == null)
                    throw new ArgumentNullException("object" + obj.ToString() + "is null !");
                else
                    notNullCallBack?.Invoke();
            }
            public static void NotNull(object obj, CFAction notNullCallBack, CFAction nullCallBack)
            {
                if (obj == null)
                    nullCallBack?.Invoke();
                else
                    notNullCallBack?.Invoke();
            }
            public static void IsNull(object obj)
            {
                if (obj != null)
                    throw new ArgumentException("Object" + obj.ToString() + "must be null !");
            }
            public static void IsNull(object obj, CFAction nullCallBack)
            {
                if (obj == null)
                    nullCallBack?.Invoke();
            }
            public static void IsNull(object obj, CFAction nullCallBack, CFAction notNullCallBack)
            {
                if (obj == null)
                    nullCallBack?.Invoke();
                else
                    notNullCallBack?.Invoke();
            }
            public static void IsAssignable<T1, T2>(T1 super, T2 sub, CFAction<T1, T2> callBack)
            {
                Type superType = typeof(T1);
                Type subType = typeof(T2);
                if (superType.IsAssignableFrom(superType))
                    callBack?.Invoke(super, sub);
                else
                    throw new InvalidCastException("SuperType : " + subType.FullName + "unssignable from subType : " + subType.FullName);
            }
            /// <summary>
            /// 是否为继承
            /// </summary>
            /// <typeparam name="T1">super</typeparam>
            /// <typeparam name="T2">sub</typeparam>
            /// <param name="callBack">若不为继承，则启用回调</param>
            public static void IsAssignable<T1, T2>(CFAction callBack)
            {
                if (!typeof(T1).IsAssignableFrom(typeof(T2)))
                    callBack?.Invoke();
            }
            /// <summary>
            /// if assaignable ,run callBack method
            /// </summary>
            /// <typeparam name="T1">superType</typeparam>
            /// <typeparam name="T2">subType</typeparam>
            /// <param name="sub">subType arg</param>
            /// <param name="callBack">若可执行，则回调，传入参数为sub对象</param>
            public static void IsAssignable<T1, T2>(T2 sub, CFAction<T2> callBack)
            {
                Type superType = typeof(T1);
                Type subType = typeof(T2);
                if (superType.IsAssignableFrom(superType))
                    callBack?.Invoke(sub);
                else
                    throw new InvalidCastException("SuperType : " + subType.FullName + "unssignable from subType : " + subType.FullName);
            }
            /// <summary>
            /// 状态检测
            /// </summary>
            /// <param name="expression">表达式</param>
            public static void State(bool expression)
            {
                State(expression, "this state must be true");
            }
            /// <summary>
            /// 状态检测
            /// </summary>
            /// <param name="expression">表达式</param>
            /// <param name="message">当表达式为false时，需要输出的信息</param>
            public static void State(bool expression,string message)
            {
                if (!expression)
                    throw new Exception(message);
            }
            /// <summary>
            /// 状态检测
            /// </summary>
            /// <param name="expression">表达式</param>
            /// <param name="callBack">当表达式为true时，调用回调</param>
            public static void State(bool expression,Action callBack)
            {
                if (expression)
                    callBack?.Invoke();
            }
            /// <summary>
            /// 状态检测
            /// </summary>
            /// <param name="expression">表达式</param>
            /// <param name="message">当表达式为false时，需要输出的信息</param>
            /// <param name="callBack">当表达式为true时，调用回调</param>
            public static void State(bool expression, string message,Action callBack)
            {
                if (!expression)
                    throw new Exception(message);
                else
                    callBack?.Invoke();
            }
            /// <summary>
            /// 条件委托，
            /// 若handler返回true，则run callBack
            /// </summary>
            /// <typeparam name="T">泛型对象</typeparam>
            /// <param name="arg">对象</param>
            /// <param name="handler">处理者</param>
            /// <param name="callBack">回调</param>
            public static void Predicate<T>(T arg, Predicate<T> handler, CFAction<T> callBack)
            {
                if (handler == null)
                    return;
                if (handler.Invoke(arg))
                    callBack?.Invoke(arg);
            }
            public static void Predicate(CFPredicateAction handler, CFAction callBack)
            {
                if (handler.Invoke())
                    callBack?.Invoke();
            }
            public static void Predicate(CFPredicateAction handler, CFAction trueCallBack, CFAction falseCallBack)
            {
                if (handler.Invoke())
                    trueCallBack?.Invoke();
                else
                    falseCallBack?.Invoke();
            }
            /// <summary>
            /// 普通异常处理捕捉者
            /// </summary>
            /// <param name="handler">处理者函数</param>
            /// <param name="exceptionHandler">异常处理函数</param>
            public static void ExceptionCatcher(CFAction handler, CFAction exceptionHandler)
            {
                try
                {
                    handler?.Invoke();
                }
                catch
                {
                    exceptionHandler?.Invoke();
                }
            }
        }
    }
}