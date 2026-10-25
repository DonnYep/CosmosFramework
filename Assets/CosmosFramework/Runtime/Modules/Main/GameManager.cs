using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
namespace Cosmos
{
    /// <summary>
    /// 当前设计为所有manager的一个管理器。
    /// 管理器对象都会通过这个对象的实例来调用，避免复杂化
    /// 可以理解为是一个Facade
    /// <para>模块注册有两条路径：</para>
    /// <para>1、内置模块：<see cref="BuiltinModuleRegistry"/> 显式 new，零反射；</para>
    /// <para>2、外部模块：仅扫描"引用了框架程序集"的程序集，再按 [Module] 特性实例化
    /// （不再全量扫描 AppDomain 下所有程序集，也不再逐次反射调用生命周期方法）。</para>
    /// <para>两条路径最终合并成同一份模块表，按 <see cref="ModuleAttribute.Priority"/> 统一排序：
    /// 用户模块可以显式声明更小的优先级插到内置模块之前 / 之间，未声明时排在全部内置模块之后。</para>
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
        /// 框架程序集（用于判断其它程序集是否可能含有模块）
        /// </summary>
        static readonly System.Reflection.Assembly frameworkAssembly = typeof(Module).Assembly;
        /// <summary>
        /// 框架程序集名（用于检查引用的程序集名）
        /// </summary>
        static readonly string frameworkAssemblyName = frameworkAssembly.GetName().Name;
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
        /// 模块是否已经启动；重复启动会被忽略
        /// </summary>
        internal static bool Launched { get; private set; }
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
        /// 获取模块（契约接口）；
        /// <para>模块未注册时抛 <see cref="InvalidOperationException"/>，而不是返回 null ——
        /// 模块缺失属于启动配置错误，应在第一次访问就暴露，而不是拖到业务深处变成 NRE。</para>
        /// </summary>
        /// <typeparam name="T">模块契约接口（IModuleManager 派生）</typeparam>
        /// <returns>模块实例</returns>
        internal static T GetModule<T>() where T : class, IModuleManager
        {
            if (TryGetModule<T>(out var module))
                return module;
            throw new InvalidOperationException(
                $"Module contract : {typeof(T)} is not registered. " +
                "Make sure the module which provides it is launched (CosmosEntry.LaunchAppDomainModules / LaunchAssemblyModules), " +
                "or check that another module has not taken the same contract.");
        }
        /// <summary>
        /// 尝试获取模块（契约接口）；未注册时返回 false，不抛异常。
        /// <para>仅用于"模块可选"的场景；必需模块请直接用 <see cref="GetModule{T}"/>。</para>
        /// </summary>
        /// <typeparam name="T">模块契约接口（IModuleManager 派生）</typeparam>
        /// <param name="module">模块实例；失败时为 null</param>
        /// <returns>是否获取成功</returns>
        internal static bool TryGetModule<T>(out T module) where T : class, IModuleManager
        {
            module = null;
            if (!interfaceModuleDict.TryGetValue(typeof(T), out var derivedType))
                return false;
            if (!moduleDict.TryGetValue(derivedType, out var registeredModule))
                return false;
            module = registeredModule as T;
            return module != null;
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
        /// <summary>
        /// 启动：内置模块 + 扫描当前 AppDomain 中"引用了框架程序集"的程序集
        /// </summary>
        internal static void InitiateAppDomainModule()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assemblies = assemblies;
            Launch(MergeModules(assemblies));
        }
        /// <summary>
        /// 启动：内置模块 + 扫描指定程序集
        /// </summary>
        /// <param name="assemblies">目标程序集</param>
        internal static void InitiateAssemblyModule(IEnumerable<System.Reflection.Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException("InitiateAssemblyModule : assemblies is invalid");
            Assemblies = assemblies.ToArray();
            Launch(MergeModules(Assemblies));
        }
        /// <summary>
        /// 启动：只注册传入的模块（不做任何扫描）
        /// </summary>
        /// <param name="modules">模块集合</param>
        internal static void InitiateModules(IEnumerable<Module> modules)
        {
            if (modules == null)
                throw new ArgumentNullException("InitiateModules : modules is invalid");
            Launch(modules);
        }
        internal static void TerminateModules()
        {
            RemoveRefreshListener();
        }
        /// <summary>
        /// 合并内置模块与扫描到的用户模块，并按统一优先级排序；
        /// <para>内置模块先放入，稳定排序保证"同优先级时内置优先"。</para>
        /// </summary>
        /// <param name="assemblies">待扫描的程序集</param>
        static List<Module> MergeModules(IEnumerable<System.Reflection.Assembly> assemblies)
        {
            var builtinModules = new List<Module>(BuiltinModuleRegistry.CreateModules());
            //内置模块必须显式声明唯一且靠前的优先级，否则用户模块的默认优先级就没有意义
            ModuleOrdering.ValidateBuiltinPriorities(builtinModules);
            var modules = new List<Module>(builtinModules.Count);
            modules.AddRange(builtinModules);
            modules.AddRange(DiscoverModules(assemblies));
            return ModuleOrdering.Sort(modules);
        }
        /// <summary>
        /// 启动流程：注册 → 激活 → 准备 → 挂载轮询
        /// </summary>
        /// <param name="modules">待启动模块</param>
        static void Launch(IEnumerable<Module> modules)
        {
            if (Launched)
            {
                Utility.Debug.LogWarning("[Cosmos] Modules have already been launched, this launch request is ignored.");
                return;
            }
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                foreach (var module in modules)
                {
                    try
                    {
                        RegisterModule(module);
                    }
                    catch (Exception e)
                    {
                        moduleInitExceptionList.Add(e);
                    }
                }
                ActiveModule();
            }
            finally
            {
                stopwatch.Stop();
                Launched = true;
                if (PrintModulePreparatory)
                    Utility.Debug.LogInfo($"[Cosmos] modules : {moduleDict.Count} , registration & preparation elapsed {stopwatch.ElapsedMilliseconds} ms");
            }
            ThrowModuleInitException();
        }
        /// <summary>
        /// 注册单个模块：解析接口 → 记录映射 → 调用初始化
        /// </summary>
        /// <param name="module">模块</param>
        static void RegisterModule(Module module)
        {
            if (module == null)
                return;
            var type = module.GetType();
            if (moduleDict.ContainsKey(type))
            {
                moduleInitExceptionList.Add(new ArgumentException($"Module : {type} is already exist!"));
                return;
            }
            var interfaceType = ModuleContractResolver.Resolve(type);
            if (interfaceType == null)
            {
                Utility.Debug.LogWarning($"Module : {type} does not implement a contract derived from {typeof(IModuleManager).Name}, skipped.");
                return;
            }
            //同一契约只能由一个模块提供；旧实现是静默覆盖，会让内置模块继续运行、GetModule 却返回新模块
            if (interfaceModuleDict.TryGetValue(interfaceType, out var registeredType))
            {
                moduleInitExceptionList.Add(new ArgumentException(
                    $"Module : {type} can not be registered, the contract {interfaceType} is already provided by {registeredType}. " +
                    "Remove one of them, or register an explicit module set via CosmosEntry.LaunchModules."));
                return;
            }
            moduleDict.Add(type, module);
            interfaceModuleDict[interfaceType] = type;
            module.InvokeInitialization();
        }
        static void ActiveModule()
        {
            foreach (var module in moduleDict.Values)
            {
                try
                {
                    module.InvokeActive();
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
                    module.InvokePreparatory();
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
                    module.InvokeDeactive();
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
                    module.InvokeTermination();
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

            //模块表与模块挂载点必须同生共死，否则重新启动会复用上一轮的挂载点与子物体
            MonoGameManager.ClearModuleInstanceObjects();

            //先清空注册表再抛异常，避免终止后的模块仍可被访问、或重复执行销毁回调
            moduleDict.Clear();
            interfaceModuleDict.Clear();
            ModuleContractResolver.Clear();
            Launched = false;

            if (moduleTerminateExceptionList.Count > 0)
            {
                var arr = moduleTerminateExceptionList.ToArray();
                moduleTerminateExceptionList.Clear();
                throw new AggregateException(arr);
            }
        }
        static void ThrowModuleInitException()
        {
            if (moduleInitExceptionList.Count == 0)
                return;
            var arr = moduleInitExceptionList.ToArray();
            moduleInitExceptionList.Clear();
            throw new AggregateException(arr);
        }
        /// <summary>
        /// 在目标程序集里发现带 [Module] 特性的模块并实例化。
        /// <para>只扫描"引用了框架程序集"的程序集：模块必然引用 <see cref="Module"/> 所在程序集，
        /// 因此可以据此过滤掉 mscorlib / System.* / UnityEngine.* 等大量无关程序集。</para>
        /// <para>排序规则：先按 <see cref="ModuleAttribute.Priority"/>（越小越先），再按类型全名，
        /// 保证每次启动的初始化顺序一致。</para>
        /// </summary>
        /// <param name="assemblies">候选程序集</param>
        /// <returns>模块实例集合</returns>
        static List<Module> DiscoverModules(IEnumerable<System.Reflection.Assembly> assemblies)
        {
            var discoveredTypes = new List<KeyValuePair<int, Type>>();
            var visited = new HashSet<System.Reflection.Assembly>();
            foreach (var assembly in assemblies)
            {
                if (assembly == null || assembly == frameworkAssembly || assembly.IsDynamic)
                    continue;
                if (!visited.Add(assembly) || !MayContainModules(assembly))
                    continue;
                var types = GetLoadableTypes(assembly);
                for (int i = 0; i < types.Length; i++)
                {
                    var type = types[i];
                    if (type == null || !type.IsClass || type.IsAbstract || type.ContainsGenericParameters)
                        continue;
                    if (!typeof(Module).IsAssignableFrom(type))
                        continue;
                    var attribute = type.GetCustomAttribute<ModuleAttribute>(false);
                    if (attribute == null)
                        continue;
                    discoveredTypes.Add(new KeyValuePair<int, Type>(attribute.Priority, type));
                }
            }
            discoveredTypes.Sort((a, b) =>
            {
                var result = a.Key.CompareTo(b.Key);
                return result != 0 ? result : string.CompareOrdinal(a.Value.FullName, b.Value.FullName);
            });
            var modules = new List<Module>(discoveredTypes.Count);
            for (int i = 0; i < discoveredTypes.Count; i++)
            {
                try
                {
                    modules.Add((Module)Activator.CreateInstance(discoveredTypes[i].Value));
                }
                catch (Exception e)
                {
                    moduleInitExceptionList.Add(e);
                }
            }
            return modules;
        }
        /// <summary>
        /// 该程序集是否可能含有模块：是否直接引用了框架程序集
        /// </summary>
        /// <param name="assembly">目标程序集</param>
        /// <returns>是否可能含有模块</returns>
        static bool MayContainModules(System.Reflection.Assembly assembly)
        {
            try
            {
                var references = assembly.GetReferencedAssemblies();
                for (int i = 0; i < references.Length; i++)
                {
                    if (references[i].Name == frameworkAssemblyName)
                        return true;
                }
            }
            catch (Exception)
            {
                //个别动态/裁剪过的程序集可能无法读取引用信息，按"可能含有"处理更安全
                return true;
            }
            return false;
        }
        /// <summary>
        /// 安全取类型：程序集里存在加载失败的类型时不抛 <see cref="ReflectionTypeLoadException"/>
        /// </summary>
        /// <param name="assembly">目标程序集</param>
        /// <returns>可用的类型数组</returns>
        static Type[] GetLoadableTypes(System.Reflection.Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types != null ? e.Types.Where(t => t != null).ToArray() : new Type[0];
            }
            catch (Exception)
            {
                return new Type[0];
            }
        }
        #endregion
    }
}
