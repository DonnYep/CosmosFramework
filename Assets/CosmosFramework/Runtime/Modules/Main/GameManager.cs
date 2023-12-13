using System.Collections.Generic;
using System;
using System.Linq;
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
        /// key=>moduleType；value===module
        /// </summary>
        readonly static Dictionary<Type, Module> moduleDict = new Dictionary<Type, Module>();
        /// <summary>
        /// 接口-module字典；
        /// key=>IModuleManager；value===Module
        /// </summary>
        readonly static Dictionary<Type, Type> interfaceModuleDict = new Dictionary<Type, Type>();
        /// <summary>
        /// 轮询更新委托
        /// </summary>
        internal static bool Pause { get; set; }
        /// <summary>
        /// 注册的模块数
        /// </summary>
        internal static int ModuleCount
        {
            get { return moduleDict.Count; }
        }
        internal static bool PrintModulePreparatory { get; set; } = true;
        /// <summary>
        /// 模块初始化时的异常集合；
        /// </summary>
        readonly static List<Exception> moduleInitExceptionList = new List<Exception>();
        /// <summary>
        /// 模块终止时的异常集合；
        /// </summary>
        readonly static List<Exception> moduleTerminateExceptionList = new List<Exception>();
        #endregion
        #region Methods
        /// <summary>
        /// 获取模块
        /// <para>若需要进行外部扩展，请继承自Module，需要实现接口 IModuleManager，并标记特性：ModuleAttribute</para>
        /// <para>如：public class TestManager:Module,ITestManager{}</para>
        /// <para>ITestManager 需要包含所有外部可调用的方法；具体请参考Cosmos源码</para> 
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
        internal static void OnRefresh()
        {
            if (Pause)
                return;
            tickRefreshHandler?.Invoke();
        }
        /// <summary>
        /// 时间流逝轮询
        /// </summary>
        /// <param name="realDeltaTime">物理世界的世界流逝单位时间</param>
        internal static void OnElapseRefresh(float realDeltaTime)
        {
            if (Pause)
                return;
            elapseRefreshHandler?.Invoke(realDeltaTime);
        }
        internal static void OnLateRefresh()
        {
            if (Pause)
                return;
            lateRefreshHandler?.Invoke();
        }
        internal static void OnFixedRefresh()
        {
            if (Pause)
                return;
            fixedRefreshHandler?.Invoke();
        }
        internal static bool HasModule(Type type)
        {
            return moduleDict.ContainsKey(type);
        }
        internal static void InitiateAssemblyModule(IEnumerable<System.Reflection.Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException("InitAssemblyModule : assemblies is invalid");
            Assemblies = assemblies.ToArray();
            var ModuleManagerType = typeof(IModuleManager);
            foreach (var asm in assemblies)
            {
                var modules = Utility.Assembly.GetInstancesByAttribute<ModuleAttribute, Module>(asm);
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
                                    Utility.Assembly.InvokeMethod(modules[i], ModuleConstant.INITIALIZATION);
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
        internal static void InitiateAppDomainModule()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assemblies = assemblies;
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
                                    Utility.Assembly.InvokeMethod(modules[i], ModuleConstant.INITIALIZATION);
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
        internal static void TerminateModules()
        {
            RemoveRefreshListener();
        }
        static void ActiveModule()
        {
            foreach (var module in moduleDict.Values)
            {
                try
                {
                    Utility.Assembly.InvokeMethod((module as Module), ModuleConstant.ACTIVE);
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
                    Utility.Assembly.InvokeMethod(module, ModuleConstant.PREPARATORY);
                    if (PrintModulePreparatory)
                        Utility.Debug.LogInfo($"Module :{module} is OnPreparatory");
                }
                catch (Exception e)
                {
                    moduleInitExceptionList.Add(e);
                }
            }
            AddRefreshListener();
        }
        static void AddRefreshListener()
        {
            foreach (var module in moduleDict.Values)
            {
                try
                {
                    TickRefreshHandler += module.Update;
                    FixedRefreshHandler += module.FixedUpdate;
                    LateRefreshHandler += module.LateUpdate;
                    ElapseRefreshHandler += module.ElapseUpdate;
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
        static void RemoveRefreshListener()
        {
            foreach (var module in moduleDict?.Values)
            {
                TickRefreshHandler -= module.Update;
                FixedRefreshHandler -= module.FixedUpdate;
                LateRefreshHandler -= module.LateUpdate;
                ElapseRefreshHandler -= module.ElapseUpdate;
            }
            OnDeactive();
        }
        static void OnDeactive()
        {
            foreach (var module in moduleDict?.Values)
            {
                try
                {
                    Utility.Assembly.InvokeMethod(module, ModuleConstant.DEACTIVE);
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
                    Utility.Assembly.InvokeMethod(module, ModuleConstant.TERMINATION);
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

            if (moduleTerminateExceptionList.Count > 0)
            {
                var arr = moduleTerminateExceptionList.ToArray();
                moduleInitExceptionList.Clear();
                throw new AggregateException(arr);
            }
            moduleDict.Clear();
            interfaceModuleDict.Clear();
        }
        #endregion
    }
}
