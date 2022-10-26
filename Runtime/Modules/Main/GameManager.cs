﻿using System.Collections.Generic;
using System;
namespace Cosmos
{
    /// <summary>
    /// 当前设计为所有manager的一个管理器。
    /// 管理器对象都会通过这个对象的实例来调用，避免复杂化
    /// 可以理解为是一个Facade
    /// </summary>
    internal static class GameManager
    {
        #region Properties
        internal static event Action FixedRefreshHandler
        {
            add { fixedRefreshHandler += value; }
            remove { fixedRefreshHandler -= value; }
        }
        internal static event Action LateRefreshHandler
        {
            add { lateRefreshHandler += value; }
            remove { lateRefreshHandler -= value; }
        }
        internal static event Action TickRefreshHandler
        {
            add { tickRefreshHandler += value; }
            remove { tickRefreshHandler -= value; }
        }
        /// <summary>
        /// 时间流逝轮询委托；
        /// </summary>
        internal static event Action<float> ElapseRefreshHandler
        {
            add { elapseRefreshHandler += value; }
            remove { elapseRefreshHandler -= value; }
        }
        /// <summary>
        /// Runtime所包含的程序集；
        /// </summary>
        internal static System.Reflection.Assembly[] Assemblies { get; private set; }
        static Action fixedRefreshHandler;
        static Action lateRefreshHandler;
        static Action tickRefreshHandler;
        static Action<float> elapseRefreshHandler;
        /// <summary>
        /// 模块字典；
        /// key=>moduleType；value=>module
        /// </summary>
        static Dictionary<Type, Module> moduleDict;
        /// <summary>
        /// 接口-module字典；
        /// key=>IModuleManager；value=>Module
        /// </summary>
        static Dictionary<Type, Type> interfaceModuleDict;
        /// <summary>
        /// 轮询更新委托
        /// </summary>
        internal static bool IsPause { get; private set; }
        //当前注册的模块总数
        static int moduleCount = 0;
        internal static int ModuleCount { get { return moduleCount; } }
        internal static bool PrintModulePreparatory { get; set; } = true;

        /// <summary>
        /// 模块轮询字典缓存；
        /// key=>module object ; value=>Action ;
        /// </summary>
        static Dictionary<object, Action> tickRefreshDict;
        static Dictionary<object, Action> fixedRefreshDict;
        static Dictionary<object, Action> lateRefreshDict;
        static Dictionary<object, Action<float>> elapseRefreshDict;

