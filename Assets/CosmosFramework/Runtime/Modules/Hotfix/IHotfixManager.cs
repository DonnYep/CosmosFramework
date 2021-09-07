using Cosmos.Hotfix;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Cosmos
{
    public interface IHotfixManager:IModuleManager
    {
        void SetHelper(IHotfixHelper helper);
        object PeekType(string typeName);
        void InitHotfixAssembly(byte[] dllBytes, byte[] pdbBytes);
        object Instantiate(string typeName, params object[] parameters);
        object InvokeMethod(object methodObject, object instance, params object[] parameters);
        object InvokeMethod(string typeName, string methodName, object instance, params object[] parameters);
        object InvokeGenericMethod(string typeName, string method, object[] genericArgs, object instance, params object[] parameters);
    }
}
