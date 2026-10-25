using Cosmos.Audio;
using Cosmos.DataTable;
using Cosmos.Download;
using Cosmos.Entity;
using Cosmos.Event;
using Cosmos.FSM;
using Cosmos.Input;
using Cosmos.Network;
using Cosmos.ObjectPool;
using Cosmos.Resource;
using Cosmos.UI;
using Cosmos.WebRequest;
using Cosmos.Procedure;
using System;
namespace Cosmos
{
    public partial class CosmosEntry
    {
        static bool isLaunched = false;
        /// <summary>
        /// 当前模块数量。
        /// </summary>
        public static int ModuleCount { get { return GameManager.ModuleCount; } }
        /// <summary>
        /// 在初始化时是否打印Module的debug信息。
        /// </summary>
        public static bool PrintModulePreparatory
        {
            get { return GameManager.PrintModulePreparatory; }
            set { GameManager.PrintModulePreparatory = value; }
        }
        public static event Action FixedRefreshHandler
        {
            add { GameManager.FixedRefreshHandler += value; }
            remove { GameManager.FixedRefreshHandler -= value; }
        }
        public static event Action LateRefreshHandler
        {
            add { GameManager.LateRefreshHandler += value; }
            remove { GameManager.LateRefreshHandler -= value; }
        }
        public static event Action TickRefreshHandler
        {
            add { GameManager.TickRefreshHandler += value; }
            remove { GameManager.TickRefreshHandler -= value; }
        }
        /// <summary>
        /// 时间流逝轮询委托。
        /// </summary>
        public static event Action<float> ElapseRefreshHandler
        {
            add { GameManager.ElapseRefreshHandler += value; }
            remove { GameManager.ElapseRefreshHandler -= value; }
        }
        //================================================
        // 内置模块的强类型入口：
        // 下面每个属性都等价于 GetModule<IXxxManager>()（一行糖），解析方式与自定义模块完全一致，
        // 因此把内置实现替换成自定义实现后，这里取到的仍然是替换后的实例。
        // 未注册时抛 InvalidOperationException（不再静默返回 null）。
        // 自定义模块没有对应属性：直接用 CosmosEntry.GetModule<T>() / CosmosEntry.TryGetModule<T>()。
        //================================================
        public static IAudioManager AudioManager { get { return GetModule<IAudioManager>(); } }
        public static IProcedureManager ProcedureManager { get { return GetModule<IProcedureManager>(); } }
        public static IFSMManager FSMManager { get { return GetModule<IFSMManager>(); } }
        public static IObjectPoolManager ObjectPoolManager { get { return GetModule<IObjectPoolManager>(); } }
        public static IInputManager InputManager { get { return GetModule<IInputManager>(); } }
        public static INetworkManager NetworkManager { get { return GetModule<INetworkManager>(); } }
        public static IResourceManager ResourceManager { get { return GetModule<IResourceManager>(); } }
        public static IUIManager UIManager { get { return GetModule<IUIManager>(); } }
        public static IEntityManager EntityManager { get { return GetModule<IEntityManager>(); } }
        public static IEventManager EventManager { get { return GetModule<IEventManager>(); } }
        public static IWebRequestManager WebRequestManager { get { return GetModule<IWebRequestManager>(); } }
        public static IDownloadManager DownloadManager { get { return GetModule<IDownloadManager>(); } }
        public static IDataTableManager DataTableManager { get { return GetModule<IDataTableManager>(); } }
        /// <summary>
        /// 初始化当前AppDomain下的Module。
        /// <para>内置模块由 <see cref="BuiltinModuleRegistry"/> 显式注册（零反射）；</para>
        /// <para>外部模块只从"引用了框架程序集"的程序集里按 [Module] 特性发现，不再全量扫描 AppDomain。</para>
        /// <para>注意：初始化尽量只执行一次！！！</para>
        /// </summary>
        public static void LaunchAppDomainModules()
        {
            Launch(() => GameManager.InitiateAppDomainModule());
        }
        /// <summary>
        /// 初始化内置模块 + 目标程序集下的Module。
        /// <para>注意：初始化尽量只执行一次！！！</para>
        /// </summary>
        /// <param name="assemblies">查询的目标程序集</param>
        public static void LaunchAssemblyModules(params System.Reflection.Assembly[] assemblies)
        {
            Launch(() => GameManager.InitiateAssemblyModule(assemblies));
        }
        /// <summary>
        /// 只注册传入的模块（不做任何程序集扫描，内置模块也不会被注册）。
        /// <para>适用于完全显式控制模块集合的场景；自定义模块需自行 new 出来。</para>
        /// <para>注意：初始化尽量只执行一次！！！</para>
        /// </summary>
        /// <param name="modules">自定义模块集合</param>
        public static void LaunchModules(params Module[] modules)
        {
            Launch(() => GameManager.InitiateModules(modules));
        }
        /// <summary>
        /// 启动守卫：防止重复启动；启动失败时回滚标记，允许修复后重试
        /// </summary>
        /// <param name="launchAction">真实的启动动作</param>
        static void Launch(Action launchAction)
        {
            if (isLaunched)
                return;
            isLaunched = true;
            try
            {
                MonoGameManager.Instance.ToString();
                launchAction();
            }
            catch (Exception)
            {
                //启动失败（例如某个模块 OnInitialization 抛异常）：允许下一次重试
                isLaunched = false;
                throw;
            }
        }
        public static void ReleaseLaunchedModules()
        {
            GameManager.TerminateModules();
            isLaunched = false;
        }
        /// <summary>
        /// 获取模块（契约接口）；
        /// <para>未注册时抛 <see cref="InvalidOperationException"/>，不会返回 null。</para>
        /// </summary>
        /// <typeparam name="T">模块契约接口（IModuleManager 派生）</typeparam>
        /// <returns>模块实例</returns>
        public static T GetModule<T>() where T : class, IModuleManager
        {
            return GameManager.GetModule<T>();
        }
        /// <summary>
        /// 尝试获取模块（契约接口）；未注册时返回 false，不抛异常。
        /// <para>仅用于"模块可选"的场景；必需模块请直接用 <see cref="GetModule{T}"/>。</para>
        /// </summary>
        /// <typeparam name="T">模块契约接口（IModuleManager 派生）</typeparam>
        /// <param name="module">模块实例；失败时为 null</param>
        /// <returns>是否获取成功</returns>
        public static bool TryGetModule<T>(out T module) where T : class, IModuleManager
        {
            return GameManager.TryGetModule<T>(out module);
        }
    }
}