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
        void InitHotfixAssembly(byte[] dllBytes, byte[] pdbBytes);
        object InstanceObject(string typeName, params object[] parameters);
        object InvokeMethod(string type, string methodName, object instance, params object[] parameters);
    }
}
