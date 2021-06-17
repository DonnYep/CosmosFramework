using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    ///  在指定可使用此特性的类中将此特性挂载于无参方法上，则被挂载的单参方法可以被Unity的Update方法轮询，并接收float类型的 deltatime；
    /// 此特性在类方法中具有唯一性；
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