        /// <summary>
        /// 模块初始化时的异常集合；
        /// </summary>
        static List<Exception> moduleInitExceptionList;
        /// <summary>
        /// 模块终止时的异常集合；
        /// </summary>
        static List<Exception> moduleTerminateExceptionList;
        #endregion
        #region Methods
        /// <summary>
        /// 获取模块；
        /// 若需要进行外部扩展，请继承自Module，需要实现接口 IModuleManager，并标记特性：ModuleAttribute
        /// 如：public class TestManager:Module,ITestManager{}
        /// ITestManager 需要包含所有外部可调用的方法；具体请参考Cosmos源码；
        /// </summary>
        /// <typeparam name="T">内置模块接口</typeparam>
        /// <returns>模板模块接口</returns>
        internal static T GetModule<T>() where T : class, IModuleManager
        {
            Type interfaceType = typeof(T);
            var hasType = interfaceModuleDict.TryGetValue(interfaceType, out var derivedType);
            if (!hasType)
                return null;
            moduleDict.TryGetValue(derivedType, out var module);
            return module as T;
        }
        internal static void OnPause()
        {
            IsPause = true;
        }
        internal static void OnUnPause()
        {
            IsPause = false;
        }
        internal static void OnRefresh()
        {
            if (IsPause)
                return;
            tickRefreshHandler?.Invoke();
        }
        /// <summary>
        /// 时间流逝轮询;
        /// </summary>
        /// <param name="realDeltaTime">物理世界的世界流逝单位时间</param>
        internal static void OnElapseRefresh(float realDeltaTime)
        {
            if (IsPause)
                return;
            elapseRefreshHandler?.Invoke(realDeltaTime);
        }
        internal static void OnLateRefresh()
        {
            if (IsPause)
                return;
            lateRefreshHandler?.Invoke();
        }
        internal static void OnFixRefresh()
        {
            if (IsPause)
                return;
            fixedRefreshHandler?.Invoke();
        }
        internal static bool HasModule(Type type)
        {
            return moduleDict.ContainsKey(type);
        }
        internal static void Dispose()
        {
            OnDeactive();
        }
        static void ModuleTermination(Module module)
        {
            var type = module.GetType();
            if (HasModule(type))
            {
                Utility.Assembly.InvokeMethod(module, ModuleConstant.OnDeactive);
                var m = moduleDict[type];
                if (tickRefreshDict.Remove(module, out var tickAction))
                    TickRefreshHandler -= tickAction;
                if (fixedRefreshDict.Remove(module, out var fixedAction))
                    FixedRefreshHandler -= fixedAction;
                if (lateRefreshDict.Remove(module, out var lateAction))
                    LateRefreshHandler -= lateAction;
                if (elapseRefreshDict.Remove(module, out var elapseAction))
                    ElapseRefreshHandler -= elapseAction;
                moduleDict.Remove(type);
                moduleCount--;
                Utility.Assembly.InvokeMethod(module, ModuleConstant.OnTermination);
                Utility.Debug.LogInfo($"Module :{module} is OnTermination", DebugColor.olive);
            }
            else
                throw new ArgumentException($"Module : {module} is not exist!");
        }
        internal static void InitAssemblyModule(System.Reflection.Assembly[] assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException("InitAssemblyModule : assemblies is invalid");
            Assemblies = assemblies;
            InitDicts();
            var ModuleManagerType = typeof(IModuleManager);
            var assemblyLength = assemblies.Length;
            for (int h = 0; h < assemblyLength; h++)
            {
                var modules = Utility.Assembly.GetInstancesByAttribute<ModuleAttribute, Module>(assemblies[h]);
                for (int i = 0; i < modules.Length; i++)
                {
                    var type = modules[i].GetType();
                    var module = modules[i];
                    if (ModuleManagerType.IsAssignableFrom(type))
                    {
                        if (!moduleDict.ContainsKey(type))
                        {
                            try
                            {
                                var interfaces = type.GetInterfaces();
                                Type interfaceType = null;
                                foreach (var inter in interfaces)
                                {
                                    if (inter.Name.Contains(type.Name))
                                    {
                                        interfaceType = inter;
                                        break;
                                    }
                                }
                                if (interfaceType != null)
                                {
                                    moduleDict.TryAdd(type, module);
                                    interfaceModuleDict.TryAdd(interfaceType, type);
                                    Utility.Assembly.InvokeMethod(modules[i], ModuleConstant.OnInitialization);
                                    moduleCount++;
                                }
                            }
                            catch (Exception e)
                            {
                                Utility.Debug.LogError(e);
                            }
                        }
                        else
                            moduleInitExceptionList.Add(new ArgumentException($"Module : {type} is already exist!"));
                    }
                }
            }
            ActiveModule();
        }
        internal static void InitAppDomainModule()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assemblies = assemblies;
            InitDicts();
            var ModuleManagerType = typeof(IModuleManager);
            var assemblyLength = assemblies.Length;
            for (int h = 0; h < assemblyLength; h++)
            {
                var modules = Utility.Assembly.GetInstancesByAttribute<ModuleAttribute, Module>(assemblies[h]);
                for (int i = 0; i < modules.Length; i++)
                {
                    var type = modules[i].GetType();
                    var module = modules[i];
                    if (ModuleManagerType.IsAssignableFrom(type))
                    {
                        if (!moduleDict.ContainsKey(type))
                        {
                            try
                            {
                                var interfaces = type.GetInterfaces();
                                Type interfaceType = null;
                                foreach (var inter in interfaces)
                                {
                                    if (inter.Name.Contains(type.Name))
                                    {
                                        interfaceType = inter;
                                        break;
                                    }
                                }
                                if (interfaceType != null)
                                {
                                    moduleDict.TryAdd(type, module);
                                    interfaceModuleDict.TryAdd(interfaceType, type);
                                    Utility.Assembly.InvokeMethod(modules[i], ModuleConstant.OnInitialization);
                                    moduleCount++;
                                }
                            }
                            catch (Exception e)
                            {
                                Utility.Debug.LogError(e);
                            }
                        }
                        else
                            moduleInitExceptionList.Add(new ArgumentException($"Module : {type} is already exist!"));
                    }
                }
            }
            ActiveModule();
        }
        static void ActiveModule()
        {
            foreach (var module in moduleDict.Values)
            {
                try
                {
                    Utility.Assembly.InvokeMethod((module as Module), ModuleConstant.OnActive);
                }
                catch (Exception e)
                {
                    moduleInitExceptionList.Add(e);
                }
            }
            PrepareModule();
        }
        static void PrepareModule()
        {
            foreach (var module in moduleDict.Values)
            {
                try
                {
                    Utility.Assembly.InvokeMethod(module, ModuleConstant.OnPreparatory);
                    if (PrintModulePreparatory)
                        Utility.Debug.LogInfo($"Module :{module} is OnPreparatory");
                }
                catch (Exception e)
                {
                    moduleInitExceptionList.Add(e);
                }
            }
            AddRefreshListen();
        }
        static void AddRefreshListen()
        {
            foreach (var module in moduleDict.Values)
            {
                try
                {
                    TickRefreshAttribute.GetRefreshAction(module, true, out var tickAction);
                    if (tickAction != null)
                    {
                        tickRefreshDict.Add(module, tickAction);
                        TickRefreshHandler += tickAction;
                    }
                    LateRefreshAttribute.GetRefreshAction(module, true, out var lateAction);
                    if (lateAction != null)
                    {
                        lateRefreshDict.Add(module, lateAction);
                        LateRefreshHandler += lateAction;
                    }
                    FixedRefreshAttribute.GetRefreshAction(module, true, out var fixedAction);
                    if (fixedAction != null)
                    {
                        fixedRefreshDict.Add(module, fixedAction);
                        FixedRefreshHandler += fixedAction;
                    }
                    ElapseRefreshAttribute.GetRefreshAction(module, true, out var elapseAction);
                    if (elapseAction != null)
                    {
                        elapseRefreshDict.Add(module, elapseAction);
                        ElapseRefreshHandler += elapseAction;
                    }
                }
                catch (Exception e)
                {
                    moduleInitExceptionList.Add(e);
                }

            }
            if (moduleInitExceptionList.Count > 0)
            {
                var arr = moduleInitExceptionList.ToArray();
                moduleInitExceptionList.Clear();
                throw new AggregateException(arr);
            }
        }
        static void OnDeactive()
        {
            foreach (var module in moduleDict?.Values)
            {
                try
                {
                    Utility.Assembly.InvokeMethod(module, ModuleConstant.OnDeactive);
                }
                catch (Exception e)
                {
                    moduleTerminateExceptionList.Add(e);
                }
            }
            OnTermination();
        }
        static void OnTermination()
        {
            foreach (var module in moduleDict?.Values)
            {
                try
                {
                    Utility.Assembly.InvokeMethod(module, ModuleConstant.OnTermination);
                }
                catch (Exception e)
                {
                    moduleTerminateExceptionList.Add(e);
                }
            }
            GameManager.tickRefreshHandler = null;
            GameManager.lateRefreshHandler = null;
            GameManager.fixedRefreshHandler = null;
            GameManager.elapseRefreshHandler = null;

            tickRefreshDict.Clear();
            lateRefreshDict.Clear();
            fixedRefreshDict.Clear();
            elapseRefreshDict.Clear();

            if (moduleTerminateExceptionList.Count > 0)
            {
                var arr = moduleTerminateExceptionList.ToArray();
                moduleInitExceptionList.Clear();
                throw new AggregateException(arr);
            }
        }
        static void InitDicts()
        {
            if (moduleDict == null)
            {
                moduleDict = new Dictionary<Type, Module>();
                interfaceModuleDict = new Dictionary<Type, Type>();

                moduleInitExceptionList = new List<Exception>();
                moduleTerminateExceptionList = new List<Exception>();

                tickRefreshDict = new Dictionary<object, Action>();
                fixedRefreshDict = new Dictionary<object, Action>();
                lateRefreshDict = new Dictionary<object, Action>();
                elapseRefreshDict = new Dictionary<object, Action<float>>();
            }
        }
        #endregion
    }
}
