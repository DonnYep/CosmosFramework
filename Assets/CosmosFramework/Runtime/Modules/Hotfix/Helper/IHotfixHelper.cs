using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Hotfix
{
    public interface IHotfixHelper
    {
        void SetAssembly(byte[] dllBytes, byte[] pdbBytes);
        object InstanceObject(string typeName, params object[] parameters);
        object InvokeMethod(string typeName,string methodName,object instance, params object[] parameters);
        //object InvokeGenericMethod(string type,string methodName,object instance, params object[] parameters);
    }
}
