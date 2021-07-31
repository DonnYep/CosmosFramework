using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace Cosmos.Hotfix
{
    public class ReflectionHotfixHelper : IHotfixHelper
    {
        Assembly assembly;
        Dictionary<string, MethodInfo> methodDict = new Dictionary<string, MethodInfo>();
        Dictionary<string, Type> strTypeDict = new Dictionary<string, Type>();

        public void SetAssembly(byte[] dllBytes, byte[] pdbBytes)
        {
            assembly = Assembly.Load(dllBytes, pdbBytes);
        }
        /// <summary>
        /// 获取类型；
        /// </summary>
        /// <param name="typeName">Type类型名</param>
        /// <returns>type类型的引用</returns>
        public object LoadType(string typeName)
        {
            return assembly.GetType(typeName);
        }
        public object InstanceObject(string typeName, params object[] parameters)
        {
            object instance;
            if (parameters == null)
                instance = assembly.CreateInstance(typeName);
            else
                instance = assembly.CreateInstance(typeName, true, BindingFlags.Default, null, parameters, null, null);
            return instance;
        }
        public object InvokeMethod(string typeName, string methodName, object instance, params object[] parameters)
        {
            var type = assembly.GetType(typeName);
            if (type != null)
            {
                MethodInfo method;
                if (instance != null)
                    method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                else
                    method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    return method.Invoke(instance, parameters);
                }
            }
            return null;
        }
    }
}
