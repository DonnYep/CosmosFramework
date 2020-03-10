using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEngine.Internal;
namespace Cosmos
{
    /// <summary>
    /// 当前设计为所有manager的一个管理器。
    /// 管理器对象都会通过这个对象的实例来调用，避免复杂化
    /// 可以理解为是一个Facade
    /// </summary>
    public sealed partial class GameManager : Singleton<GameManager>
    {
        // 模块表
        static Dictionary<string, IModule> moduleMap;
        //当前注册的模块总数
        int moduleCount = 0;
        public  int ModuleCount { get { return moduleCount; } }
        public GameObject InstanceObject { get { if (instanceObject == null)
                { instanceObject = new GameObject(this.GetType().ToString());Object.DontDestroyOnLoad(instanceObject); }
                return instanceObject; } }
        GameObject instanceObject;
        /// <summary>
        /// 注册模块
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="module"></param>
        public void RegisterModule(string  moduleName, IModule module )
        {
            if (!HasModule(moduleName))
            {
                moduleMap.Add(moduleName, module);
                module.OnInitialization();
                moduleCount++;
            }
            else
                Utility.DebugError("module \t" + moduleName + "is already exist!");
        }
        /// <summary>
        /// 注销模块
        /// </summary>
        /// <param name="moduleName"></param>
        public void DeregisterModule(string  moduleName)
        {
            if (HasModule(moduleName))
            {
                moduleMap[moduleName].OnTermination();
                moduleMap.Remove(moduleName);
                moduleCount--;
            }
            else
                Utility.DebugError("module \t" + moduleName + " is  not exist!");
        }
        public bool HasModule(string moduleName)
        {
            return moduleMap.ContainsKey(moduleName);
        }
        public IModule GetModule(string moduleName)
        {
            if (HasModule(moduleName))
                return moduleMap[moduleName];
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
            if (moduleMap == null){moduleMap = new Dictionary<string, IModule>();}
        }
        #region Methods
        /// <summary>
        /// 清理静态成员的对象，内存未释放完全
        /// </summary>
        static public void ClearGameManager()
        {
            Instance.ClearSingleton();
        }
        /// <summary>
        /// 清除单个实例，有一个默认参数。
        /// 默认延迟为0，表示立刻删除、
        /// 仅在场景中删除对应对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t">默认参数，表示延迟</param>
        public static void KillObject(Object obj, float delay=0)
        {
           GameObject.Destroy(obj,delay);
        }
        /// <summary>
        /// 立刻清理实例对象
        /// 会在内存中清理实例
        /// </summary>
        /// <param name="obj"></param>
        public static void KillObjectImmediate(Object obj)
        {
            GameObject.DestroyImmediate(obj);
        }
        /// <summary>
        /// 清除一组实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        public static void KillObjects<T>(List<T> objs)where T : Object
        {
            for (int i = 0; i < objs.Count; i++)
            {
                GameObject.Destroy(objs[i]);
            }
            objs.Clear();
        }
        public static void KillObjects<T>(HashSet<T> objs) where T : Object
        {
            foreach (var obj in objs)
            {
                GameObject.Destroy(obj);
            }
            objs.Clear();
        }
        #endregion
    }
    public enum ContainerState : int
    {
        Empty = -1,
        Hold = 0,
        Full = 1
    }
}
