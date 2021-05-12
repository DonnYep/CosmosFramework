using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Cosmos
{
    public class CosmosEntry 
    {
        public static event Action FixedRefreshHandler
        {
            add { GameManager.FixedRefreshHandler+= value; }
            remove { GameManager.FixedRefreshHandler -= value; }
        }
        public static event Action LateRefreshHandler
        {
            add { GameManager.LateRefreshHandler+= value; }
            remove { GameManager.LateRefreshHandler -= value; }
        }
        public static event Action RefreshHandler
        {
            add { GameManager.RefreshHandler+= value; }
            remove { GameManager.RefreshHandler -= value; }
        }
        /// <summary>
        /// 时间流逝轮询委托；
        /// </summary>
        public static event Action<long> ElapseRefreshHandler
        {
            add { GameManager.ElapseRefreshHandler+= value; }
            remove { GameManager.ElapseRefreshHandler -= value; }
        }
        public static IAudioManager AudioManager { get { return GameManager.GetModule<IAudioManager>(); } }
        public static IControllerManager ControllerManager { get { return GameManager.GetModule<IControllerManager>(); } }
        public static IFSMManager FSMManager { get { return GameManager.GetModule<IFSMManager>(); } }
        public static IObjectPoolManager ObjectPoolManager { get { return GameManager.GetModule<IObjectPoolManager>(); } }
        public static IConfigManager ConfigManager { get { return GameManager.GetModule<IConfigManager>(); } }
        public static IInputManager InputManager { get { return GameManager.GetModule<IInputManager>(); } }
        public static INetworkManager NetworkManager { get { return GameManager.GetModule<INetworkManager>(); } }
        public static IResourceManager ResourceManager { get { return GameManager.GetModule<IResourceManager>(); } }
        public static IUIManager UIManager { get { return GameManager.GetModule<IUIManager>(); } }
        public static IHotfixManager HotfixManager { get { return GameManager.GetModule<IHotfixManager>(); } }
        public static IDataManager DataManager { get { return GameManager.GetModule<IDataManager>(); } }
        public static IEntityManager EntityManager { get { return GameManager.GetModule<IEntityManager>(); } }
        public static IEventManager EventManager { get { return GameManager.GetModule<IEventManager>(); } }
        public static ISceneManager SceneManager { get { return GameManager.GetModule<ISceneManager>(); } }

        public static GameObject AudioMount { get { return GameManager.GetModuleMount<IAudioManager>(); } }
        public static GameObject ControllerMount { get { return GameManager.GetModuleMount<IControllerManager>(); } }
        public static GameObject FSMMount { get { return GameManager.GetModuleMount<IFSMManager>(); } }
        public static GameObject ObjectPoolMount { get { return GameManager.GetModuleMount<IObjectPoolManager>(); } }
        public static GameObject ConfigMount { get { return GameManager.GetModuleMount<IConfigManager>(); } }
        public static GameObject InputMount { get { return GameManager.GetModuleMount<IInputManager>(); } }
        public static GameObject NetworkMount { get { return GameManager.GetModuleMount<INetworkManager>(); } }
        public static GameObject ResourceMount { get { return GameManager.GetModuleMount<IResourceManager>(); } }
        public static GameObject UIMount { get { return GameManager.GetModuleMount<IUIManager>(); } }
        public static GameObject HotfixMount { get { return GameManager.GetModuleMount<IHotfixManager>(); } }
        public static GameObject DataMount { get { return GameManager.GetModuleMount<IDataManager>(); } }
        public static GameObject EntityMount { get { return GameManager.GetModuleMount<IEntityManager>(); } }
        public static GameObject EventMount { get { return GameManager.GetModuleMount<IEventManager>(); } }
        public static GameObject SceneMount { get { return GameManager.GetModuleMount<ISceneManager>(); } }
        /// <summary>
        /// 启动当前AppDomain下的helper
        /// </summary>
        public static void LaunchAppDomainHelpers()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var length = assemblies.Length;
            for (int i = 0; i < length; i++)
            {
                var helper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IDebugHelper>(assemblies[i]);
                if (helper != null)
                {
                    Utility.Debug.SetHelper(helper);
                    break;
                }
            }
            for (int i = 0; i < length; i++)
            {
                var helper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IJsonHelper>(assemblies[i]);
                if (helper != null)
                {
                    Utility.Json.SetHelper(helper);
                    break;
                }
            }
            for (int i = 0; i < length; i++)
            {
                var helper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IMessagePackHelper>(assemblies[i]);
                if (helper != null)
                {
                    Utility.MessagePack.SetHelper(helper);
                    break;
                }
            }
        }
        /// <summary>
        /// 启动目标程序集下的helper
        /// </summary>
        /// <param name="assembly">查询的目标程序集</param>
        public static void LaunchAssemblyHelpers(System.Reflection.Assembly assembly)
        {
            var debugHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IDebugHelper>(assembly);
            if (debugHelper!= null)
            {
                Utility.Debug.SetHelper(debugHelper);
            }
            var jsonHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IJsonHelper>(assembly);
            if (jsonHelper!= null)
            {
                Utility.Json.SetHelper(jsonHelper);
            }
            var mpHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IMessagePackHelper>(assembly);
            if (mpHelper != null)
            {
                Utility.MessagePack.SetHelper(mpHelper);
            }
        }
        /// <summary>
        /// 初始化当前AppDomain下的Module；
        /// 注意：初始化尽量只执行一次！！！
        /// </summary>
        public static void LaunchAppDomainModules()
        {
            GameManager.InitAppDomainModule();
        }
        /// <summary>
        /// 初始化目标程序集下的Module；
        /// 注意：初始化尽量只执行一次！！！
        /// </summary>
        /// <param name="assemblies">查询的目标程序集</param>
        public static void LaunchAssemblyModules(params System.Reflection .Assembly[]  assemblies)
        {
            GameManager.InitAssemblyModule(assemblies);
        }
    }
}