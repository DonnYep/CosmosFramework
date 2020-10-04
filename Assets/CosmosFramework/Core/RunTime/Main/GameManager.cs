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
                { instanceObject = new GameObject(typeof(GameManager).ToString()); 
                    Object.DontDestroyOnLoad(instanceObject); }
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
                    audioManager = new AudioManager();
                    ModuleInitialization(audioManager);
                }
                return audioManager;
            }
        }
        static ResourceManager resourceManager;
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (resourceManager == null)
                {
                    resourceManager = new ResourceManager();
                    ModuleInitialization(resourceManager);
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
                    objectPoolManager = new ObjectPoolManager();
                    ModuleInitialization(objectPoolManager);
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
                    networkManager = new NetworkManager();
                    ModuleInitialization(networkManager);
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
                    monoManager = new MonoManager();
                    ModuleInitialization(monoManager);
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
                    inputManager = new InputManager();
                    ModuleInitialization(inputManager);
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
                    uiManager = new UIManager();
                    ModuleInitialization(uiManager);
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
                    eventManager = new EventManager();
                    ModuleInitialization(eventManager);
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
                    sceneManager = new SceneManager();
                    ModuleInitialization(sceneManager);
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
                    fsmManager = new FSMManager();
                    ModuleInitialization(fsmManager);
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
                    configManager = new ConfigManager();
                    ModuleInitialization(configManager);
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
                    dataManager = new DataManager();
                    ModuleInitialization(dataManager);
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
                    controllerManager = new ControllerManager();
                    ModuleInitialization(controllerManager);
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
                    entityManager = new EntityManager();
                    ModuleInitialization(entityManager);
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
                    referencePoolManager = new ReferencePoolManager();
                    ModuleInitialization(referencePoolManager);
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
                    hotfixManager = new HotfixManager();
                 ModuleInitialization(hotfixManager);
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
        internal static void ModuleInitialization(IModule module)
        {
            var type = module.GetType();
            if (!HasModule(type))
            {
                module.OnInitialization();
                moduleDict.Add(module.GetType(), module);
                moduleCount++;
                RefreshHandler += module.OnRefresh;
                Utility.Debug.LogInfo($"Module :{module} is OnInitialization");
            }
            else
                throw new ArgumentException($"Module : {module} is already exist!");
        }
        internal static void ModuleTermination(IModule module)
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
        internal static bool HasModule(Type type)
        {
            return moduleDict.ContainsKey(type);
        }
        /// <summary>
        /// 构造函数，只有使用到时候才产生
        /// </summary>
        static GameManager()
        {
            if (moduleDict == null)
            {
                moduleDict = new Dictionary<Type, IModule>();
                InstanceObject.gameObject.AddComponent<GameManagerAgent>();
            }
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
        #endregion
    }
}
