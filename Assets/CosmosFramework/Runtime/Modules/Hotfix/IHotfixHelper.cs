using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Hotfix
{
    public interface IHotfixHelper
    {
        object LoadType(string typeName);
        IHotfixHelper SetAssembly(byte[] dllBytes, byte[] pdbBytes);
        object Instantiate(string typeName, params object[] parameters);
        object InvokeMethod(object methodObject, object instance, params object[] parameters);
        object InvokeMethod(string typeName, string methodName, object instance, params object[] parameters);
        object InvokeGenericMethod(string type, string method, object[] genericArgs, object instance, params object[] parameters);
    }
}
