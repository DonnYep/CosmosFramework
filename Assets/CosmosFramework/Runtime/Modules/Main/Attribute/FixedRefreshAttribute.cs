using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 在指定可使用此特性的类中将此特性挂载于无参方法上，则被挂载的无参方法可以被Unity的FixedUpdate方法轮询；
    /// 此特性在类方法中具有唯一性；
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class FixedRefreshAttribute:Attribute
    {
        static List<Exception> exceptionList = new List<Exception>();
        public static void GetRefreshAction(object obj, bool inherit, out Action action)
        {
            var type = obj.GetType();
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
