using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 若在合法的Module中为指定方法挂载此特性，则此特性可以被Unity的LateUpdate方法轮询；
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class LateRefreshAttribute:Attribute
    {
        static List<Exception> exceptionList = new List<Exception>();
        public static void GetRefreshAction(object obj, bool inherit, out Action action)
        {
            var type = obj.GetType();
            action = null;
            var refreshMethods = Utility.Assembly.GetTypeMethodsByAttribute(type, typeof(LateRefreshAttribute), inherit);
            if (refreshMethods != null)
            {
                if (refreshMethods.Length > 1)
                    exceptionList.Add(new OverflowException("Each module's LateRefreshAttribute must have exactly one"));
                else if (refreshMethods.Length == 1)
                {
                    var args = refreshMethods[0].GetParameters();
                    if (args.Length > 0)
                        exceptionList.Add(new MethodAccessException("LateRefreshAttribute target method must be no argument"));
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
