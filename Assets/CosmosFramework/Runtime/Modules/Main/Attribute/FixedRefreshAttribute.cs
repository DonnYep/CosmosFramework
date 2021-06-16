using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 若在合法的Module中为指定方法挂载此特性，则此特性可以被Unity的FixedUpdate方法轮询；
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class FixedRefreshAttribute:Attribute
    {
        static List<Exception> exceptionList = new List<Exception>();
        public static void GetRefreshAction<T>(T obj, bool inherit, out Action action)
            where T : class
        {
            var type = typeof(T);
            action = null;
            var refreshMethods = Utility.Assembly.GetTypeMethodsByAttribute(type, typeof(FixedRefreshAttribute), inherit);
            if (refreshMethods != null)
            {
                if (refreshMethods.Length > 1)
                    exceptionList.Add(new OverflowException($"{obj}'s FixedRefreshAttribute must have exactly one"));
                else if (refreshMethods.Length == 1)
                {
                    var args = refreshMethods[0].GetParameters();
                    if (args.Length > 0)
                        exceptionList.Add(new MethodAccessException("FixedRefreshAttribute target method must be no argument"));
                    else
                    {
                        action = new Action(() => refreshMethods[0].Invoke(obj, null));
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
