using Cosmos.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Cosmos.Hotfix
{
    /// <summary>
    /// 热修复管理器
    /// </summary>
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
        public void InitHotfixAssembly(byte[] dllBytes,byte[] pdbBytes)
        {
            hotfixHelper.SetAssembly(dllBytes, pdbBytes);
        }
        public object InstanceObject(string typeName, params object[] parameters)
        {
            return hotfixHelper.InstanceObject(typeName, parameters);
        }
        public object InvokeMethod(string type, string methodName, object instance, params object[] parameters)
        {
            return hotfixHelper.InvokeMethod(type, methodName, instance, parameters);
        }
        protected override void OnPreparatory()
        {
            hotfixHelper = new DefaultReflectionHelper();
        }
        #endregion
    }
}