using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace Cosmos.Hotfix
{
    //================================================
    /*
     * 1、热修复模块；
     */
    //================================================
    [Module]
    internal class HotfixManager : Module, IHotfixManager
    {
        #region Properties
        IHotfixHelper hotfixHelper;
        #endregion
        #region Methods
        public void SetHelper(IHotfixHelper helper)
        {
            hotfixHelper = helper;
        }
        public object PeekType(string typeName)
        {
            return hotfixHelper.LoadType(typeName);
        }
        public void InitHotfixAssembly(byte[] dllBytes, byte[] pdbBytes)
        {
            hotfixHelper.SetAssembly(dllBytes, pdbBytes);
        }
        public object Instantiate(string typeName, params object[] parameters)
        {
            return hotfixHelper.Instantiate(typeName, parameters);
        }
        public object InvokeMethod(object methodObject, object instance, params object[] parameters)
        {
            return hotfixHelper.InvokeMethod(methodObject, instance, parameters);
        }
        public object InvokeMethod(string typeName, string methodName, object instance, params object[] parameters)
        {
            return hotfixHelper.InvokeMethod(typeName, methodName, instance, parameters);
        }
        public object InvokeGenericMethod(string typeName, string method, object[] genericArgs, object instance, params object[] parameters)
        {
            return hotfixHelper.InvokeGenericMethod(typeName, method, genericArgs, instance, parameters);
        }
        protected override void OnPreparatory()
        {
            hotfixHelper = new ReflectionHotfixHelper();
        }
        #endregion
    }
}