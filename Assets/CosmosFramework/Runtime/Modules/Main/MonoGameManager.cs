using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
namespace Cosmos
{
    /// <summary>
    /// GameManager是静态的，因此由此类代理完成生命周期
    /// </summary>
    [DisallowMultipleComponent]
    //[DefaultExecutionOrder(-1000)]
    public sealed class MonoGameManager : MonoSingleton<MonoGameManager>
    {
        DateTime previousTimeSinceStartup;
        /// <summary>
        /// 模块-mount字典；
        ///  key=>moduleType；value=>gameobject
        /// </summary>
        Dictionary<Type, GameObject> moduleMountDict;
        public bool IsPause { get; private set; }
        public bool Pause
        {
            get { return IsPause; }
            set
            {
                if (IsPause == value)
                    return;
                IsPause = value;
                if (IsPause)
                {
                    OnPause();
                }
                else
                {
                    OnUnPause();
                }
            }
        }

        event Action ApplicationQuitHandler;
        public void OnPause()
        {
            GameManager.OnPause();
        }
        public void OnUnPause()
        {
            GameManager.OnUnPause();
        }
        protected override void Awake()
        {
            base.Awake();
            gameObject.name = "CosmosRoot";
            DontDestroyOnLoad(this.gameObject);
            previousTimeSinceStartup = DateTime.Now;
            moduleMountDict = new Dictionary<Type, GameObject>();
        }
        public int ModuleCount { get { return GameManager.ModuleCount; } }
        public GameObject GetModuleGameObject(IModuleInstance module)
        {
            var type = module.GetType();
            var hasType = GameManager.HasModule(type);
            if (!hasType)
                return null;
            GameObject moduleMount;
            var hasMount = moduleMountDict.TryGetValue(type, out moduleMount);
            if (!hasMount)
            {
                moduleMount = new GameObject(type.Name + "-->>Instance");
                moduleMount.transform.SetParent(transform);
                moduleMountDict[type] = moduleMount;
            }
            else
            {
                if (moduleMount == null)
                {
                    moduleMount = new GameObject(type.Name + "-->>Instance");
                    moduleMount.transform.SetParent(transform);
                    moduleMountDict[type] = moduleMount;
                }
            }
            return moduleMount;
        }
        /// <summary>
        /// 清除单个实例，有一个默认参数。
        /// 默认延迟为0，表示立刻删除、
        /// 仅在场景中删除对应对象
        /// </summary>
        public static void KillObject(Object obj, float delay = 0)
        {
            GameObject.Destroy(obj, delay);
        }
        /// <summary>
        /// 立刻清理实例对象
        /// 会在内存中清理实例
        /// Editor适用
        /// </summary>
        public static void KillObjectImmediate(Object obj)
        {
            GameObject.DestroyImmediate(obj);
        }
        /// <summary>
        /// 清除一组实例
        /// </summary>
        public static void KillObjects<T>(IEnumerable<T> objs) where T : Object
        {
            foreach (var obj in objs)
            {
                GameObject.Destroy(obj);
            }
        }
        protected override void OnDestroy()
        {
            GameManager.Dispose();
        }
        private void FixedUpdate()
        {
            if (IsPause)
                return;
            GameManager.OnFixRefresh();
        }
        private void Update()
        {
            float deltaTime = (float)(DateTime.Now.Subtract(previousTimeSinceStartup).TotalMilliseconds / 1000.0f);
            previousTimeSinceStartup = DateTime.Now;

            if (IsPause)
                return;
            GameManager.OnRefresh();
            GameManager.OnElapseRefresh(deltaTime);
        }
        private void LateUpdate()
        {
            if (IsPause)
                return;
            GameManager.OnLateRefresh();
        }
        private void OnApplicationQuit()
        {
            ApplicationQuitHandler?.Invoke();
        }
    }
}