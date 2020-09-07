using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    public sealed partial class Utility
    {
        /// <summary>
        /// 断言处理工具;
        /// Assert断言仅用于测试环境调试，请勿在生产环境使用。
        /// </summary>
        [Obsolete("Assert断言仅用于测试环境调试，请勿在生产环境使用")]
        public static class Assert
        {
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 判断不为空
            /// </summary>
            /// <typeparam name="T">泛型类型</typeparam>
            /// <param name="arg">泛型对象</param>
            public static void NotNull(object obj)
            {
#if UNITY_EDITOR || CF_UNIT_TEST
                if (obj == null)
                    throw new ArgumentNullException("object" + obj.ToString() + "isEmpty !");
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 判断不为空
            /// </summary>
            /// <typeparam name="T">泛型类型</typeparam>
            /// <param name="arg">泛型对象</param>
            /// <param name="message">自定义需要打印的信息</param>
            public static void NotNull(object obj, object message)
            {
#if UNITY_EDITOR || CF_UNIT_TEST
                if (obj == null)
                    throw new ArgumentNullException(message.ToString());
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 判断不为空
            /// 若不为空，则执行回调
            /// </summary>
            /// <typeparam name="T">泛型类型</typeparam>
            /// <param name="arg">泛型对象</param>
            /// <param name="callBack">若不为空，则执行回调</param>
            public static void NotNull(object obj, Action notNullCallBack)
            {
#if UNITY_EDITOR || CF_UNIT_TEST

                if (obj == null)
                    throw new ArgumentNullException("object" + obj.ToString() + "is null !");
                else
                    notNullCallBack?.Invoke();
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 判断不为空
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="notNullCallBack">不为空的回调</param>
            /// <param name="nullCallBack">为空时候的回调</param>
            public static void NotNull(object obj, Action notNullCallBack, Action nullCallBack)
            {
#if UNITY_EDITOR || CF_UNIT_TEST

                if (obj == null)
                    nullCallBack?.Invoke();
                else
                    notNullCallBack?.Invoke();
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 是否为空
            /// </summary>
            /// <param name="obj">判断是否为空的对象</param>
            public static void IsNull(object obj)
            {
#if UNITY_EDITOR || CF_UNIT_TEST
                if (obj != null)
                    throw new ArgumentException("Object" + obj.ToString() + "must be null !");
#endif
            }
            public static void IsNull(object obj, Action nullCallBack)
            {
#if UNITY_EDITOR || CF_UNIT_TEST

                if (obj == null)
                    nullCallBack?.Invoke();
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 是否为空
            /// </summary>
            /// <param name="obj">判断是否为空的对象</param>
            /// <param name="nullCallBack">为空时候的回调</param>
            /// <param name="notNullCallBack">不为空的回调</param>
            public static void IsNull(object obj, Action nullCallBack, Action notNullCallBack)
            {
#if UNITY_EDITOR || CF_UNIT_TEST

                if (obj == null)
                    nullCallBack?.Invoke();
                else
                    notNullCallBack?.Invoke();
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 判断是否为继承关系
            /// </summary>
            /// <typeparam name="T1">父类</typeparam>
            /// <typeparam name="T2">子类</typeparam>
            /// <param name="callBack">符合继承时候的回调</param>
            public static void IsAssignable<T1, T2>(T1 super, T2 sub, Action<T1, T2> callBack)
            {
#if UNITY_EDITOR || CF_UNIT_TEST

                Type superType = typeof(T1);
                Type subType = typeof(T2);
                if (superType.IsAssignableFrom(superType))
                    callBack?.Invoke(super, sub);
                else
                    throw new InvalidCastException("SuperType : " + subType.FullName + "unssignable from subType : " + subType.FullName);
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 是否为继承
            /// </summary>
            /// <typeparam name="T1">super</typeparam>
            /// <typeparam name="T2">sub</typeparam>
            /// <param name="callBack">若不为继承，则启用回调</param>
            public static void IsAssignable<T1, T2>(Action callBack)
            {
#if UNITY_EDITOR || CF_UNIT_TEST

                if (!typeof(T1).IsAssignableFrom(typeof(T2)))
                    callBack?.Invoke();
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// if assaignable ,run callBack method
            /// </summary>
            /// <typeparam name="T1">superType</typeparam>
            /// <typeparam name="T2">subType</typeparam>
            /// <param name="sub">subType arg</param>
            /// <param name="callBack">若可执行，则回调，传入参数为sub对象</param>
            public static void IsAssignable<T1, T2>(T2 sub, Action<T2> callBack)
            {
#if UNITY_EDITOR || CF_UNIT_TEST

                Type superType = typeof(T1);
                Type subType = typeof(T2);
                if (superType.IsAssignableFrom(superType))
                    callBack?.Invoke(sub);
                else
                    throw new InvalidCastException("SuperType : " + subType.FullName + "unssignable from subType : " + subType.FullName);
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 状态检测
            /// </summary>
            /// <param name="expression">表达式</param>
            public static void State(bool expression)
            {
#if UNITY_EDITOR || CF_UNIT_TEST
                State(expression, "this state must be true");
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 状态检测
            /// </summary>
            /// <param name="expression">表达式</param>
            /// <param name="message">当表达式为false时，需要输出的信息</param>
            public static void State(bool expression, string message)
            {
#if UNITY_EDITOR || CF_UNIT_TEST
                if (!expression)
                    throw new Exception(message);
#endif
            }
            /// <summary>
            /// 状态检测
            /// </summary>
            /// <param name="expression">表达式</param>
            /// <param name="callBack">当表达式为true时，调用回调</param>
            public static void State(bool expression, Action callBack)
            {
#if UNITY_EDITOR || CF_UNIT_TEST

                if (expression)
                    callBack?.Invoke();
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 状态检测
            /// </summary>
            /// <param name="expression">表达式</param>
            /// <param name="message">当表达式为false时，需要输出的信息</param>
            /// <param name="callBack">当表达式为true时，调用回调</param>
            public static void State(bool expression, string message, Action callBack)
            {
#if UNITY_EDITOR || CF_UNIT_TEST

                if (!expression)
                    throw new Exception(message);
                else
                    callBack?.Invoke();
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 条件委托，
            /// 若handler返回true，则run callBack
            /// </summary>
            /// <typeparam name="T">泛型对象</typeparam>
            /// <param name="arg">对象</param>
            /// <param name="handler">处理者</param>
            /// <param name="callBack">回调</param>
            public static void Predicate<T>(T arg, Predicate<T> handler, Action<T> callBack)
            {
#if UNITY_EDITOR || CF_UNIT_TEST

                if (handler == null)
                    return;
                if (handler.Invoke(arg))
                    callBack?.Invoke(arg);
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 条件委托是否为true
            /// </summary>
            /// <param name="handler">条件委托</param>
            /// <param name="trueCallBack">true时候的回调</param>
            public static void Predicate(Func<bool> handler, Action trueCallBack)
            {
#if UNITY_EDITOR || CF_UNIT_TEST

                if (handler.Invoke())
                    trueCallBack?.Invoke();
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 条件委托是否为true
            /// </summary>
            /// <param name="handler">条件委托</param>
            /// <param name="trueCallBack">true时候的回调</param>
            /// <param name="falseCallBack">false时候的回调</param>
            public static void Predicate(Func<bool> handler, Action trueCallBack, Action falseCallBack)
            {
#if UNITY_EDITOR || CF_UNIT_TEST

                if (handler.Invoke())
                    trueCallBack?.Invoke();
                else
                    falseCallBack?.Invoke();
#endif
            }
            /// <summary>
            /// Assert断言仅用于测试环境调试
            /// 普通异常处理捕捉者
            /// </summary>
            /// <param name="handler">处理者函数</param>
            /// <param name="exceptionHandler">异常处理函数</param>
            public static void ExceptionCatcher(Action handler, Action exceptionHandler)
            {
#if UNITY_EDITOR || CF_UNIT_TEST
                try
                {
                    handler?.Invoke();
                }
                catch
                {
                    exceptionHandler?.Invoke();
                }
#endif
            }
        }
    }
}