using Cosmos.Audio;
using Cosmos.Config;
using Cosmos.Controller;
using Cosmos.Data;
using Cosmos.DataTable;
using Cosmos.Download;
using Cosmos.Entity;
using Cosmos.Event;
using Cosmos.FSM;
using Cosmos.Hotfix;
using Cosmos.Input;
using Cosmos.Network;
using Cosmos.ObjectPool;
using Cosmos.Resource;
using Cosmos.Scene;
using Cosmos.UI;
using Cosmos.WebRequest;
using Cosmos.Procedure;
using System;
namespace Cosmos
{
    public class CosmosEntry
    {
        static bool isLaunched = false;
        /// <summary>
        /// 当前模块数量；
        /// </summary>
        public static int ModuleCount { get { return GameManager.ModuleCount; } }
        /// <summary>
        /// 在初始化时是否打印Module的debug信息；
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
        /// 时间流逝轮询委托；
        /// </summary>
        public static event Action<float> ElapseRefreshHandler
        {
            add { GameManager.ElapseRefreshHandler += value; }
            remove { GameManager.ElapseRefreshHandler -= value; }
        }
        public static IAudioManager AudioManager { get { return GetModule<IAudioManager>(); } }
        public static IProcedureManager  ProcedureManager{ get { return GetModule<IProcedureManager>(); } }
        public static IControllerManager ControllerManager { get { return GetModule<IControllerManager>(); } }
        public static IFSMManager FSMManager { get { return GetModule<IFSMManager>(); } }
        public static IObjectPoolManager ObjectPoolManager { get { return GetModule<IObjectPoolManager>(); } }
        public static IConfigManager ConfigManager { get { return GetModule<IConfigManager>(); } }
        public static IInputManager InputManager { get { return GetModule<IInputManager>(); } }
        public static INetworkManager NetworkManager { get { return GetModule<INetworkManager>(); } }
        public static IResourceManager ResourceManager { get { return GetModule<IResourceManager>(); } }
        public static IUIManager UIManager { get { return GetModule<IUIManager>(); } }
        public static IHotfixManager HotfixManager { get { return GetModule<IHotfixManager>(); } }
        public static IDataNodeManager DataNodeManager { get { return GetModule<IDataNodeManager>(); } }
        public static IEntityManager EntityManager { get { return GetModule<IEntityManager>(); } }
        public static IEventManager EventManager { get { return GetModule<IEventManager>(); } }
        public static ISceneManager SceneManager { get { return GetModule<ISceneManager>(); } }
        public static IWebRequestManager WebRequestManager { get { return GetModule<IWebRequestManager>(); } }
        public static IDownloadManager DownloadManager { get { return GetModule<IDownloadManager>(); } }
        public static IDataTableManager DataTableManager { get { return GetModule<IDataTableManager>(); } }

        /// <summary>
        /// 初始化当前AppDomain下的Module；
        /// 注意：初始化尽量只执行一次！！！
        /// </summary>
        public static void LaunchAppDomainModules()
        {
            if (isLaunched)
                return;
            MonoGameManager.Instance.ToString();
            GameManager.InitAppDomainModule();
        }
        /// <summary>
        /// 初始化目标程序集下的Module；
        /// 注意：初始化尽量只执行一次！！！
        /// </summary>
        /// <param name="assemblies">查询的目标程序集</param>
        public static void LaunchAssemblyModules(params System.Reflection.Assembly[] assemblies)
        {
            MonoGameManager.Instance.ToString();
            GameManager.InitAssemblyModule(assemblies);
        }
        public static void ReleaseLaunchedModules()
        {
            GameManager.Dispose();
        }
        public static T GetModule<T>() where T : class, IModuleManager
        {
            return GameManager.GetModule<T>();
        }
    }
}