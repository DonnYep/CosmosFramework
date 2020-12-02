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
using System.Reflection;
using System.Threading.Tasks;

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
            remove { fixedRefreshHandler -= value; }
        }
        public static event Action LateRefreshHandler
        {
            add { lateRefreshHandler += value; }
            remove { lateRefreshHandler -= value; }
        }
        public static event Action RefreshHandler
        {
            add { refreshHandler += value; }
            remove { refreshHandler -= value; }
        }
        static Action fixedRefreshHandler;
        static Action lateRefreshHandler;
        static Action refreshHandler;
        // 模块表
        static Dictionary<Type, Module> moduleDict;
        static Dictionary<Type, GameObject> moduleMountDict;
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
        public static void PreparatoryModule()
        {
            foreach (var module in moduleDict.Keys)
            {
                Utility.Debug.LogInfo($"Module :{module} has  been initialized");
            }
        }
        public static T GetModule<T>() where T : class, IModuleManager
        {
            Type type = typeof(T);
            moduleDict.TryGetValue(type, out var module);
            return module as T;
        }
        public static GameObject GetModuleMount<T>() where T : class, IModuleManager
        {
            Type type = typeof(T);
            GameObject moduleMount;
            var hasMount = moduleMountDict.TryGetValue(type, out moduleMount);
            if (!hasMount)
            {
                moduleMount = new GameObject(type.Name + "Module-->>Container");
                moduleMount.transform.SetParent(InstanceObject.transform);
                if (!moduleMountDict.TryAdd(type, moduleMount))
                {
                    KillObjectImmediate(moduleMount);
                }
            }
            return moduleMount;
        }
        static GameManager()
        {
            if (moduleDict == null)
            {
                moduleDict = new Dictionary<Type, Module>();
                moduleMountDict = new Dictionary<Type, GameObject>();
                try
                {
                    InstanceObject.gameObject.AddComponent<GameManagerAgent>();
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError(e);
                }
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
        static void ModuleInitialization(Module module)
        {
            var type = module.GetType();
            if (!HasModule(type))
            {
                module.OnInitialization();
                moduleDict.Add(type, module);
                moduleCount++;
            }
            else
                throw new ArgumentException($"Module : {type} is already exist!");
        }
        static void ModuleTermination(Module module)
        {
            var type = module.GetType();
            if (HasModule(type))
            {
                module.OnDeactive();
                var m = moduleDict[type];
                RefreshHandler -= module.OnRefresh;
                FixedRefreshHandler -= module.OnFixRefresh;
                LateRefreshHandler -= module.OnLateRefresh;
                moduleDict.Remove(type);
                moduleCount--;
                module.OnTermination();
                Utility.Debug.LogInfo($"Module :{module} is OnTermination", MessageColor.DARKBLUE);
            }
            else
                throw new ArgumentException($"Module : {module} is not exist!");
        }
        static void InitModule()
        {
            var modules = Utility.Assembly.GetInstancesByAttribute<ModuleAttribute, Module>();
            for (int i = 0; i < modules.Length; i++)
            {
                ModuleInitialization(modules[i]);
            }
            ActiveModule();
        }
        static void ActiveModule()
        {
            foreach (var module in moduleDict.Values)
            {
                
                (module as Module) .OnActive();
            }
            PrepareModule();
        }
        static void PrepareModule()
        {
            foreach (var mgr in moduleDict.Values)
            {
                var module = mgr as Module;
                module.OnPreparatory();
                RefreshHandler += module.OnRefresh;
                FixedRefreshHandler += module.OnFixRefresh;
                LateRefreshHandler += module.OnLateRefresh;
            }
        }
        #endregion
    }
}
