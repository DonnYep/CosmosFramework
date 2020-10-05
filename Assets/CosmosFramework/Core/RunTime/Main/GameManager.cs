using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using Cosmos.Event;
using Cosmos.UI;
using Cosmos.Mono;
using Cosmos.Input;
using Cosmos.Scene;
using Cosmos.ObjectPool;
using Cosmos.Audio;
using Cosmos.Resource;
using Cosmos.Reference;
using Cosmos.Controller;
using Cosmos.FSM;
using Cosmos.Data;
using Cosmos.Config;
using Cosmos.Network;
using Cosmos.Entity;
using Cosmos.Hotfix;
using System;

namespace Cosmos
{
    /// <summary>
    /// 当前设计为所有manager的一个管理器。
    /// 管理器对象都会通过这个对象的实例来调用，避免复杂化
    /// 可以理解为是一个Facade
    /// </summary>
    internal static partial class GameManager
    {
        #region Properties
        public static event Action FixedRefreshHandler
        {
            add { fixedRefreshHandler += value; }
            remove
            {
                try { fixedRefreshHandler -= value; }
                catch (Exception e) { Utility.Debug.LogError(e); }
            }
        }
        public static event Action LateRefreshHandler
        {
            add { lateRefreshHandler += value; }
            remove
            {
                try { lateRefreshHandler -= value; }
                catch (Exception e) { Utility.Debug.LogError(e); }
            }
        }
        public static event Action RefreshHandler
        {
            add { refreshHandler += value; }
            remove
            {
                try { refreshHandler -= value; }
                catch (Exception e) { Utility.Debug.LogError(e); }
            }
        }
        static Action fixedRefreshHandler;
        static Action lateRefreshHandler;
        static Action refreshHandler;
        // 模块表
        static Dictionary<Type, IModule> moduleDict;
        internal static Dictionary<Type, IModule> ModuleDict { get { return moduleDict; } }
        /// <summary>
        /// 轮询更新委托
        /// </summary>
        public static bool IsPause { get; private set; }
        //当前注册的模块总数
        static int moduleCount = 0;
        internal static int ModuleCount { get { return moduleCount; } }
        internal static GameObject InstanceObject
        {
            get
            {
                if (instanceObject == null)
                {
                    instanceObject = new GameObject(typeof(GameManager).ToString());
                    Object.DontDestroyOnLoad(instanceObject);
                }
                return instanceObject;
            }
        }
        static GameObject instanceObject;
        static AudioManager audioManager;
        internal static AudioManager AudioManager
        {
            get
            {
                if (audioManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(AudioManager), out module);
                    audioManager = module as AudioManager;
                }
                return audioManager; ;
            }
        }
        static ResourceManager resourceManager;
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (resourceManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(ResourceManager), out module);
                    resourceManager = module as ResourceManager;
                }
                return resourceManager;
            }
        }
        static ObjectPoolManager objectPoolManager;
        internal static ObjectPoolManager ObjectPoolManager
        {
            get
            {
                if (objectPoolManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(ObjectPoolManager), out module);
                    objectPoolManager = module as ObjectPoolManager;
                }
                return objectPoolManager;
            }
        }
        static NetworkManager networkManager;
        internal static NetworkManager NetworkManager
        {
            get
            {
                if (networkManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(NetworkManager), out module);
                    networkManager = module as NetworkManager;
                }
                return networkManager;
            }
        }
        static MonoManager monoManager;
        internal static MonoManager MonoManager
        {
            get
            {
                if (monoManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(MonoManager), out module);
                    monoManager = module as MonoManager;
                }
                return monoManager;
            }
        }
        static InputManager inputManager;
        internal static InputManager InputManager
        {
            get
            {
                if (inputManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(InputManager), out module);
                    inputManager = module as InputManager;
                }
                return inputManager;
            }
        }
        static UIManager uiManager;
        internal static UIManager UIManager
        {
            get
            {
                if (uiManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(InputManager), out module);
                    uiManager = module as UIManager;
                }
                return uiManager;
            }
        }
        static EventManager eventManager;
        internal static EventManager EventManager
        {
            get
            {
                if (eventManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(EventManager), out module);
                    eventManager = module as EventManager;
                }
                return eventManager;
            }
        }
        static SceneManager sceneManager;
        internal static SceneManager SceneManager
        {
            get
            {
                if (sceneManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(SceneManager), out module);
                    sceneManager = module as SceneManager;
                }
                return sceneManager;
            }
        }
        static FSMManager fsmManager;
        internal static FSMManager FSMManager
        {
            get
            {
                if (fsmManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(FSMManager), out module);
                    fsmManager = module as FSMManager;
                }
                return fsmManager;
            }
        }
        static ConfigManager configManager;
        internal static ConfigManager ConfigManager
        {
            get
            {
                if (configManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(ConfigManager), out module);
                    configManager = module as ConfigManager;
                }
                return configManager;
            }
        }
        static DataManager dataManager;
        internal static DataManager DataManager
        {
            get
            {
                if (dataManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(DataManager), out module);
                    dataManager = module as DataManager;
                }
                return dataManager;
            }
        }
        static ControllerManager controllerManager;
        internal static ControllerManager ControllerManager
        {
            get
            {
                if (controllerManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(ControllerManager), out module);
                    controllerManager = module as ControllerManager;
                }
                return controllerManager;
            }
        }
        static EntityManager entityManager;
        internal static EntityManager EntityManager
        {
            get
            {
                if (entityManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(EntityManager), out module);
                    entityManager = module as EntityManager;
                }
                return entityManager;
            }
        }
        static ReferencePoolManager referencePoolManager;
        internal static ReferencePoolManager ReferencePoolManager
        {
            get
            {
                if (referencePoolManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(ReferencePoolManager), out module);
                    referencePoolManager = module as ReferencePoolManager;
                }
                return referencePoolManager;
            }
        }
        static HotfixManager hotfixManager;
        internal static HotfixManager HotfixManager
        {
            get
            {
                if (hotfixManager == null)
                {
                    IModule module;
                    moduleDict.TryGetValue(typeof(HotfixManager), out module);
                    hotfixManager = module as HotfixManager;
                }
                return hotfixManager;
            }
        }
        #endregion

        #region Methods
        public static void OnPause()
        {
            IsPause = true;
        }
        public static void OnUnPause()
        {
            IsPause = false;
        }
        public static void OnRefresh()
        {
            if (IsPause)
                return;
            refreshHandler?.Invoke();
        }
        public static void OnLateRefresh()
        {
            if (IsPause)
                return;
            lateRefreshHandler?.Invoke();
        }
        public static void OnFixRefresh()
        {
            if (IsPause)
                return;
            fixedRefreshHandler?.Invoke();
        }
        public static void CheckModule()
        {
            foreach (var module in moduleDict.Values)
            {
                Utility.Debug.LogInfo($"Module :{module} has already been initialized");
            }
        }
        static GameManager()
        {
            if (moduleDict == null)
            {
                moduleDict = new Dictionary<Type, IModule>();
                InstanceObject.gameObject.AddComponent<GameManagerAgent>();
                InitModule();
            }
        }
        internal static bool HasModule(Type type)
        {
            return moduleDict.ContainsKey(type);
        }
        /// <summary>
        /// 清理静态成员的对象，内存未释放完全
        /// </summary>
        internal static void Dispose()
        {
        }
        /// <summary>
        /// 清除单个实例，有一个默认参数。
        /// 默认延迟为0，表示立刻删除、
        /// 仅在场景中删除对应对象
        /// </summary>
        internal static void KillObject(Object obj, float delay = 0)
        {
            GameObject.Destroy(obj, delay);
        }
        /// <summary>
        /// 立刻清理实例对象
        /// 会在内存中清理实例
        /// </summary>
        internal static void KillObjectImmediate(Object obj)
        {
            GameObject.DestroyImmediate(obj);
        }
        /// <summary>
        /// 清除一组实例
        /// </summary>
        internal static void KillObjects<T>(List<T> objs) where T : Object
        {
            for (int i = 0; i < objs.Count; i++)
            {
                GameObject.Destroy(objs[i]);
            }
            objs.Clear();
        }
        internal static void KillObjects<T>(HashSet<T> objs) where T : Object
        {
            foreach (var obj in objs)
            {
                GameObject.Destroy(obj);
            }
            objs.Clear();
        }
        static void ModuleInitialization(IModule module)
        {
            var type = module.GetType();
            if (!HasModule(type))
            {
                module.OnInitialization();
                moduleDict.Add(module.GetType(), module);
                moduleCount++;
                RefreshHandler += module.OnRefresh;
                //Utility.Debug.LogInfo($"Module :{module} is OnInitialization");
            }
            else
                throw new ArgumentException($"Module : {module} is already exist!");
        }
        static void ModuleTermination(IModule module)
        {
            var type = module.GetType();
            if (HasModule(type))
            {
                module.OnTermination();
                var m = moduleDict[type];
                RefreshHandler -= m.OnRefresh;
                moduleDict.Remove(type);
                moduleCount--;
                Utility.Debug.LogInfo($"Module :{module} is OnTermination", MessageColor.DARKBLUE);
            }
            else
                throw new ArgumentException($"Module : {module} is not exist!");
        }
        static void InitModule()
        {
            var modules = Utility.Assembly.GetInstancesByAttribute<ModuleAttribute, IModule>();
            for (int i = 0; i < modules.Length; i++)
            {
                ModuleInitialization(modules[i]);
            }
            PrepareModule();
        }
        static void PrepareModule()
        {
            foreach (var module in moduleDict.Values)
            {
                module.OnPreparatory();
            }
        }
        #endregion
    }
}
