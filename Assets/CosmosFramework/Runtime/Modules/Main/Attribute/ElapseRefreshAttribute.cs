using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 若在合法的Module中为指定方法挂载此特性，则此特性可以被Unity的Update方法轮询，并接收deltatime；
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ElapseRefreshAttribute:Attribute
    {
        static List<Exception> exceptionList = new List<Exception>();
        public static void GetRefreshAction(object obj, bool inherit, out Action<float> action)
        {
            var type = obj.GetType();
            action = null;
            var refreshMethods = Utility.Assembly.GetTypeMethodsByAttribute(type, typeof(ElapseRefreshAttribute), inherit);
            if (refreshMethods != null)
            {
                if (refreshMethods.Length > 1)
                    exceptionList.Add(new OverflowException("Each module's ElapseRefreshAttribute must have exactly one"));
                else if (refreshMethods.Length == 1)
                {
                    var args = refreshMethods[0].GetParameters();
                    if (args.Length !=1)
                        exceptionList.Add(new MethodAccessException("ElapseRefreshAttribute target method must be one argument"));
                    else
                    {
                        action = new Action<float>((deltatime) => refreshMethods[0].Invoke(obj, new object[] { deltatime}));
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
