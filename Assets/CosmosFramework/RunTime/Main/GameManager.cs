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
using System.Reflection;
using System;

namespace Cosmos
{
    /// <summary>
    /// 当前设计为所有manager的一个管理器。
    /// 管理器对象都会通过这个对象的实例来调用，避免复杂化
    /// 可以理解为是一个Facade
    /// </summary>
    internal sealed partial class GameManager : Singleton<GameManager>
    {
        // 模块表
        static Dictionary<string, IModule> moduleDict;
        internal static Dictionary<string, IModule> ModuleDict { get { return moduleDict; } }
        //当前注册的模块总数
        int moduleCount = 0;
        internal int ModuleCount { get { return moduleCount; } }
        internal GameObject InstanceObject
        {
            get
            {
                if (instanceObject == null)
                { instanceObject = new GameObject(this.GetType().ToString()); Object.DontDestroyOnLoad(instanceObject); }
                return instanceObject;
            }
        }
        GameObject instanceObject;
        /// <summary>
        /// 注册模块
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="module"></param>
        internal void RegisterModule(string moduleName, IModule module)
        {
            if (!HasModule(moduleName))
            {
                moduleDict.Add(moduleName, module);
                moduleCount++;
                Utility.DebugLog("Module:\"" + moduleName + "\" " + "  is OnInitialization" + "\n based on GameManager");
            }
            else
                throw new ArgumentException("Module:\"" + moduleName + "\" " + " is already exist!");
        }
        /// <summary>
        /// 注销模块
        /// </summary>
        /// <param name="moduleName"></param>
        internal void DeregisterModule(string moduleName)
        {
            if (HasModule(moduleName))
            {
                moduleDict.Remove(moduleName);
                moduleCount--;
                Utility.DebugLog("Module:\"" + moduleName + "\" " + "  is OnTermination" + "\n based on GameManager", MessageColor.DARKBLUE);
            }
            else
                throw new ArgumentException("Module:\"" + moduleName + "\" " + " is  not exist!");
        }
        internal bool HasModule(string moduleName)
        {
            return moduleDict.ContainsKey(moduleName);
        }
        internal IModule GetModule(string moduleName)
        {
            if (HasModule(moduleName))
                return moduleDict[moduleName];
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 构造函数，只有使用到时候才产生
        /// </summary>
        public GameManager()
        {
            if (moduleDict == null)
            {
                moduleDict = new Dictionary<string, IModule>();
                InstanceObject.gameObject.AddComponent<GameManagerAgent>();
            }
        }
        #region Methods
        /// <summary>
        /// 清理静态成员的对象，内存未释放完全
        /// </summary>
        internal static void ClearGameManager()
        {
            Instance.Dispose();
        }
        /// <summary>
        /// 清除单个实例，有一个默认参数。
        /// 默认延迟为0，表示立刻删除、
        /// 仅在场景中删除对应对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t">默认参数，表示延迟</param>
        internal static void KillObject(Object obj, float delay = 0)
        {
            GameObject.Destroy(obj, delay);
        }
        /// <summary>
        /// 立刻清理实例对象
        /// 会在内存中清理实例
        /// </summary>
        /// <param name="obj"></param>
        internal static void KillObjectImmediate(Object obj)
        {
            GameObject.DestroyImmediate(obj);
        }
        /// <summary>
        /// 清除一组实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
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
        #region Module
        static AudioManager audioManager;
        internal static AudioManager AudioManager
        {
            get
            {
                if (audioManager == null)
                {
                    audioManager = new AudioManager();
                    Instance.ModuleInitialization(audioManager);
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
                    Instance.ModuleInitialization(resourceManager);
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
                    Instance.ModuleInitialization(objectPoolManager);
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
                    Instance.ModuleInitialization(networkManager);
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
                    Instance.ModuleInitialization(monoManager);
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
                    Instance.ModuleInitialization(inputManager);
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
                    Instance.ModuleInitialization(uiManager);
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
                    Instance.ModuleInitialization(eventManager);
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
                    Instance.ModuleInitialization(sceneManager);
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
                    Instance.ModuleInitialization(fsmManager);
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
                    Instance.ModuleInitialization(configManager);
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
                    Instance.ModuleInitialization(dataManager);
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
                    Instance.ModuleInitialization(controllerManager);
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
                    Instance.ModuleInitialization(entityManager);
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
                    Instance.ModuleInitialization(referencePoolManager);
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
                    Instance.ModuleInitialization(hotfixManager);
                }
                return hotfixManager;
            }
        }
        internal void ModuleInitialization(IModule module)
        {
            module.OnInitialization();
            Instance.RegisterModule(module.ModuleFullyQualifiedName, module);
        }
        internal void ModuleTermination(IModule module)
        {
            module.OnTermination();
            Instance.DeregisterModule(module.ModuleFullyQualifiedName);
        }
        #endregion
    }
    internal enum ContainerState : short
    {
        Empty = -1,
        Hold = 0,
        Full = 1
    }
}
