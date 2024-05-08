using System;
namespace Cosmos
{
    public static partial class Utility
    {
        /// <summary>
        /// 断言处理工具
        /// </summary>
        public static class Assert
        {
            /// <summary>
            /// 判断不为空
            /// </summary>
            /// <param name="obj">判断是否为空的对象</param>
            public static void NotNull(object obj)
            {
                if (obj == null)
                    throw new ArgumentNullException("object" + "isEmpty !");
            }
            /// <summary>
            /// 判断不为空
            /// </summary>
            /// <param name="obj">判断是否为空的对象</param>
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
            /// <param name="obj">判断是否为空的对象</param>
            /// <param name="notNullCallBack">若不为空，则执行回调</param>
            public static void NotNull(object obj, Action notNullCallBack)
            {
                if (obj == null)
                    throw new ArgumentNullException("object" + "is null !");
                else
                    notNullCallBack?.Invoke();
            }
            /// <summary>
            /// 判断不为空
            /// </summary>
            /// <param name="obj">判断是否为空的对象</param>
            /// <param name="notNullCallBack">不为空的回调</param>
            /// <param name="nullCallBack">为空时候的回调</param>
            public static void NotNull(object obj, Action notNullCallBack, Action nullCallBack)
            {
                if (obj == null)
                    nullCallBack?.Invoke();
                else
                    notNullCallBack?.Invoke();
            }
            /// <summary>
            /// 是否为空
            /// </summary>
            /// <param name="obj">判断是否为空的对象</param>
            public static bool IsNull(object obj)
            {
                return System.Object.ReferenceEquals(obj, null);
            }
            /// <summary>
            /// 是否为空
            /// </summary>
            /// <param name="obj">判断是否为空的对象</param>
            /// <param name="nullCallBack">为空时候的回调</param>
            public static void IsNull(object obj, Action nullCallBack)
            {

                if (IsNull(obj))
                    nullCallBack?.Invoke();
            }
            /// <summary>
            /// 是否为空
            /// </summary>
            /// <param name="obj">判断是否为空的对象</param>
            /// <param name="nullCallBack">为空时候的回调</param>
            /// <param name="notNullCallBack">不为空的回调</param>
            public static void IsNull(object obj, Action nullCallBack, Action notNullCallBack)
            {
                if (IsNull(obj))
                    nullCallBack?.Invoke();
                else
                    notNullCallBack?.Invoke();
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
            public static void State(bool expression, string message)
            {
                if (!expression)
                    throw new Exception(message);
            }
            /// <summary>
            /// 状态检测
            /// </summary>
            /// <param name="expression">表达式</param>
            /// <param name="callBack">当表达式为true时，调用回调</param>
            public static void State(bool expression, Action callBack)
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
            public static void State(bool expression, string message, Action callBack)
            {
                if (!expression)
                    throw new Exception(message);
                else
                    callBack?.Invoke();
            }
            /// <summary>
            /// 条件委托。若handler返回true，则run callBack。
            /// </summary>
            /// <typeparam name="T">泛型对象</typeparam>
            /// <param name="arg">对象</param>
            /// <param name="handler">处理者</param>
            /// <param name="callBack">回调</param>
            public static void Predicate<T>(T arg, Predicate<T> handler, Action<T> callBack)
            {
                if (handler == null)
                    return;
                if (handler.Invoke(arg))
                    callBack?.Invoke(arg);
            }
            /// <summary>
            /// 条件委托是否为true
            /// </summary>
            /// <param name="handler">条件委托</param>
            /// <param name="trueCallBack">true时候的回调</param>
            public static void Predicate(Func<bool> handler, Action trueCallBack)
            {
                if (handler.Invoke())
                    trueCallBack?.Invoke();
            }
            /// <summary>
            /// 条件委托是否为true
            /// </summary>
            /// <param name="handler">条件委托</param>
            /// <param name="trueCallBack">true时候的回调</param>
            /// <param name="falseCallBack">false时候的回调</param>
            public static void Predicate(Func<bool> handler, Action trueCallBack, Action falseCallBack)
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
            public static void ExceptionCatcher(Action handler, Action exceptionHandler)
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